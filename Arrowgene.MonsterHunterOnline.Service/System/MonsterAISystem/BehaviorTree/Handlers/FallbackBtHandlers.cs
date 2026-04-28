namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree.Handlers
{
    /// <summary>
    /// Custom handlers for the programmatic fallback BT used when a monster has no
    /// dedicated CryEngine BT XML wired up (or its Operations aren't yet implemented).
    /// They reproduce the legacy <c>MonsterAI.Tick</c> Idle/Chase/Attack state machine
    /// but routed through the BT runtime, so Phase 6.5 ships an integration with zero
    /// behavioural regression vs. the pre-BT code.
    ///
    /// Coupled to <see cref="MonsterAI"/> on purpose — these are the *fallback*; the
    /// real em001 master BT will instead use the generic Phase 6.4 handlers
    /// (BlackBoardCheck / AnimSequencePlay / etc.) once Phase 6.6 wires the missing
    /// real implementations.
    /// </summary>
    public static class FallbackBtHandlers
    {
        public const string OpHasTarget = "HasTarget";
        public const string OpInAttackRange = "InAttackRange";
        public const string OpPerformAttack = "PerformAttack";
        public const string OpMoveTowardTarget = "MoveTowardTarget";
        public const string OpGoIdle = "GoIdle";

        public static void RegisterAll(BtHandlerRegistry registry)
        {
            registry.RegisterCondition(OpHasTarget, new HasTargetHandler());
            registry.RegisterCondition(OpInAttackRange, new InAttackRangeHandler());
            registry.RegisterAction(OpPerformAttack, new PerformAttackHandler());
            registry.RegisterAction(OpMoveTowardTarget, new MoveTowardTargetHandler());
            registry.RegisterAction(OpGoIdle, new GoIdleHandler());
        }
    }

    /// <summary>Success when the monster has a player within aggro range, else Failure.</summary>
    public sealed class HasTargetHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not MonsterAI mon) return BtStatus.Failure;
            return mon.HasAggroTarget ? BtStatus.Success : BtStatus.Failure;
        }
    }

    /// <summary>Success when the cached target distance is within attack range.</summary>
    public sealed class InAttackRangeHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not MonsterAI mon) return BtStatus.Failure;
            return mon.IsTargetInAttackRange ? BtStatus.Success : BtStatus.Failure;
        }
    }

    /// <summary>
    /// Launches the monster's attack sequence (broadcasts LCM/Movestate/SequenceState
    /// and locks <c>_currentSequence</c>). Returns Success — the next tick's Phase A
    /// sequence-advance prevents the BT from re-firing until the lock clears.
    /// </summary>
    public sealed class PerformAttackHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not MonsterAI mon) return BtStatus.Failure;
            mon.StartAttack();
            return BtStatus.Success;
        }
    }

    /// <summary>Advances chase by one tick (StepToward + LCM/Movestate broadcast).</summary>
    public sealed class MoveTowardTargetHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not MonsterAI mon) return BtStatus.Failure;
            mon.StepChaseTowardLastTarget();
            return BtStatus.Success;
        }
    }

    /// <summary>Catch-all: marks the monster Idle and broadcasts MonsterActiveState=1 once.</summary>
    public sealed class GoIdleHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx)
        {
            if (ctx.Owner is not MonsterAI mon) return BtStatus.Failure;
            mon.GoIdle();
            return BtStatus.Success;
        }
    }
}
