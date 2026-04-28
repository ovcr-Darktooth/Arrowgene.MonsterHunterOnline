using System;
using System.IO;
using System.Threading;
using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto.Core;
using Arrowgene.MonsterHunterOnline.Service.Data;
using Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree;
using Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree.Handlers;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem
{
    public enum MonsterAIState { Idle, Chase, Attack, Dead }

    public class MonsterAI : IDisposable, IBtMonsterAdapter
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
        private readonly PartBreakComponent _partBreak;
        private readonly StatusEffectComponent _status;

        private Timer _timer;
        private bool _disposed;

        // Sequence State
        private SequenceData _currentSequence;
        private float _sequenceTime;
        private CSVec3 _sequenceStartPos;
        private CSQuat _sequenceStartRot;
        private float _sequenceStartYaw;
        private (float x, float y, float z) _sequenceLocalOrigin;

        // Behavior Tree runtime (Phase 6.5).
        private readonly BtRunner _btRunner;
        private readonly Blackboard _btBlackboard;
        private Client _lastTarget;
        private float _lastTargetDist = float.MaxValue;
        private bool _deathFired;

        /// <summary>
        /// Standalone attack-sequence names the fallback BT picks from (uniformly at random
        /// among those present in the monster's <see cref="SequenceSet"/>). Only "complete"
        /// attacks are listed — Start/Loop/End fragments (e.g. <c>Attack_Leap_F_Start</c> →
        /// <c>Attack_Leap_F_Finish</c>) are skipped because the fallback BT doesn't chain
        /// sequences yet. Names taken from em001skill — add other monsters' candidates as
        /// they get tested.
        /// </summary>
        private static readonly string[] AttackCandidates =
        {
            "DragonDash",
            "Attack_BodyDown_Fast",
            "Attack_Swing_Claw_L_Fast",
            "Attack_Swing_Claw_L_Heavy",
            "Attack_Swing_Claw_R_Fast",
            "Attack_Swing_Claw_R_Heavy",
            "Attack_Throw_Claw_F",
            "Attack_Throw_Tail_L",
            "LeftClaw",
            "RightClaw",
            "BeaverRoll",
            "RotateAttack",
            // Generic monster fallbacks (may exist on other monsters' sets).
            "Head", "Attack", "HeadAttack", "TailAttack",
            "JumpAttack", "HipCheck", "Bite", "BiteAttack", "TurnAttack"
        };
        private static readonly Random _attackRng = new Random();

        public MonsterAI(uint netId, uint renderNetId, int monsterInfoId, CSVec3 spawnPos, MonsterAIManager manager, SequenceManager sequenceManager, int maxHp, PartsTable partsTable)
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

            string refName = manager?.GetAssetId(monsterInfoId) ?? "em001";
            _sequenceSet = _sequenceManager?.GetOrLoad(refName);

            _partBreak = new PartBreakComponent(monsterInfoId, partsTable);
            _status = new StatusEffectComponent(monsterInfoId, partsTable);

            _btBlackboard = new Blackboard();
            _btRunner = BuildBtRunner(refName);

            _timer = new Timer(Tick, null, TickMs, TickMs);
        }

        /// <summary>
        /// Applies a part-targeted hit. Any break tiers crossed by this hit are broadcast
        /// as <c>Hit_PartBroken_&lt;partId&gt;</c> sequences if the animation set defines them.
        /// </summary>
        public void ApplyPartHit(string partId, PartWeaponType weapon, float rawDamage)
        {
            if (_partBreak == null || string.IsNullOrEmpty(partId)) return;
            var broken = _partBreak.ApplyHit(partId, weapon, rawDamage);
            if (broken == null || broken.Count == 0) return;

            string seqName = $"Hit_PartBroken_{partId}";
            bool hasSeq = _sequenceSet != null && _sequenceSet.Sequences.ContainsKey(seqName);
            foreach (var tier in broken)
            {
                Logger.Info($"Monster {NetId} part '{partId}' broke tier {tier.Tier} (DmgVal={tier.DmgVal}) seq={(hasSeq ? seqName : "<none>")}");
                if (hasSeq)
                {
                    CSQuat rot = _sequenceStartRot ?? new CSQuat(1f, 0, 0, 0);
                    BroadcastSequenceState(seqName, 0f, Position, rot);
                }
            }
        }

        /// <summary>
        /// Applies a hit to the part's unbalance buildup. If the stagger threshold is crossed,
        /// broadcasts <c>Hit_Stun_&lt;partId&gt;</c> if the sequence exists in the loaded set.
        /// </summary>
        public void ApplyUnbalance(string partId, float rawDamage)
        {
            if (_status == null || string.IsNullOrEmpty(partId)) return;
            if (!_status.ApplyHit(partId, rawDamage)) return;

            string seqName = $"Hit_Stun_{partId}";
            bool hasSeq = _sequenceSet != null && _sequenceSet.Sequences.ContainsKey(seqName);
            Logger.Info($"Monster {NetId} part '{partId}' staggered (Unbalance threshold crossed) seq={(hasSeq ? seqName : "<none>")}");
            if (hasSeq)
            {
                CSQuat rot = _sequenceStartRot ?? new CSQuat(1f, 0, 0, 0);
                BroadcastSequenceState(seqName, 0f, Position, rot);
            }
        }

        /// <summary>
        /// Adds raw damage to the fall buildup. When the threshold is crossed, broadcasts
        /// <c>Hit_FallDown_&lt;L|R&gt;_Start</c>; the L/R side is chosen so that the monster
        /// falls away from the attacker (hit from monster's right → falls to its left,
        /// exposing the right side — standard MH convention).
        /// </summary>
        /// <param name="hitFromPos">World position of the attacker; used to compute side.
        /// Falls back to a deterministic default (Left) if null.</param>
        public void ApplyFall(float rawDamage, CSVec3 hitFromPos)
        {
            if (_status == null) return;
            if (!_status.ApplyFallHit(rawDamage)) return;

            FallDirection dir = ComputeFallDirection(hitFromPos);
            char side = dir == FallDirection.Left ? 'L' : 'R';
            string startSeq = $"Hit_FallDown_{side}_Start";

            bool hasSeq = _sequenceSet != null && _sequenceSet.Sequences.ContainsKey(startSeq);
            Logger.Info($"Monster {NetId} fell ({dir}) — fall buildup #{_status.Fall.FallCount} seq={(hasSeq ? startSeq : "<none>")}");
            if (hasSeq)
            {
                CSQuat rot = _sequenceStartRot ?? new CSQuat(1f, 0, 0, 0);
                BroadcastSequenceState(startSeq, 0f, Position, rot);
                // TODO Phase 7: chain Loop → End once the BT runtime owns sequence transitions.
            }
        }

        /// <summary>
        /// Adds raw damage to the faint buildup. When the threshold is crossed, broadcasts
        /// the standalone <c>Stun</c> sequence (sustained faint, distinct from per-part stagger).
        /// </summary>
        public void ApplyFaint(float rawDamage)
        {
            if (_status == null) return;
            if (!_status.ApplyFaintHit(rawDamage)) return;

            const string seqName = "Stun";
            bool hasSeq = _sequenceSet != null && _sequenceSet.Sequences.ContainsKey(seqName);
            Logger.Info($"Monster {NetId} fainted — faint buildup #{_status.Faint.FaintCount} seq={(hasSeq ? seqName : "<none>")}");
            if (hasSeq)
            {
                CSQuat rot = _sequenceStartRot ?? new CSQuat(1f, 0, 0, 0);
                BroadcastSequenceState(seqName, 0f, Position, rot);
            }
        }

        /// <summary>
        /// Adds elemental buildup. When the per-element threshold is crossed, broadcasts the
        /// matching <c>Abnormal_&lt;Element&gt;_Start</c> sequence. Element→sequence mapping
        /// follows em001's animation set (Daze for Fire/Electric, Paralysis for Electric variant,
        /// Sleep for Water, Flare for Dragon — placeholder until weapon→element data is wired).
        /// </summary>
        public void ApplyElement(HitElement element, float amount)
        {
            if (_status == null || element == HitElement.None) return;
            if (!_status.ApplyElementHit(element, amount)) return;

            string buffName = ElementToBuffName(element);
            string startSeq = $"Abnormal_{buffName}_Start";

            bool hasSeq = _sequenceSet != null && _sequenceSet.Sequences.ContainsKey(startSeq);
            Logger.Info($"Monster {NetId} suffered abnormal {element} (buildup threshold crossed) seq={(hasSeq ? startSeq : "<none>")}");
            if (hasSeq)
            {
                CSQuat rot = _sequenceStartRot ?? new CSQuat(1f, 0, 0, 0);
                BroadcastSequenceState(startSeq, 0f, Position, rot);
            }
        }

        private static string ElementToBuffName(HitElement element) => element switch
        {
            HitElement.Fire => "Flare",
            HitElement.Water => "Sleep",
            HitElement.Dragon => "Daze",
            HitElement.Electric => "Paralysis",
            HitElement.Ice => "Daze",
            _ => "Daze",
        };

        /// <summary>
        /// Decides which side the monster falls toward, based on attacker position relative to
        /// monster facing (CryEngine: +Y forward, +X right; right-axis = (sin(yaw), -cos(yaw))).
        /// Hit from monster's right → falls Left (exposing right side); else falls Right.
        /// </summary>
        private FallDirection ComputeFallDirection(CSVec3 hitFromPos)
        {
            if (hitFromPos == null) return FallDirection.Left;
            float yaw = _sequenceStartYaw;
            float rx = MathF.Sin(yaw);
            float ry = -MathF.Cos(yaw);
            float dx = hitFromPos.x - Position.x;
            float dy = hitFromPos.y - Position.y;
            float side = dx * rx + dy * ry;
            return side > 0f ? FallDirection.Left : FallDirection.Right;
        }

        /// <summary>
        /// Builds the BT runner for this monster. When the manager has a configured
        /// <see cref="BtTreeLoader"/> AND the master XML for this asset key resolves on disk
        /// (e.g. <c>em001/em001.xml_decrypted.xml</c>), wires the real CryEngine BT through the
        /// generic Phase 6.4 handlers. Otherwise falls back to the programmatic Idle/Chase/
        /// Attack tree from <see cref="MonsterAiFallbackBt"/>. Either path produces a runner
        /// using the same <see cref="_btBlackboard"/>, so HP/Dead sync stays unchanged.
        /// </summary>
        private BtRunner BuildBtRunner(string refName)
        {
            BtTreeLoader loader = _manager?.BtLoader;
            if (loader != null && !string.IsNullOrEmpty(refName))
            {
                string rel = Path.Combine(refName, refName + ".xml");
                try
                {
                    BtTree tree = loader.Resolve(rel, parent: null, out _);
                    if (tree != null)
                    {
                        BtHandlerRegistry registry = new BtHandlerRegistry();
                        BtDefaultHandlers.RegisterAll(registry);
                        BtContext ctx = new BtContext(_btBlackboard, loader, registry) { Owner = this };
                        Logger.Info($"Monster {NetId} loaded master BT '{tree.SourcePath}'");
                        return new BtRunner(tree, ctx);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Monster {NetId} master BT load failed for '{refName}': {ex.Message} — falling back to programmatic BT");
                }
            }
            Logger.Info($"Monster {NetId} using fallback BT (refName='{refName}')");
            return MonsterAiFallbackBt.BuildRunner(this, _btBlackboard);
        }

        /// <summary>
        /// Tick is split in two phases:
        /// (A) advance any locked sequence (root-motion replay + hit events) and short-circuit
        /// while the lock holds — sequence-lock semantics are owned by MonsterAI, not by the BT;
        /// (B) refresh target + sync state into the blackboard, then tick the BT runner. The BT
        /// chooses Idle/Chase/Attack and calls back into <see cref="StartAttack"/> /
        /// <see cref="StepChaseTowardLastTarget"/> / <see cref="GoIdle"/> via custom handlers.
        /// </summary>
        private void Tick(object _)
        {
            if (_disposed) return;

            try
            {
                if (AdvanceLockedSequence()) return;

                var (target, dist) = _manager.FindNearestPlayer(Position);
                _lastTarget = target;
                _lastTargetDist = dist;

                SyncBlackboard();
                _btRunner.Tick(TickMs / 1000f);
            }
            catch (Exception ex)
            {
                Logger.Error($"Monster {NetId} tick error: {ex.Message}");
            }
        }

        /// <summary>Returns true while a sequence is still playing and the BT must NOT tick.</summary>
        private bool AdvanceLockedSequence()
        {
            if (_currentSequence == null) return false;

            _sequenceTime += (TickMs / 1000f);

            if (_sequenceTime >= _currentSequence.TimeRange)
            {
                Logger.Debug($"Monster {NetId} finished sequence {_currentSequence.Name}");
                _currentSequence = null;
                return false;
            }

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

            return true;
        }

        /// <summary>Mirrors authoritative state into the BT blackboard at the start of each BT tick.</summary>
        private void SyncBlackboard()
        {
            _btBlackboard.Set("Health", CurrentHp);
            _btBlackboard.Set("MaxHealth", MaxHp);
            _btBlackboard.Set("Dead", CurrentHp <= 0);
        }

        // --- Internals consumed by FallbackBtHandlers (kept as instance helpers so the
        // legacy Idle/Chase/Attack code stays in one place — the handlers just route to it). ---

        internal Client LastTarget => _lastTarget;
        internal float LastTargetDistance => _lastTargetDist;
        internal bool HasAggroTarget => _lastTarget != null && _lastTargetDist <= AggroRange;

        /// <summary>
        /// Attack range uses 2D (XY) distance — the legacy Tick did the same. _lastTargetDist
        /// is 3D (from FindNearestPlayer) and would gate out attacks whenever the player has
        /// any Z offset relative to the monster (e.g. terrain steps).
        /// </summary>
        internal bool IsTargetInAttackRange
        {
            get
            {
                if (_lastTarget == null) return false;
                CSVec3 targetPos = _lastTarget.State.Position ?? _lastTarget.State.InitSpawnPos;
                if (targetPos == null) return false;
                return Distance2D(Position, targetPos) <= AttackRange;
            }
        }

        /// <summary>Picks an attack sequence, broadcasts it, and locks <c>_currentSequence</c>.</summary>
        internal void StartAttack()
        {
            if (_lastTarget == null) return;
            CSVec3 targetPos = _lastTarget.State.Position ?? _lastTarget.State.InitSpawnPos;
            State = MonsterAIState.Attack;

            CSQuat rot = LookAtQuat(Position, targetPos);
            float yawStart = MathF.Atan2(targetPos.y - Position.y, targetPos.x - Position.x);
            CSVec3 zeroSpeed = new(0, 0, 0);

            string attackSequence = PickRandomAttackSequence();

            SendLocomotion(Position, rot, targetPos, zeroSpeed, attackSequence, 0, true, true);
            SendMovestate(Position, rot, zeroSpeed);
            BroadcastSequenceState(attackSequence, 0f, Position, rot);

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

        /// <summary>
        /// Snaps the monster's facing toward <paramref name="targetPos"/> and broadcasts
        /// a Movestate so the client mirrors the new rotation. Updates
        /// <see cref="_sequenceStartRot"/>/<see cref="_sequenceStartYaw"/> so any sequence
        /// started later inherits the rotated heading. Used by the
        /// <c>EntityRotateToTarget</c> BT handler — instant rotate (no smoothing yet).
        /// </summary>
        internal void RotateInstantTo(CSVec3 targetPos)
        {
            if (targetPos == null) return;
            CSQuat rot = LookAtQuat(Position, targetPos);
            float yaw = MathF.Atan2(targetPos.y - Position.y, targetPos.x - Position.x);
            _sequenceStartRot = rot;
            _sequenceStartYaw = yaw;
            SendMovestate(Position, rot, new CSVec3(0, 0, 0));
        }

        /// <summary>Advances the chase one tick toward the cached last target.</summary>
        internal void StepChaseTowardLastTarget()
        {
            if (_lastTarget == null) return;
            CSVec3 targetPos = _lastTarget.State.Position ?? _lastTarget.State.InitSpawnPos;
            State = MonsterAIState.Chase;

            float tickSeconds = TickMs / 1000f;
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

        /// <summary>
        /// Picks one of the loaded attack sequences uniformly at random. Filters
        /// <see cref="AttackCandidates"/> against the monster's <see cref="SequenceSet"/>
        /// so only sequences that actually exist are considered. Falls back to "Head" if
        /// no candidates match (legacy behaviour).
        /// </summary>
        private string PickRandomAttackSequence()
        {
            if (_sequenceSet == null) return "Head";

            string pick = null;
            int seen = 0;
            foreach (string candidate in AttackCandidates)
            {
                if (!_sequenceSet.Sequences.ContainsKey(candidate)) continue;
                seen++;
                // Reservoir sampling, n=1: each candidate has 1/seen chance to replace
                // the current pick — uniform across whichever entries actually match.
                if (_attackRng.Next(seen) == 0) pick = candidate;
            }
            return pick ?? "Head";
        }

        /// <summary>Transitions to Idle and broadcasts MonsterActiveState=1 once on entry.</summary>
        internal void GoIdle()
        {
            if (State != MonsterAIState.Idle)
            {
                State = MonsterAIState.Idle;
                Logger.Debug($"Monster {NetId} -> Idle");
                _manager.BroadcastMonsterActiveState(NetId, 1, Position, CurrentSyncTimeMs());
            }
        }

        // --- IBtMonsterAdapter ---

        public bool IsAlive => !_disposed && CurrentHp > 0;
        public bool IsSequencePlaying() => _currentSequence != null;
        public bool IsSequencePlaying(string sequenceName)
            => _currentSequence != null && _currentSequence.Name == sequenceName;

        /// <summary>
        /// Generic adapter entry point used by the Phase 6.4 <c>AnimSequencePlay</c> handler
        /// (real em001 BT). The fallback BT goes through <see cref="StartAttack"/> instead and
        /// never reaches this. Returns false if the sequence is unknown or another sequence is
        /// already locked.
        /// </summary>
        public bool TryStartSequence(string sequenceName)
        {
            if (string.IsNullOrEmpty(sequenceName)) return false;
            if (_currentSequence != null) return false;
            if (_sequenceSet == null || !_sequenceSet.Sequences.TryGetValue(sequenceName, out var seq))
                return false;

            CSQuat rot = _sequenceStartRot ?? new CSQuat(1f, 0, 0, 0);
            CSVec3 zeroSpeed = new(0, 0, 0);
            SendLocomotion(Position, rot, Position, zeroSpeed, sequenceName, 0, true, true);
            SendMovestate(Position, rot, zeroSpeed);
            BroadcastSequenceState(sequenceName, 0f, Position, rot);

            _sequenceStartPos = CloneVec(Position);
            _sequenceStartRot = rot;
            _sequenceStartYaw = 0f;
            _currentSequence = seq;
            _sequenceTime = 0f;
            _sequenceLocalOrigin = seq.Position.HasAny ? seq.Position.Sample(0f) : (0f, 0f, 0f);
            return true;
        }

        /// <summary>Adapter death entry point — idempotent; first call delegates to <see cref="makeDie"/>.</summary>
        public void HandleDeath()
        {
            if (_deathFired) return;
            makeDie();
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

        public void makeDie()
        {
            if (_deathFired) return;
            _deathFired = true;
            State = MonsterAIState.Dead;

            CSQuat rot = new CSQuat();
            CSVec3 zeroSpeed = new(0, 0, 0);
            SendLocomotion(Position, rot, Position, zeroSpeed, "Die", 0, true, true);
            SendMovestate(Position, rot, zeroSpeed);
            BroadcastSequenceState("Die", 0f, Position, rot);
            //send packet CS_CMD_INSTANCE_FINISH_RSP
            try
            {
                var finish = new CSInstanceFinishRsp
                {
                    ShowFlag = 1, // enter countdown flow
                    CountDownSeconds = 5, // start a 60 seconds timer
                    WinFlag = 1 // mark as win (1 = win)
                };
                _manager.BroadcastInstanceFinish(finish);
            }
            catch (Exception ex)
            {
                Logger.Error($"Monster {NetId} failed to broadcast instance finish: {ex.Message}");
            }
            Dispose();
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

        /// <summary>
        /// Builds a yaw-only quaternion that rotates the model's intrinsic forward (+Y in
        /// CryEngine character space) to the world-space direction (to - from). Subtracting
        /// π/2 from atan2(dy, dx) compensates for the +Y-forward convention — without it,
        /// the encoded rotation aligns model-+Y with world-+X and the monster appears 90°
        /// off from its chase target.
        /// </summary>
        private static CSQuat LookAtQuat(CSVec3 from, CSVec3 to)
        {
            float deltaX = to.x - from.x;
            float deltaY = to.y - from.y;

            if (MathF.Abs(deltaX) < 0.001f && MathF.Abs(deltaY) < 0.001f)
            {
                return new CSQuat(1.0f, 0, 0, 0);
            }

            return YawQuat(MathF.Atan2(deltaY, deltaX) - MathF.PI / 2f);
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
