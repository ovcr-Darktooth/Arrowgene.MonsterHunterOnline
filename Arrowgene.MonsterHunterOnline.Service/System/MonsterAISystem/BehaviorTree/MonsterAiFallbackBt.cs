using Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree.Handlers;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// Builds the programmatic fallback Behavior Tree for monsters that don't have a
    /// dedicated CryEngine BT XML wired up yet. Reproduces the legacy if/else
    /// (Idle / Chase / Attack-when-in-range) inside the BT runtime so
    /// <see cref="MonsterAI.Tick"/> can route everything through one code path.
    ///
    /// Tree shape:
    /// <code>
    /// Selector (Top)
    /// ├── Sequence (Engage)
    /// │   ├── Condition: HasTarget
    /// │   └── Selector (AttackOrChase)
    /// │       ├── Sequence (AttackInRange)
    /// │       │   ├── Condition: InAttackRange
    /// │       │   └── Action:    PerformAttack
    /// │       └── Action:    MoveTowardTarget
    /// └── Action:    GoIdle
    /// </code>
    /// </summary>
    public static class MonsterAiFallbackBt
    {
        private const string FallbackXml =
            "<Behavior Ver=\"1.01\">"
            + "<Node Type=\"Root\" Node_id=\"1\" Name=\"FallbackRoot\">"
            + "<Connector Identifier=\"GenericChildren\">"
            + "<Node Type=\"Selector\" Node_id=\"2\" Name=\"Top\">"
            + "<Connector Identifier=\"GenericChildren\">"
            + "<Node Type=\"Sequence\" Node_id=\"3\" Name=\"Engage\">"
            + "<Connector Identifier=\"GenericChildren\">"
            + "<Node Type=\"Condition\" Node_id=\"4\" Operation=\"HasTarget\"/>"
            + "<Node Type=\"Selector\" Node_id=\"5\" Name=\"AttackOrChase\">"
            + "<Connector Identifier=\"GenericChildren\">"
            + "<Node Type=\"Sequence\" Node_id=\"6\" Name=\"AttackInRange\">"
            + "<Connector Identifier=\"GenericChildren\">"
            + "<Node Type=\"Condition\" Node_id=\"7\" Operation=\"InAttackRange\"/>"
            + "<Node Type=\"Action\" Node_id=\"8\" Operation=\"PerformAttack\"/>"
            + "</Connector></Node>"
            + "<Node Type=\"Action\" Node_id=\"9\" Operation=\"MoveTowardTarget\"/>"
            + "</Connector></Node>"
            + "</Connector></Node>"
            + "<Node Type=\"Action\" Node_id=\"10\" Operation=\"GoIdle\"/>"
            + "</Connector></Node>"
            + "</Connector></Node>"
            + "</Behavior>";

        public static BtRunner BuildRunner(MonsterAI owner, Blackboard blackboard)
        {
            BtNode root = BtParser.Parse(FallbackXml);
            BtTree tree = new BtTree(root, "(fallback)");
            BtHandlerRegistry registry = new BtHandlerRegistry();
            FallbackBtHandlers.RegisterAll(registry);
            BtContext ctx = new BtContext(blackboard, null, registry) { Owner = owner };
            return new BtRunner(tree, ctx);
        }
    }
}
