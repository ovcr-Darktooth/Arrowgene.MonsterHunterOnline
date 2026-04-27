namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// Implemented by every concrete Action/Condition (Phase 6.4). The runner picks the
    /// handler from <see cref="BtHandlerRegistry"/> using the node's
    /// <c>Operation</c> attribute (e.g. "BlackBoardCheck", "SetBlackBoard",
    /// "CheckHealth", "BTOperation", "HandleDeath").
    /// </summary>
    public interface IBtHandler
    {
        BtStatus Tick(BtNode node, BtContext ctx);
    }
}
