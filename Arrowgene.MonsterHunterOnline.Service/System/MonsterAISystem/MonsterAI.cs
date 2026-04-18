using System;
using System.Threading;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem
{
    public enum MonsterAIState { Idle, Chase, Attack, Dead }

    public class MonsterAI : IDisposable
    {
        private const float AggroRange = 500f;
        private const float AttackRange = 6f;
        private const float MoveSpeedPerTick = 3f;
        private const int TickMs = 1000;

        private static readonly ILogger Logger = LogProvider.Logger(typeof(MonsterAI));

        public uint NetId { get; }
        public uint RenderNetId { get; }
        public int MonsterInfoId { get; }
        public CSVec3 Position { get; private set; }
        public MonsterAIState State { get; private set; }

        private readonly MonsterAIManager _manager;
        private Timer _timer;
        private long _syncTime;
        private bool _disposed;

        public MonsterAI(uint netId, uint renderNetId, int monsterInfoId, CSVec3 spawnPos, MonsterAIManager manager)
        {
            NetId = netId;
            RenderNetId = renderNetId;
            MonsterInfoId = monsterInfoId;
            Position = new CSVec3 { x = spawnPos.x, y = spawnPos.y, z = spawnPos.z };
            State = MonsterAIState.Idle;
            _manager = manager;
            _timer = new Timer(Tick, null, TickMs, TickMs);
        }

        private void Tick(object _)
        {
            if (_disposed) return;

            try
            {
                _syncTime += TickMs;
                var (target, dist) = _manager.FindNearestPlayer(Position);

                Logger.Info($"Monster {NetId} tick: target={(target == null ? "NULL" : target.Identity)}, dist={(dist == float.MaxValue ? "MAX(no pos)" : dist.ToString("F1"))}, state={State}");

                if (target == null || dist > AggroRange)
                {
                    if (State != MonsterAIState.Idle)
                    {
                        State = MonsterAIState.Idle;
                        Logger.Debug($"Monster {NetId} -> Idle");
                        // Notify clients: monster is still active but returned to idle
                        _manager.BroadcastMonsterActiveState(NetId, 1, Position, _syncTime);
                    }
                    BroadcastLocomotion("Idle", 0, new CSVec3());
                    _manager.BroadcastMonsterActiveState(0, 1, Position, _syncTime);
                    return;
                }

                uint targetId = target.Character?.Id ?? 0;
                CSVec3 targetPos = target.State.Position;
                float dx = targetPos.x - Position.x;
                float dy = targetPos.y - Position.y;
                float dz = targetPos.z - Position.z;

                if (dist <= AttackRange)
                {
                    if (State != MonsterAIState.Attack)
                    {
                        State = MonsterAIState.Attack;
                        Logger.Debug($"Monster {NetId} -> Attack target={targetId}");
                        // Notify clients: monster entered attack state
                        _manager.BroadcastMonsterActiveState(NetId, 1, Position, _syncTime);
                    }
                    BroadcastLocomotion("Attack", targetId, new CSVec3());
                    _manager.BroadcastMonsterActiveState(0, 1, Position, _syncTime);
                }
                else
                {
                    if (State != MonsterAIState.Chase)
                    {
                        State = MonsterAIState.Chase;
                        Logger.Debug($"Monster {NetId} -> Chase target={targetId}");
                        // Notify clients: monster started chasing
                        _manager.BroadcastMonsterActiveState(NetId, 1, Position, _syncTime);
                    }
                    float scale = MoveSpeedPerTick / dist;
                    Position = new CSVec3
                    {
                        x = Position.x + dx * scale,
                        y = Position.y + dy * scale,
                        z = Position.z + dz * scale,
                    };
                    float tickSec = TickMs / 1000f;
                    var speed = new CSVec3
                    {
                        x = (dx * scale) / tickSec,
                        y = (dy * scale) / tickSec,
                        z = (dz * scale) / tickSec,
                    };
                    Logger.Debug($"Monster {NetId} -> Moving to target={targetId}");
                    BroadcastLocomotion("Run_F", targetId, speed);
                    _manager.BroadcastMonsterActiveState(0, 1, Position, _syncTime);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Monster {NetId} tick error: {ex.Message}");
            }
        }

        private void BroadcastLocomotion(string animSeq, uint targetId, CSVec3 moveSpeed)
        {
            var lcm = new CSMonsterLocomotion
            {
                MonsterID = NetId,
                SyncTime = _syncTime,
                AnimSeqName = animSeq,
                MonsterPos = new CSVec3 { x = Position.x, y = Position.y, z = Position.z },
                MonsterRot = new CSQuat(1, 0, 0, 0),
                MoveSpeed = moveSpeed,
                TargetID = targetId,
                SetPos = 1,
                SteeringEnabled = targetId != 0 ? (byte)1 : (byte)0,
            };
            _manager.BroadcastLcm(lcm);

            if (RenderNetId != 0)
            {
                var renderLcm = new CSMonsterLocomotion
                {
                    MonsterID = RenderNetId,
                    SyncTime = _syncTime,
                    AnimSeqName = animSeq,
                    MonsterPos = new CSVec3 { x = Position.x, y = Position.y, z = Position.z },
                    MonsterRot = new CSQuat(1, 0, 0, 0),
                    MoveSpeed = moveSpeed,
                    TargetID = targetId,
                    SetPos = 1,
                    SteeringEnabled = targetId != 0 ? (byte)1 : (byte)0,
                };
                _manager.BroadcastLcm(renderLcm);
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _timer?.Dispose();
            _timer = null;
            // ActiveState=0 signals the client to deactivate this monster entity
            _manager.BroadcastMonsterActiveState(NetId, 0, Position, _syncTime);
        }
    }
}
