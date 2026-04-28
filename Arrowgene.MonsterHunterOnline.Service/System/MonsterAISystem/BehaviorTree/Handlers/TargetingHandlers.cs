using System.Globalization;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree.Handlers
{
    /// <summary>
    /// Phase 6.6.3 targeting / locomotion handlers — replace the four most critical
    /// stubs in <see cref="BtDefaultHandlers"/> so the em001 master BT actually faces,
    /// chases, and reaches its target instead of just walking the tree no-op.
    ///
    /// Coupled to <see cref="MonsterAI"/> directly (not through
    /// <see cref="IBtMonsterAdapter"/>) because they need the monster's authoritative
    /// position, target cache, and movement helpers — extending the adapter for a
    /// dozen properties would obscure intent. Tests that pass a non-MonsterAI owner
    /// (e.g. <c>FakeMonster</c>) will see Failure, which is the safe default.
    /// </summary>
    public static class TargetingHandlers
    {
        public static void RegisterAll(BtHandlerRegistry registry)
        {
            registry.RegisterAction("SetTarget", new SetTargetHandler());
            registry.RegisterAction("CopyTargetPropertyToBB", new CopyTargetPropertyToBBHandler());
            registry.RegisterAction("EntityRotateToTarget", new EntityRotateToTargetHandler());
            registry.RegisterAction("EntityMoveToTarget", new EntityMoveToTargetHandler());
            registry.RegisterCondition("DistanceCheck", new DistanceCheckHandler());
        }
    }

    /// <summary>
    /// <c>DistanceCheck OperationChar="&lt;" Value="6"</c> — true when the distance
    /// to the current target compares to <c>Value</c> via OperationChar. Gates the
    /// per-attack range branches in em001Attack* (Level*Long, Level*Short, etc.) —
    /// without it, every range-gated attack returns Failure and the monster never
    /// engages despite SetTarget succeeding.
    /// </summary>
    public sealed class DistanceCheckHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not MonsterAI mon) return BtStatus.Failure;
            if (mon.LastTarget?.State?.Position == null) return BtStatus.Failure;

            string opChar = node.GetAttr("OperationChar") ?? "<";
            string raw = node.GetAttr("Value");
            if (!float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float threshold))
                return BtStatus.Failure;

            float dist = mon.LastTargetDistance;
            bool ok = opChar switch
            {
                "<" => dist < threshold,
                "<=" => dist <= threshold,
                ">" => dist > threshold,
                ">=" => dist >= threshold,
                "==" => global::System.Math.Abs(dist - threshold) < 1e-3f,
                _ => false
            };
            return ok ? BtStatus.Success : BtStatus.Failure;
        }
    }

    /// <summary>
    /// <c>SetTarget Value="N"</c> — picks the nearest live player and stamps their
    /// world position into BB key <c>TargetPos</c> so subsequent Copy/Rotate/Move
    /// handlers can read it. Returns Failure when no player is in range.
    ///
    /// The XML <c>Value</c> attribute is treated as a max sense radius; observed em001
    /// values are 10 / 15 / 45 / 80 (small / medium / aggro). When parsing fails or
    /// Value is absent we use <see cref="MonsterAI.LastTargetDistance"/> ungated, so a
    /// missing attribute still picks the cached target.
    /// </summary>
    public sealed class SetTargetHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not MonsterAI mon) return BtStatus.Failure;
            Client target = mon.LastTarget;
            if (target?.State?.Position == null) return BtStatus.Failure;

            string raw = node.GetAttr("Value");
            if (!string.IsNullOrEmpty(raw)
                && float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float radius)
                && mon.LastTargetDistance > radius)
            {
                return BtStatus.Failure;
            }

            CSVec3 p = target.State.Position;
            ctx.Blackboard.Set("TargetPos",
                $"{p.x.ToString(CultureInfo.InvariantCulture)},{p.y.ToString(CultureInfo.InvariantCulture)},{p.z.ToString(CultureInfo.InvariantCulture)}");
            return BtStatus.Success;
        }
    }

    /// <summary>
    /// <c>CopyTargetPropertyToBB PropertyName="X" TargetName="Y"</c> — copies a property
    /// of the current target into BB key <c>Y</c>. Only the player-targeted properties
    /// the chase/attack flow needs are mapped; non-player property names (e.g.
    /// <c>StateChangeType</c>, <c>RelativePath</c>, <c>teleportPos</c> — used on
    /// FindLogicPoint output) return Failure so the parent branch bails out instead of
    /// writing a garbage value into BB.
    /// </summary>
    public sealed class CopyTargetPropertyToBBHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not MonsterAI mon) return BtStatus.Failure;
            Client target = mon.LastTarget;
            if (target?.State?.Position == null) return BtStatus.Failure;

            string prop = node.GetAttr("PropertyName");
            string bbKey = node.GetAttr("TargetName");
            if (string.IsNullOrEmpty(prop) || string.IsNullOrEmpty(bbKey)) return BtStatus.Failure;

            CSVec3 p = target.State.Position;
            switch (prop)
            {
                case "Position":
                case "Pos":
                case "TargetPos":
                    ctx.Blackboard.Set(bbKey,
                        $"{p.x.ToString(CultureInfo.InvariantCulture)},{p.y.ToString(CultureInfo.InvariantCulture)},{p.z.ToString(CultureInfo.InvariantCulture)}");
                    return BtStatus.Success;
                default:
                    return BtStatus.Failure;
            }
        }
    }

    /// <summary>
    /// <c>EntityRotateToTarget</c> — yaw the monster toward its current target and
    /// broadcast Movestate. v1 is an instant rotation (no smooth interpolation); the
    /// em001 sub-trees handle large-arc rotations through dedicated rotate-anim
    /// sequences anyway, so the base op only needs to commit the new heading.
    /// </summary>
    public sealed class EntityRotateToTargetHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not MonsterAI mon) return BtStatus.Failure;
            Client target = mon.LastTarget;
            CSVec3 targetPos = target?.State?.Position;
            if (targetPos == null) return BtStatus.Failure;

            mon.RotateInstantTo(targetPos);
            return BtStatus.Success;
        }
    }

    /// <summary>
    /// <c>EntityMoveToTarget</c> — step toward the current target. Returns Running
    /// while distance &gt; attack range (one chase step per tick via
    /// <see cref="MonsterAI.StepChaseTowardLastTarget"/>), Success once in range so
    /// the parent Sequence can fall through to the attack node.
    /// </summary>
    public sealed class EntityMoveToTargetHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not MonsterAI mon) return BtStatus.Failure;
            if (mon.LastTarget?.State?.Position == null) return BtStatus.Failure;

            if (mon.IsTargetInAttackRange) return BtStatus.Success;

            mon.StepChaseTowardLastTarget();
            return BtStatus.Running;
        }
    }
}
