namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree.Handlers
{
    /// <summary>
    /// One-stop registration for the handler set Phase 6.4 ships. Real handlers cover
    /// the BB/timing/sequence/death core; everything else em001 references is wired
    /// to a no-op <see cref="StubSuccessHandler"/> (Actions) or
    /// <see cref="StubFailureHandler"/> (Conditions) so the runner walks the full
    /// tree without crashing while leaving Phase 6.6 / future work clearly visible
    /// via <see cref="BtRunner.MissingOperations"/> (will be empty after this) and
    /// the BB state changes never happening.
    ///
    /// **Stub semantics:**
    /// <list type="bullet">
    /// <item>Action stubs return Success — the parent Sequence keeps progressing as
    /// if the action ran.</item>
    /// <item>Condition stubs return Failure — the branch bails out instead of
    /// accidentally activating.</item>
    /// </list>
    /// </summary>
    public static class BtDefaultHandlers
    {
        public static void RegisterAll(BtHandlerRegistry registry)
        {
            RegisterCore(registry);
            TargetingHandlers.RegisterAll(registry);
            RegisterStubs(registry);
        }

        public static void RegisterCore(BtHandlerRegistry registry)
        {
            // Conditions with real logic.
            registry.RegisterCondition("BlackBoardCheck", new BlackBoardCheckHandler());
            registry.RegisterCondition("CheckHealth", new CheckHealthHandler());
            registry.RegisterCondition("TimeCheck", new TimeCheckHandler());

            // AlwaysTrue is used as both Condition and Action in em001 sub-trees.
            var alwaysTrue = new AlwaysTrueHandler();
            registry.RegisterCondition("AlwaysTrue", alwaysTrue);
            registry.RegisterAction("AlwaysTrue", alwaysTrue);

            // Actions with real logic.
            registry.RegisterAction("SetBlackBoard", new SetBlackBoardHandler());
            registry.RegisterAction("SetTime", new SetTimeHandler());
            registry.RegisterAction("DelayTime", new DelayTimeHandler());
            registry.RegisterAction("AnimSequencePlay", new AnimSequencePlayHandler());
            registry.RegisterAction("HandleDeath", new HandleDeathHandler());

            // AnimSequenceIsPlaying is typed as Action in the XML (em001 death branch)
            // but logically a Condition — register on both sides to be safe.
            var animPlaying = new AnimSequenceIsPlayingHandler();
            registry.RegisterAction("AnimSequenceIsPlaying", animPlaying);
            registry.RegisterCondition("AnimSequenceIsPlaying", animPlaying);
        }

        public static void RegisterStubs(BtHandlerRegistry registry)
        {
            var ok = new StubSuccessHandler();
            var no = new StubFailureHandler();

            // Stub Conditions — return Failure so branches don't accidentally fire.
            foreach (string op in StubConditions)
            {
                if (!registry.HasCondition(op)) registry.RegisterCondition(op, no);
            }

            // Stub Actions — return Success so Sequences keep walking.
            foreach (string op in StubActions)
            {
                if (!registry.HasAction(op)) registry.RegisterAction(op, ok);
            }
        }

        // Conditions referenced in em001 sub-trees we don't yet implement.
        private static readonly string[] StubConditions =
        {
            "BlackBoardCheckBBOPBB",
            "DistanceCheck",
            "IsInTheAir",
        };

        // Actions referenced in em001 sub-trees we don't yet implement.
        // These mostly touch targeting/locomotion/effects/spawning — Phase 6.5/6.6
        // will replace the most important ones with real implementations.
        private static readonly string[] StubActions =
        {
            "AnimSequenceSetInput",
            "BBValueCopy",
            "BTOperation",
            "CaculateDamagedState",
            "CaculateDamagedValue",
            "CalculateLocalPointInWorld",
            "CallScriptFunc",
            // CopyTargetPropertyToBB — Phase 6.6.3 (TargetingHandlers)
            "DropEntity",
            "EntityMove",
            "EntityMoveToPos",
            // EntityMoveToTarget — Phase 6.6.3 (TargetingHandlers)
            "EntityPlayAnimation",
            "EntityRotateOrientation",
            "EntityRotateToPos",
            // EntityRotateToTarget — Phase 6.6.3 (TargetingHandlers)
            "FindLogicPoint",
            "FmodMusicCues",
            "GetTargetPosByID",
            "HideAttachment",
            "LoadModel",
            "PickUpEntity",
            "RandomSelectLevelTarget",
            "RemoveEffect",
            "RemoveTriggerProxy",
            "SendMsgToClass",
            "SendStateToClient",
            "SetArea",
            "SetBlackBoardBBOPBB",
            "SetBlackBoardBBOPC",
            "SetBlackBoardEqualFloat",
            "SetBlackBoardEqualInt",
            "SetBlackBoardEqualString",
            "SetPathLength",
            "SetPathPointByIndex",
            // SetTarget — Phase 6.6.3 (TargetingHandlers)
            "SetTargetIDOrientation",
            "SetTargetPosOrientation",
            "SpawnCollectTrig",
            "SpawnEffect",
            "SpawnEntitySimple",
            "TeleportToTargetPos",
            "void",
        };
    }

    public sealed class StubSuccessHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx) => BtStatus.Success;
    }

    public sealed class StubFailureHandler : IBtHandler
    {
        public BtStatus Tick(BtNode node, BtContext ctx) => BtStatus.Failure;
    }
}
