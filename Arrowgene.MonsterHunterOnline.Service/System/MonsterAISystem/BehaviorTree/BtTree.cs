using System.Collections.Generic;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// A parsed BT file: the root node plus the source path it came from.
    /// SourcePath is used by <see cref="BtTreeLoader"/> to resolve relative <c>Reference</c>
    /// paths against the file's own folder.
    /// </summary>
    public sealed class BtTree
    {
        public BtNode Root { get; }
        public string SourcePath { get; }

        /// <summary>
        /// Sub-nodes addressable by name within this tree, populated when the loader
        /// indexes the tree for dotted reference selectors like <c>File.Root_node.Sleep</c>.
        /// Key = node Name, Value = the node itself. Multiple nodes can share a name in
        /// CryEngine BTs; this dict keeps the first one encountered (depth-first).
        /// </summary>
        public Dictionary<string, BtNode> NamedNodes { get; } = new();

        public BtTree(BtNode root, string sourcePath)
        {
            Root = root;
            SourcePath = sourcePath;
        }
    }
}
