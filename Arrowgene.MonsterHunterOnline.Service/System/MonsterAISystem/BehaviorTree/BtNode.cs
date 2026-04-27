using System.Collections.Generic;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// Base class for every parsed CryEngine 3 BT node. Pure data — no runtime/tick logic yet
    /// (that lands in Phase 6.3). Each concrete subclass exposes the type-specific attributes
    /// the runtime will need; everything else stays in <see cref="Attributes"/> as raw strings
    /// so Operation-specific data (KeyName, Value, Percentage, BTName, …) can be read by the
    /// Action/Condition implementations without touching the parser.
    /// </summary>
    public abstract class BtNode
    {
        public string Name { get; set; }
        public int NodeId { get; set; }
        public List<BtNode> Children { get; } = new();
        public Dictionary<string, string> Attributes { get; } = new();

        public abstract string Kind { get; }

        public string GetAttr(string key)
        {
            return Attributes.TryGetValue(key, out string v) ? v : null;
        }
    }

    public sealed class BtRoot : BtNode
    {
        public override string Kind => "Root";
    }

    public sealed class BtSelector : BtNode
    {
        public string SelectorType { get; set; }
        public override string Kind => "Selector";
    }

    public sealed class BtSequence : BtNode
    {
        public override string Kind => "Sequence";
    }

    /// <summary>
    /// Leaf returning Success/Failure. <see cref="Operation"/> identifies which Condition handler
    /// the runtime should dispatch to (e.g. "BlackBoardCheck", "CheckHealth"). Operation-specific
    /// args are in <see cref="BtNode.Attributes"/>.
    /// </summary>
    public sealed class BtCondition : BtNode
    {
        public string Operation { get; set; }
        public override string Kind => "Condition";
    }

    /// <summary>
    /// Leaf with side effects. <see cref="Operation"/> selects the Action handler
    /// (e.g. "SetBlackBoard", "BTOperation", "HandleDeath"). Args in <see cref="BtNode.Attributes"/>.
    /// </summary>
    public sealed class BtAction : BtNode
    {
        public string Operation { get; set; }
        public override string Kind => "Action";
    }

    /// <summary>
    /// Decorator that loops or inverts its child. CryEngine encodes the variant on either
    /// <c>Filter_Type</c> ("Until_Fails", "Non", …) or <c>FilterType</c> ("Counter") — we keep
    /// both raw values so the runtime can pick the right semantics later.
    /// </summary>
    public sealed class BtFilter : BtNode
    {
        public string FilterType { get; set; }       // Filter_Type=
        public string FilterCategory { get; set; }   // FilterType=
        public override string Kind => "Filter";
    }

    /// <summary>
    /// Inlines another BT file as a sub-tree. Reference forms observed on em001:
    /// <list type="bullet">
    /// <item><c>.\Em001Idle.xml</c> — relative to current file's folder</item>
    /// <item><c>Em001Sense.xml</c> — same folder, no leading dot</item>
    /// <item><c>..\Common_CE2\Evaluators\X.xml</c> — parent-folder traversal</item>
    /// <item><c>Em001Sleep.Root_node.Sleep</c> — file + dotted sub-node selector</item>
    /// </list>
    /// Resolution is the loader's job; the parser only captures the raw string.
    /// </summary>
    public sealed class BtReference : BtNode
    {
        public string Reference { get; set; }
        public override string Kind => "Reference";
    }
}
