using System.Collections.Generic;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// Per-tick context handed to every handler. Owns the Blackboard plus a per-node
    /// state bag that survives across ticks — that's how Sequence/Selector remember
    /// their running-child index, and how Filter/Counter nodes count iterations.
    ///
    /// <see cref="Owner"/> is the <see cref="MonsterAI"/> the BT is driving (typed as
    /// <c>object</c> here to keep the BT layer free of MonsterAI dependencies — Phase
    /// 6.4 handlers cast as needed). <see cref="DeltaSeconds"/> is the wall-clock delta
    /// since the previous tick, fed by the runner.
    /// </summary>
    public sealed class BtContext
    {
        public Blackboard Blackboard { get; }
        public BtTreeLoader Loader { get; }
        public BtHandlerRegistry Handlers { get; }
        public object Owner { get; set; }
        public float DeltaSeconds { get; set; }

        private readonly Dictionary<int, object> _nodeState = new();

        public BtContext(Blackboard blackboard, BtTreeLoader loader, BtHandlerRegistry handlers)
        {
            Blackboard = blackboard;
            Loader = loader;
            Handlers = handlers;
        }

        public T GetState<T>(BtNode node) where T : class
        {
            return _nodeState.TryGetValue(node.NodeId, out object v) ? v as T : null;
        }

        public T GetOrCreateState<T>(BtNode node) where T : class, new()
        {
            if (_nodeState.TryGetValue(node.NodeId, out object v) && v is T existing) return existing;
            T fresh = new T();
            _nodeState[node.NodeId] = fresh;
            return fresh;
        }

        public void ResetState(BtNode node)
        {
            _nodeState.Remove(node.NodeId);
        }
    }

    internal sealed class BtCompositeState
    {
        public int RunningIndex = -1;
    }

    internal sealed class BtCounterState
    {
        public int Iterations;
    }
}
