namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// Standard CryEngine BT tick result. <see cref="Running"/> means the node still has
    /// work to do and must be re-ticked next frame; the parent Sequence/Selector remembers
    /// the running child's index so it can resume there.
    /// </summary>
    public enum BtStatus
    {
        Success,
        Failure,
        Running
    }
}
