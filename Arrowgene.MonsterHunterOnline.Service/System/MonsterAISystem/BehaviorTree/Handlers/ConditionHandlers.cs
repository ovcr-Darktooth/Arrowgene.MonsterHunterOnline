using System;
using System.Globalization;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree.Handlers
{
    /// <summary>
    /// <c>BlackBoardCheck KeyName="X" Value="Y" [Operator="=="]</c> — true when the
    /// blackboard variable named X equals (or relates per Operator to) Y. CryEngine
    /// stores Value as a string regardless of type; we compare via the BB's
    /// <see cref="Blackboard.GetAsString"/> normaliser.
    ///
    /// Operators observed: <c>==</c> (default), <c>!=</c>, <c>&gt;</c>, <c>&gt;=</c>,
    /// <c>&lt;</c>, <c>&lt;=</c>. Numeric comparisons use the var's float view; string
    /// comparisons fall back to ordinal equality.
    /// </summary>
    public sealed class BlackBoardCheckHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            string key = node.GetAttr("KeyName");
            string expected = node.GetAttr("Value");
            string op = node.GetAttr("Operator") ?? "==";
            if (string.IsNullOrEmpty(key)) return BtStatus.Failure;

            return Compare(ctx.Blackboard, key, expected, op) ? BtStatus.Success : BtStatus.Failure;
        }

        internal static bool Compare(Blackboard bb, string key, string expected, string op)
        {
            string actual = bb.GetAsString(key);
            string type = bb.TypeOf(key);

            // Numeric path for Int/Uint32/Float types — also when the value parses as float
            // (CryEngine sometimes leaves Type unset on derived BBs).
            bool numeric = type == "Float" || type == "Int" || type == "Uint32";
            if (!numeric) numeric = float.TryParse(actual, NumberStyles.Float, CultureInfo.InvariantCulture, out _)
                                 && float.TryParse(expected, NumberStyles.Float, CultureInfo.InvariantCulture, out _);

            if (numeric
                && float.TryParse(actual, NumberStyles.Float, CultureInfo.InvariantCulture, out float a)
                && float.TryParse(expected, NumberStyles.Float, CultureInfo.InvariantCulture, out float e))
            {
                return op switch
                {
                    "==" => Math.Abs(a - e) < 1e-5f,
                    "!=" => Math.Abs(a - e) >= 1e-5f,
                    ">" => a > e,
                    ">=" => a >= e,
                    "<" => a < e,
                    "<=" => a <= e,
                    _ => false
                };
            }

            // String / bool path.
            return op switch
            {
                "==" => string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
                "!=" => !string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }
    }

    /// <summary>
    /// <c>CheckHealth Percentage="0.1" OperationChar="&lt;"</c> — true when
    /// <c>Health / MaxHealth</c> compares to <c>Percentage</c> via OperationChar.
    /// Both Health and MaxHealth come from the blackboard. Used by em001's Escape
    /// branch (HP &lt; 10%).
    /// </summary>
    public sealed class CheckHealthHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            string opChar = node.GetAttr("OperationChar") ?? "<";
            string pctRaw = node.GetAttr("Percentage");
            if (!float.TryParse(pctRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out float pct))
                return BtStatus.Failure;

            int hp = ctx.Blackboard.GetInt("Health");
            int max = ctx.Blackboard.GetInt("MaxHealth");
            if (max <= 0) return BtStatus.Failure;

            float ratio = (float)hp / max;
            bool ok = opChar switch
            {
                "<" => ratio < pct,
                "<=" => ratio <= pct,
                ">" => ratio > pct,
                ">=" => ratio >= pct,
                "==" => Math.Abs(ratio - pct) < 1e-5f,
                _ => false
            };
            return ok ? BtStatus.Success : BtStatus.Failure;
        }
    }

    /// <summary><c>AlwaysTrue</c> — unconditional Success.</summary>
    public sealed class AlwaysTrueHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx) => BtStatus.Success;
    }

    /// <summary>
    /// <c>AnimSequenceIsPlaying [SequenceName="X"]</c> — querying the monster adapter.
    /// In em001's death branch this is wrapped in a <c>Filter Until_Fails</c> that
    /// loops while the death animation plays. With no adapter, returns Failure so the
    /// loop terminates immediately — safe default.
    /// </summary>
    public sealed class AnimSequenceIsPlayingHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not IBtMonsterAdapter mon) return BtStatus.Failure;

            string name = node.GetAttr("SequenceName");
            bool playing = string.IsNullOrEmpty(name) ? mon.IsSequencePlaying() : mon.IsSequencePlaying(name);
            return playing ? BtStatus.Success : BtStatus.Failure;
        }
    }

    /// <summary>
    /// <c>TimeCheck KeyName="RecordedTime" Value="3.5" OperationChar="&gt;"</c> —
    /// compare elapsed time since a recorded timestamp (set by <see cref="SetTimeHandler"/>)
    /// against a literal threshold. <c>now - recorded {OperationChar} threshold</c>.
    /// </summary>
    public sealed class TimeCheckHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            string key = node.GetAttr("KeyName") ?? node.GetAttr("RecordTimeBBName");
            string opChar = node.GetAttr("OperationChar") ?? ">";
            string raw = node.GetAttr("Value");
            // Empty key → implicit time slot shared with the matching SetTime in this sub-tree.
            if (string.IsNullOrEmpty(key)) key = SetTimeHandler.ImplicitTimeKey;
            if (!float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float threshold))
                return BtStatus.Failure;

            float recorded = ctx.Blackboard.GetFloat(key);
            float elapsed = ctx.TotalSeconds - recorded;
            bool ok = opChar switch
            {
                ">" => elapsed > threshold,
                ">=" => elapsed >= threshold,
                "<" => elapsed < threshold,
                "<=" => elapsed <= threshold,
                _ => false
            };
            return ok ? BtStatus.Success : BtStatus.Failure;
        }
    }
}
