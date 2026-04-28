using System.Globalization;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree.Handlers
{
    /// <summary>
    /// <c>SetBlackBoard KeyName="X" Value="Y"</c> — write the blackboard variable. The
    /// value is coerced through the var's declared type (so <c>Value="Idle"</c> stays
    /// a string, <c>Value="True"</c> becomes a bool, <c>Value="0.1"</c> becomes a
    /// float, etc.).
    /// </summary>
    public sealed class SetBlackBoardHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            string key = node.GetAttr("KeyName");
            string value = node.GetAttr("Value");
            if (string.IsNullOrEmpty(key)) return BtStatus.Failure;
            ctx.Blackboard.SetFromString(key, value);
            return BtStatus.Success;
        }
    }

    /// <summary>
    /// <c>SetTime RecordTimeBBName="X"</c> — stamp the current accumulated game-time
    /// (<see cref="BtContext.TotalSeconds"/>) into the named blackboard float so a
    /// later <see cref="TimeCheckHandler"/> can compute elapsed time.
    /// </summary>
    public sealed class SetTimeHandler : IBtHandler
    {
        // Implicit time slot used when SetTime / TimeCheck nodes carry no key attribute.
        // em001rotatetoplayer pairs `<SetTime/>` with `<TimeCheck Value="1.5"/>` (both
        // attribute-less); they're meant to share a per-sub-tree default slot.
        internal const string ImplicitTimeKey = "__BtImplicitTime";

        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            string key = node.GetAttr("RecordTimeBBName") ?? node.GetAttr("KeyName");
            if (string.IsNullOrEmpty(key)) key = ImplicitTimeKey;
            ctx.Blackboard.Set(key, ctx.TotalSeconds);
            return BtStatus.Success;
        }
    }

    /// <summary>
    /// <c>DelayTime Value="N"</c> (seconds) — returns Running for N seconds, then Success.
    /// First entry stamps a start time into per-node state; subsequent ticks compare
    /// against <see cref="BtContext.TotalSeconds"/>.
    /// </summary>
    public sealed class DelayTimeHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            string raw = node.GetAttr("Value") ?? node.GetAttr("Time");
            if (!float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float seconds))
                return BtStatus.Success; // no delay configured → instant pass

            DelayState state = ctx.GetOrCreateState<DelayState>(node);
            if (!state.Started)
            {
                state.StartedAt = ctx.TotalSeconds;
                state.Started = true;
            }

            if (ctx.TotalSeconds - state.StartedAt >= seconds)
            {
                ctx.ResetState(node);
                return BtStatus.Success;
            }
            return BtStatus.Running;
        }

        private sealed class DelayState
        {
            public bool Started;
            public float StartedAt;
        }
    }

    /// <summary>
    /// <c>AnimSequencePlay SequenceName="X"</c> — start the named sequence on the
    /// monster. Returns Running while the sequence plays, Success once finished,
    /// Failure if the adapter is missing or the sequence couldn't start.
    /// </summary>
    public sealed class AnimSequencePlayHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not IBtMonsterAdapter mon) return BtStatus.Failure;

            string name = node.GetAttr("SequenceName") ?? node.GetAttr("Name");
            if (string.IsNullOrEmpty(name)) return BtStatus.Failure;

            PlayState state = ctx.GetOrCreateState<PlayState>(node);
            if (!state.Started)
            {
                if (!mon.TryStartSequence(name))
                {
                    ctx.ResetState(node);
                    return BtStatus.Failure;
                }
                state.Started = true;
                state.SequenceName = name;
                return BtStatus.Running;
            }

            if (mon.IsSequencePlaying(state.SequenceName)) return BtStatus.Running;
            ctx.ResetState(node);
            return BtStatus.Success;
        }

        private sealed class PlayState
        {
            public bool Started;
            public string SequenceName;
        }
    }

    /// <summary>
    /// <c>HandleDeath</c> — fires the monster's death cleanup once. Subsequent ticks
    /// short-circuit to Success without re-firing.
    /// </summary>
    public sealed class HandleDeathHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            DeathState state = ctx.GetOrCreateState<DeathState>(node);
            if (state.Fired) return BtStatus.Success;

            if (ctx.Owner is IBtMonsterAdapter mon) mon.HandleDeath();
            ctx.Blackboard.SetFromString("Dead", "True");
            state.Fired = true;
            return BtStatus.Success;
        }

        private sealed class DeathState
        {
            public bool Fired;
        }
    }
}
