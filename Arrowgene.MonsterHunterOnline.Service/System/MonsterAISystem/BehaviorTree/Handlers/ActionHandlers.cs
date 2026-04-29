using System;
using System.Globalization;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree.Handlers
{
    /// <summary>
    /// <c>SetBlackBoard KeyName="X" Value="Y"</c> — write the blackboard variable. The
    /// value is coerced through the var's declared type (so <c>Value="Idle"</c> stays
    /// a string, <c>Value="True"</c> becomes a bool, <c>Value="0.1"</c> becomes a
    /// float, etc.).
    /// <para>
    /// Vec3/Quat keys use the multi-attribute form <c>Value1/Value2/Value3[/Value4]</c>
    /// — see e.g. <c>em001dashtoplayer.xml</c> setting <c>MoveSpeed</c> to
    /// <c>Value1="0" Value2="11.1" Value3="0"</c> (local-frame forward velocity). When
    /// <c>Value1</c> is present we compose a comma-separated string and let the
    /// blackboard's Vec3 storage keep it raw.
    /// </para>
    /// </summary>
    public sealed class SetBlackBoardHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            string key = node.GetAttr("KeyName");
            if (string.IsNullOrEmpty(key)) return BtStatus.Failure;

            string v1 = node.GetAttr("Value1");
            if (!string.IsNullOrEmpty(v1))
            {
                string v2 = node.GetAttr("Value2") ?? "0";
                string v3 = node.GetAttr("Value3") ?? "0";
                string v4 = node.GetAttr("Value4");
                string composed = v4 != null ? $"{v1},{v2},{v3},{v4}" : $"{v1},{v2},{v3}";
                ctx.Blackboard.SetFromString(key, composed);
                return BtStatus.Success;
            }

            ctx.Blackboard.SetFromString(key, node.GetAttr("Value"));
            return BtStatus.Success;
        }
    }

    /// <summary>
    /// <c>SetBlackBoardEqualString/Float/Int KeyName1="dst" KeyName2="src"</c> — typed copy
    /// <c>BB[dst] = BB[src]</c>. The em001 BT uses these to snapshot state across ticks, e.g.
    /// <c>SetLastState: LastState = CurState</c> in <c>em001setstate.xml</c>.
    /// </summary>
    public sealed class SetBlackBoardEqualHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            string dst = node.GetAttr("KeyName1");
            string src = node.GetAttr("KeyName2");
            if (string.IsNullOrEmpty(dst) || string.IsNullOrEmpty(src)) return BtStatus.Failure;

            object value = ctx.Blackboard.GetRaw(src);
            if (value == null) return BtStatus.Failure;

            ctx.Blackboard.Set(dst, value);
            return BtStatus.Success;
        }
    }

    /// <summary>
    /// <c>SetBlackBoardBBOPC KeyName="dst" KeyNameOP1="lhs" OP="+|-|*|/" Value="N"</c> — scalar
    /// arithmetic <c>BB[dst] = BB[lhs] OP N</c>. em001 uses this for counters/accumulators
    /// (<c>AttackPeriod += 1</c>, <c>EatPeriod += 1</c>, <c>TempFloat = MaxHealth * 0.35</c>).
    /// Without this, every per-tick "bump the cooldown" stays at 0 and gating conditions like
    /// <c>AttackPeriod &gt; N</c> never become true — the BT loops in idle/rotate forever.
    /// </summary>
    public sealed class SetBlackBoardBbopcHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            string dst = node.GetAttr("KeyName");
            string lhs = node.GetAttr("KeyNameOP1");
            string op = node.GetAttr("OP");
            string rhsRaw = node.GetAttr("Value");
            if (string.IsNullOrEmpty(dst) || string.IsNullOrEmpty(lhs) || string.IsNullOrEmpty(op)) return BtStatus.Failure;
            if (!float.TryParse(rhsRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out float rhs))
                return BtStatus.Failure;

            float a = ToFloat(ctx.Blackboard.GetRaw(lhs));
            float result = ApplyOp(a, op, rhs);
            StoreNumeric(ctx, dst, result);
            return BtStatus.Success;
        }

        internal static float ToFloat(object v) => v switch
        {
            float f => f,
            int i => i,
            uint u => u,
            string s when float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float p) => p,
            _ => 0f
        };

        internal static float ApplyOp(float a, string op, float b) => op switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" => b != 0f ? a / b : 0f,
            _ => 0f
        };

        internal static void StoreNumeric(BtContext ctx, string key, float value)
        {
            // Match the declared type so downstream BlackBoardCheck comparisons line up.
            string type = ctx.Blackboard.TypeOf(key);
            object stored = type switch
            {
                "Int" => (object)(int)value,
                "Uint32" => (object)(uint)MathF.Max(0f, value),
                _ => value, // Float and unknown both kept as float
            };
            ctx.Blackboard.Set(key, stored);
        }
    }

    /// <summary>
    /// <c>SetBlackBoardBBOPBB KeyName="dst" KeyNameOP1="lhs" KeyNameOP2="rhs" OP="+|-|*|/"</c>
    /// — same as BBOPC but the right-hand operand is another BB key. em001 uses it to subtract
    /// damage from health (<c>Health = Health - HitDamageHealth</c>) and to accumulate part
    /// damage (<c>HeadDamageSum += HitDamageHealth</c>).
    /// </summary>
    public sealed class SetBlackBoardBbopbbHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            string dst = node.GetAttr("KeyName");
            string lhs = node.GetAttr("KeyNameOP1");
            string rhs = node.GetAttr("KeyNameOP2");
            string op = node.GetAttr("OP");
            if (string.IsNullOrEmpty(dst) || string.IsNullOrEmpty(lhs) || string.IsNullOrEmpty(rhs) || string.IsNullOrEmpty(op))
                return BtStatus.Failure;

            float a = SetBlackBoardBbopcHandler.ToFloat(ctx.Blackboard.GetRaw(lhs));
            float b = SetBlackBoardBbopcHandler.ToFloat(ctx.Blackboard.GetRaw(rhs));
            float result = SetBlackBoardBbopcHandler.ApplyOp(a, op, b);
            SetBlackBoardBbopcHandler.StoreNumeric(ctx, dst, result);
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

            // CryEngine BTs use AnimSequence="X" on AnimSequencePlay actions (e.g. em001's
            // DragonDashCancel, FindPlayer, Threaten03). The legacy SequenceName attr is kept
            // as a fallback for any XML that uses it; node.Name is the last-ditch source — the
            // parser captures it on the node itself, not in Attributes.
            string name = node.GetAttr("AnimSequence")
                          ?? node.GetAttr("SequenceName")
                          ?? node.Name;
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
