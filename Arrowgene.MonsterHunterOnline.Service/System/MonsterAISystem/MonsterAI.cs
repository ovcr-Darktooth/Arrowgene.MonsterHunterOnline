using System;
using System.Threading;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Service.Data;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem
{
    public enum MonsterAIState { Idle, Chase, Attack, Dead }

    public class MonsterAI : IDisposable
    {
        private const float AggroRange = 500f;
        private const float AttackRange = 5.0f;
        private const float MoveSpeedPerTick = 1.5f;
        private const int TickMs = 200;

        private static readonly ILogger Logger = LogProvider.Logger(typeof(MonsterAI));

        public uint NetId { get; }
        public uint RenderNetId { get; }
        public int MonsterInfoId { get; }
        public CSVec3 Position { get; private set; }
        public MonsterAIState State { get; private set; }
        public int MaxHp { get; }
        public int CurrentHp { get; set; }

        private readonly MonsterAIManager _manager;
        private readonly SequenceManager _sequenceManager;
        private SequenceSet _sequenceSet;

        private Timer _timer;
        private long _syncTime;
        private bool _disposed;

        // Sequence State
        private SequenceData _currentSequence;
        private float _sequenceTime;
        private CSVec3 _sequenceStartPos;
        private CSQuat _sequenceStartRot;
        private float _sequenceStartYaw;
        private (float x, float y, float z) _sequenceLocalOrigin;
        
        public MonsterAI(uint netId, uint renderNetId, int monsterInfoId, CSVec3 spawnPos, MonsterAIManager manager, SequenceManager sequenceManager, int maxHp)
        {
            NetId = netId;
            RenderNetId = renderNetId;
            MonsterInfoId = monsterInfoId;
            Position = new CSVec3 { x = spawnPos.x, y = spawnPos.y, z = spawnPos.z };
            State = MonsterAIState.Idle;
            MaxHp = maxHp > 0 ? maxHp : 100;
            CurrentHp = MaxHp;
            _manager = manager;
            _sequenceManager = sequenceManager;

            string refName = GetMonsterRefName(monsterInfoId);
            _sequenceSet = _sequenceManager?.GetOrLoad(refName);

            _timer = new Timer(Tick, null, TickMs, TickMs);
        }

        private string GetMonsterRefName(int infoId)
        {
            if (infoId == 60030) return "em003"; // Test Bulldrome
            if (infoId == 60010) return "em001"; // Test Bulldrome
            if (infoId == 39004) return "em003";
            return "em001";
        }

        private void Tick(object _)
        {
            if (_disposed) return;

            try
            {
                var (target, dist) = _manager.FindNearestPlayer(Position);

                // If executing a sequence, lock state until it finishes
                if (_currentSequence != null)
                {
                    _sequenceTime += (TickMs / 1000f);

                    if (_sequenceTime >= _currentSequence.TimeRange)
                    {
                        Logger.Debug($"Monster {NetId} finished sequence {_currentSequence.Name}");
                        _currentSequence = null;
                    }
                    else
                    {
                        // Check hitboxes
                        foreach (var hitEvent in _currentSequence.PhysicEvents)
                        {
                            if (Math.Abs(_sequenceTime - hitEvent.Time) < (TickMs / 1000f))
                            {
                                Logger.Info($"Monster {NetId} HitEvent -> {hitEvent.Name} (Firemode:{hitEvent.Firemode}, AttackData:{hitEvent.AttackData})");
                            }
                        }

                        // Root-motion: sample baked Position curves and advance authoritative transform.
                        if (_currentSequence.Position.HasAny)
                        {
                            var local = _currentSequence.Position.Sample(_sequenceTime);
                            float lx = local.x - _sequenceLocalOrigin.x;
                            float ly = local.y - _sequenceLocalOrigin.y;
                            float lz = local.z - _sequenceLocalOrigin.z;

                            // Local frame: +Y forward, +X right (CryEngine). Yaw measured CCW from world +X.
                            float c = MathF.Cos(_sequenceStartYaw);
                            float s = MathF.Sin(_sequenceStartYaw);
                            float wx = lx * s + ly * c;
                            float wy = -lx * c + ly * s;

                            var newPos = new CSVec3(
                                _sequenceStartPos.x + wx,
                                _sequenceStartPos.y + wy,
                                _sequenceStartPos.z + lz);
                            var velocity = ComputeVelocity(Position, newPos, TickMs / 1000f);
                            Position = newPos;
                            SendMovestate(newPos, _sequenceStartRot, velocity);
                        }

                        return; // Lock behavior while animating
                    }
                }

                if (target == null || dist > AggroRange)
                {
                    if (State != MonsterAIState.Idle)
                    {
                        State = MonsterAIState.Idle;
                        Logger.Debug($"Monster {NetId} -> Idle");
                        _manager.BroadcastMonsterActiveState(NetId, 1, Position, CurrentSyncTimeMs());
                    }
                    if (_currentSequence == null)
                    {
                        // Optionally broadcast idle, but playerstate didn't bother when target was lost, just didn't send anything.
                    }
                    return;
                }

                uint targetId = target.Character?.Id ?? 0;
                CSVec3 targetPos = target.State.Position ?? target.State.InitSpawnPos;
                float dist2D = Distance2D(Position, targetPos);
                float tickSeconds = TickMs / 1000f;

                if (dist2D <= AttackRange)
                {
                    State = MonsterAIState.Attack;

                    CSQuat rot = LookAtQuat(Position, targetPos);
                    float yawStart = MathF.Atan2(targetPos.y - Position.y, targetPos.x - Position.x);
                    CSVec3 zeroSpeed = new(0, 0, 0);

                    string attackSequence = "Head"; // Default fallback
                    if (_sequenceSet != null && _sequenceSet.Sequences.ContainsKey("Attack")) attackSequence = "Attack";
                    if (_sequenceSet != null && _sequenceSet.Sequences.ContainsKey("Head")) attackSequence = "Head";
                    if (_sequenceSet != null && _sequenceSet.Sequences.ContainsKey("DragonDash")) attackSequence = "DragonDash";

                    SendLocomotion(Position, rot, targetPos, zeroSpeed, attackSequence, 0, true, true);
                    SendMovestate(Position, rot, zeroSpeed);
                    BroadcastSequenceState(attackSequence, 0f, Position, rot);

                    // Lock sequence + capture start pose for root-motion replay
                    _sequenceStartPos = CloneVec(Position);
                    _sequenceStartRot = rot;
                    _sequenceStartYaw = yawStart;
                    if (_sequenceSet != null && _sequenceSet.Sequences.TryGetValue(attackSequence, out var seq))
                    {
                        _currentSequence = seq;
                        _sequenceTime = 0f;
                        _sequenceLocalOrigin = seq.Position.HasAny ? seq.Position.Sample(0f) : (0f, 0f, 0f);
                    }
                    else
                    {
                        // Fake sequence lock if no data (like PlayerState did with 1.63 seconds)
                        _currentSequence = new SequenceData { Name = attackSequence, TimeRange = 1.63f };
                        _sequenceTime = 0f;
                        _sequenceLocalOrigin = (0f, 0f, 0f);
                    }
                }
                else
                {
                    State = MonsterAIState.Chase;
                    
                    CSVec3 nextPos = StepToward(Position, targetPos, MoveSpeedPerTick);
                    CSQuat moveRot = LookAtQuat(Position, targetPos);
                    CSVec3 moveSpeed = ComputeVelocity(Position, nextPos, tickSeconds);

                    string moveSequence = "Run";
                    if (_sequenceSet != null && _sequenceSet.Sequences.ContainsKey("Dash")) moveSequence = "Dash";
                    if (_sequenceSet != null && _sequenceSet.Sequences.ContainsKey("Run_F")) moveSequence = "Run_F";

                    SendLocomotion(Position, moveRot, nextPos, moveSpeed, moveSequence, 0, false, false);
                    SendMovestate(nextPos, moveRot, moveSpeed);

                    Position = CloneVec(nextPos);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Monster {NetId} tick error: {ex.Message}");
            }
        }

        private void SendLocomotion(CSVec3 position, CSQuat rotation, CSVec3 targetPos, CSVec3 moveSpeed, string animSequence, uint skillId, bool restartAnim, bool needTargetAttackPos)
        {
            var locomotion = new CSMonsterLocomotion
            {
                SteeringEnabled = 1,
                SyncTime = CurrentSyncTimeMs(),
                MonsterID = NetId,
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
            _manager.BroadcastLcm(locomotion);
        }

        private void SendMovestate(CSVec3 position, CSQuat rotation, CSVec3 speed)
        {
            var movestate = new CSMonsterMovestate
            {
                SyncTime = CurrentSyncTimeMs(),
                MonsterID = NetId,
                Location = CloneVec(position),
                Rotation = rotation,
                Speed = CloneVec(speed),
            };
            _manager.BroadcastMovestate(movestate);
        }

        private void BroadcastSequenceState(string animSeq, float curTime, CSVec3 pos, CSQuat rot)
        {
            var sq = new CSMonsterSequenceState
            {
                MonsterID = NetId,
                AnimSeqName = animSeq ?? string.Empty,
                CurTime = curTime,
                Location = CloneVec(pos),
                Rotation = rot
            };
            _manager.BroadcastSequenceState(sq);
        }

        public void Dispose()
        {
            _disposed = true;
            _timer?.Dispose();
            _timer = null;
            _manager.BroadcastMonsterActiveState(NetId, 0, Position, CurrentSyncTimeMs());
        }

        // --- MATH REPLICATED FROM PLAYERSTATE.CS TO ENSURE PERFECT NETWORK SYNCHRONIZATION ---

        private static float Distance2D(CSVec3 from, CSVec3 to)
        {
            if (from == null || to == null) return 9999f;
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
    }
}
