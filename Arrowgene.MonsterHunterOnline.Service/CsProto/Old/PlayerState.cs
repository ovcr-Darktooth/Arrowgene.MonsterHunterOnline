using System;
using System.Threading;
using System.Threading.Tasks;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Protocol.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto;

namespace Arrowgene.MonsterHunterOnline.Service;

/// <summary>
///  this is a temporary holder for central management of packet and information,
/// it can be considered the "playground" for now.
///
/// On*- function are lifecycle hooks
/// Send*- functions are to send specific data that has been consistently populated
/// </summary>
public class PlayerState
{
    private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(PlayerState));

    private const int BattleMonsterTickMs = 200;
    private const float BattleMonsterMoveSpeed = 1.5f;      // units per tick (~7.5 u/s at 200ms)
    private const float BattleMonsterAttackRange = 5.0f;
    private const string BattleMonsterMoveSequence = "Run";
    private const string BattleMonsterAttackSequence = "Head";
    private static readonly TimeSpan BattleMonsterAttackDuration = TimeSpan.FromSeconds(1.63);
    private static readonly TimeSpan BattleMonsterAttackCooldown = TimeSpan.FromSeconds(3);

    private readonly object _battleMonsterLock = new();
    private readonly Client _client;
    private CancellationTokenSource _battleMonsterLoopCts;
    private Task _battleMonsterLoopTask;

    public static Server Server;
    public int levelId { get; set; }
    public int prevLevelId { get; set; }
    public CSVec3 Position { get; set; }
    public CSVec3 PendingMonsterSpawnPos { get; set; }
    public uint? PendingMonsterNetId { get; set; }
    public uint? ActiveMonsterNetId { get; private set; }
    public CSVec3 ActiveMonsterPos { get; private set; }
    public int MainInstanceLevelId { get; set; }
    public bool SelectRoleTrigger { get; set; }

    public CSQuatT InitSpawnPose = new CSQuatT()
    {
        q = new CSQuat()
        {
            v = new CSVec3() { x = 10, y = 10, z = 10 },
            w = 10
        },
        t = new CSVec3() { x = 404.91379f, y = 396.74976f, z = 85.0f }
    };

    public CSVec3 InitSpawnPos = new CSVec3()
    {
        x = 404.91379f,
        y = 396.74976f,
        z = 85.0f
    };

    public int InitLevelId = 150101;

    public PlayerState(Client client)
    {
        _client = client;
        if (Position == null)
        {
            Position = InitSpawnPos;
        }
    }

    public void StartBattleMonsterLoop(uint netId, CSVec3 spawnPos)
    {
        StopBattleMonsterLoop();

        CancellationTokenSource loopCts = new();

        lock (_battleMonsterLock)
        {
            ActiveMonsterNetId = netId;
            ActiveMonsterPos = CloneVec(spawnPos);
            _battleMonsterLoopCts = loopCts;
            _battleMonsterLoopTask = Task.Run(() => RunBattleMonsterLoop(netId, loopCts.Token), loopCts.Token);
        }

        Logger.Info(_client,
            $"Start battle monster loop NetId=0x{netId:X8} Spawn={FormatVec(spawnPos)} Speed={BattleMonsterMoveSpeed} AttackRange={BattleMonsterAttackRange} AttackSeq={BattleMonsterAttackSequence}");
    }

    public void StopBattleMonsterLoop()
    {
        CancellationTokenSource loopCts;
        Task loopTask;
        uint? netId;

        lock (_battleMonsterLock)
        {
            loopCts = _battleMonsterLoopCts;
            loopTask = _battleMonsterLoopTask;
            netId = ActiveMonsterNetId;

            _battleMonsterLoopCts = null;
            _battleMonsterLoopTask = null;
            ActiveMonsterNetId = null;
            ActiveMonsterPos = null;
        }

        if (loopCts == null)
        {
            return;
        }

        try
        {
            loopCts.Cancel();
        }
        catch
        {
            // ignore cancellation races during shutdown
        }

        if (loopTask != null)
        {
            _ = loopTask.ContinueWith(_ => loopCts.Dispose());
        }
        else
        {
            loopCts.Dispose();
        }

        if (netId != null)
        {
            Logger.Info(_client, $"Stop battle monster runtime loop NetId=0x{netId.Value:X8}");
        }
    }

    private async Task RunBattleMonsterLoop(uint netId, CancellationToken cancellationToken)
    {
        float tickSeconds = BattleMonsterTickMs / 1000.0f;
        DateTime attackCooldownUntilUtc = DateTime.UtcNow + TimeSpan.FromSeconds(1);
        DateTime attackingUntilUtc = DateTime.MinValue;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                CSVec3 currentPos;

                lock (_battleMonsterLock)
                {
                    if (ActiveMonsterNetId != netId || ActiveMonsterPos == null)
                    {
                        return;
                    }

                    currentPos = CloneVec(ActiveMonsterPos);
                }

                DateTime nowUtc = DateTime.UtcNow;
                CSVec3 playerPos = SnapshotPlayerPosition();
                float dist = Distance2D(currentPos, playerPos);

                // Attacking — hold position until animation finishes
                if (nowUtc < attackingUntilUtc)
                {
                    await Task.Delay(BattleMonsterTickMs, cancellationToken);
                    continue;
                }

                // In attack range and cooldown elapsed → attack
                if (dist <= BattleMonsterAttackRange && nowUtc >= attackCooldownUntilUtc)
                {
                    CSQuat rot = LookAtQuat(currentPos, playerPos);
                    CSVec3 zeroSpeed = new(0, 0, 0);

                    SendBattleMonsterLocomotion(netId, currentPos, rot, playerPos, zeroSpeed,
                        BattleMonsterAttackSequence, skillId: 0, restartAnim: true, needTargetAttackPos: true);
                    SendBattleMonsterMovestate(netId, currentPos, rot, zeroSpeed);
                    SendBattleMonsterSequenceState(netId, BattleMonsterAttackSequence, 0f, currentPos, rot);

                    Logger.Info(_client,
                        $"Battle monster attack NetId=0x{netId:X8} Seq={BattleMonsterAttackSequence} MonsterPos={FormatVec(currentPos)} PlayerPos={FormatVec(playerPos)} Dist={dist:F2}");

                    attackingUntilUtc = nowUtc + BattleMonsterAttackDuration;
                    attackCooldownUntilUtc = nowUtc + BattleMonsterAttackCooldown;

                    await Task.Delay(BattleMonsterTickMs, cancellationToken);
                    continue;
                }

                // Move toward player
                CSVec3 nextPos = StepToward(currentPos, playerPos, BattleMonsterMoveSpeed);
                CSQuat moveRot = LookAtQuat(currentPos, playerPos);
                CSVec3 moveSpeed = ComputeVelocity(currentPos, nextPos, tickSeconds);

                SendBattleMonsterLocomotion(netId, currentPos, moveRot, nextPos, moveSpeed,
                    BattleMonsterMoveSequence, skillId: 0, restartAnim: false, needTargetAttackPos: false);
                SendBattleMonsterMovestate(netId, nextPos, moveRot, moveSpeed);

                lock (_battleMonsterLock)
                {
                    if (ActiveMonsterNetId == netId)
                    {
                        ActiveMonsterPos = CloneVec(nextPos);
                    }
                }

                await Task.Delay(BattleMonsterTickMs, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // expected on stop
        }
        catch (Exception ex)
        {
            Logger.Exception(_client, ex);
        }
    }

    private void SendBattleMonsterLocomotion(uint netId, CSVec3 position, CSQuat rotation, CSVec3 targetPos, CSVec3 moveSpeed, string animSequence, uint skillId, bool restartAnim, bool needTargetAttackPos)
    {
        CSMonsterLocomotion locomotion = new()
        {
            SteeringEnabled = 1,
            SyncTime = CurrentSyncTimeMs(),
            MonsterID = netId,
            AnimSeqName = animSequence ?? string.Empty,
            SkillID = (int)skillId,
            MoveSpeed = CloneVec(moveSpeed),
            MonsterPos = CloneVec(position),
            MonsterRot = rotation,
            TargetDis = new CSVec3(targetPos.x - position.x, targetPos.y - position.y, targetPos.z - position.z),
            TargetAttackPos = CloneVec(targetPos),
            NeedTargetAttackPos = needTargetAttackPos ? (byte)1 : (byte)0,
            SkillSpeed = 1.0f,
            RestartAnim = restartAnim ? (byte)1 : (byte)0,
            SetRotate = 1,
            SetPos = 1,
        };

        _client.SendCsPacket(NewCsPacket.MonsterLCM(locomotion));
    }

    private void SendBattleMonsterMovestate(uint netId, CSVec3 position, CSQuat rotation, CSVec3 speed)
    {
        CSMonsterMovestate movestate = new()
        {
            SyncTime = CurrentSyncTimeMs(),
            MonsterID = netId,
            Location = CloneVec(position),
            Rotation = rotation,
            Speed = CloneVec(speed),
        };

        _client.SendCsPacket(NewCsPacket.MonsterMovestate(movestate));
    }

    private void SendBattleMonsterSequenceState(uint netId, string animSequence, float currentTime, CSVec3 position, CSQuat rotation)
    {
        CSMonsterSequenceState sequenceState = new()
        {
            MonsterID = netId,
            AnimSeqName = animSequence ?? string.Empty,
            CurTime = currentTime,
            Location = CloneVec(position),
            Rotation = rotation,
        };

        _client.SendCsPacket(NewCsPacket.MonsterSequenceState(sequenceState));
    }

    private CSVec3 SnapshotPlayerPosition()
    {
        CSVec3 pos = Position ?? InitSpawnPos;
        return CloneVec(pos);
    }

    private static float Distance2D(CSVec3 from, CSVec3 to)
    {
        float deltaX = to.x - from.x;
        float deltaY = to.y - from.y;
        return MathF.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
    }

    private static CSVec3 StepToward(CSVec3 from, CSVec3 to, float maxStep)
    {
        float dx = to.x - from.x;
        float dy = to.y - from.y;
        float dist = MathF.Sqrt(dx * dx + dy * dy);
        if (dist <= maxStep)
        {
            return new CSVec3(to.x, to.y, from.z);
        }

        float scale = maxStep / dist;
        return new CSVec3(from.x + dx * scale, from.y + dy * scale, from.z);
    }

    private static long CurrentSyncTimeMs()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static CSQuat LookAtQuat(CSVec3 from, CSVec3 to)
    {
        float deltaX = to.x - from.x;
        float deltaY = to.y - from.y;

        if (MathF.Abs(deltaX) < 0.001f && MathF.Abs(deltaY) < 0.001f)
        {
            return new CSQuat(1.0f, 0, 0, 0);
        }

        return YawQuat(MathF.Atan2(deltaY, deltaX));
    }

    private static CSQuat YawQuat(float yaw)
    {
        float halfYaw = yaw * 0.5f;
        return new CSQuat(MathF.Cos(halfYaw), 0, 0, MathF.Sin(halfYaw));
    }

    private static CSVec3 ComputeVelocity(CSVec3 from, CSVec3 to, float deltaSeconds)
    {
        if (deltaSeconds <= 0.0f)
        {
            return new CSVec3();
        }

        return new CSVec3(
            (to.x - from.x) / deltaSeconds,
            (to.y - from.y) / deltaSeconds,
            (to.z - from.z) / deltaSeconds);
    }

    private static CSVec3 CloneVec(CSVec3 vec)
    {
        return new CSVec3(vec.x, vec.y, vec.z);
    }

    private static string FormatVec(CSVec3 vec)
    {
        return $"({vec.x:F3}, {vec.y:F3}, {vec.z:F3})";
    }
}
