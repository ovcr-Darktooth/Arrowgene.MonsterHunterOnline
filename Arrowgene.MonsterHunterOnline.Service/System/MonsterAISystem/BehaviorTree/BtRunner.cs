using System;
using System.Collections.Generic;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// Walks a parsed BT tree and dispatches each node to its handler. One runner per
    /// monster instance. CryEngine semantics:
    ///
    /// <list type="bullet">
    /// <item><b>Root</b>: ticks its single child, returns the child's status.</item>
    /// <item><b>Selector</b> (priority): ticks children in order, returns Success on the
    /// first child that succeeds, Running if a child runs (and remembers the index so
    /// the next tick resumes there), Failure if all children fail.</item>
    /// <item><b>Sequence</b>: ticks children in order, returns Failure on the first
    /// child that fails, Running if a child runs (resumes at index), Success if all
    /// succeed.</item>
    /// <item><b>Condition</b>: dispatches to a registered Condition handler.</item>
    /// <item><b>Action</b>: dispatches to a registered Action handler.</item>
    /// <item><b>Filter</b>: decorator. <c>Filter_Type=Non</c> = ticks once, returns child
    /// status as-is (CryEngine groups under Filter for grouping). <c>Until_Fails</c> =
    /// loops the child until it returns Failure, then returns Success.
    /// <c>FilterType=Counter</c> caps the loop iteration count via <c>Count</c> attr.</item>
    /// <item><b>Reference</b>: lazily resolves the referenced sub-tree via the loader,
    /// then ticks its root.</item>
    /// </list>
    ///
    /// Unknown Operations log once and return Failure rather than crashing — Phase 6.4
    /// will fill the registry; the BT must remain tickable while it does.
    /// </summary>
    public sealed class BtRunner
    {
        private readonly BtTree _tree;
        private readonly BtContext _ctx;
        private readonly HashSet<string> _missingOps = new();

        public BtRunner(BtTree tree, BtContext ctx)
        {
            _tree = tree ?? throw new ArgumentNullException(nameof(tree));
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public BtTree Tree => _tree;
        public BtContext Context => _ctx;

        /// <summary>Operations referenced by the tree but not registered. For diagnostics.</summary>
        public IReadOnlyCollection<string> MissingOperations => _missingOps;

        public BtStatus Tick(float deltaSeconds)
        {
            _ctx.DeltaSeconds = deltaSeconds;
            _ctx.TotalSeconds += deltaSeconds;
            return TickNode(_tree.Root);
        }

        private BtStatus TickNode(BtNode node)
        {
            switch (node)
            {
                case BtRoot _:
                    return node.Children.Count > 0 ? TickNode(node.Children[0]) : BtStatus.Success;

                case BtSelector sel:
                    return TickSelector(sel);

                case BtSequence seq:
                    return TickSequence(seq);

                case BtCondition cond:
                    return Dispatch(cond, _ctx.Handlers.GetCondition(cond.Operation), cond.Operation);

                case BtAction act:
                    return Dispatch(act, _ctx.Handlers.GetAction(act.Operation), act.Operation);

                case BtFilter filt:
                    return TickFilter(filt);

                case BtReference reff:
                    return TickReference(reff);

                default:
                    return BtStatus.Failure;
            }
        }

        private BtStatus TickSelector(BtSelector sel)
        {
            BtCompositeState state = _ctx.GetOrCreateState<BtCompositeState>(sel);
            int start = state.RunningIndex >= 0 ? state.RunningIndex : 0;

            for (int i = start; i < sel.Children.Count; i++)
            {
                BtStatus s = TickNode(sel.Children[i]);
                if (s == BtStatus.Running)
                {
                    state.RunningIndex = i;
                    return BtStatus.Running;
                }
                if (s == BtStatus.Success)
                {
                    state.RunningIndex = -1;
                    return BtStatus.Success;
                }
                // Failure → try next.
            }

            state.RunningIndex = -1;
            return BtStatus.Failure;
        }

        private BtStatus TickSequence(BtSequence seq)
        {
            BtCompositeState state = _ctx.GetOrCreateState<BtCompositeState>(seq);
            int start = state.RunningIndex >= 0 ? state.RunningIndex : 0;

            for (int i = start; i < seq.Children.Count; i++)
            {
                BtStatus s = TickNode(seq.Children[i]);
                if (s == BtStatus.Running)
                {
                    state.RunningIndex = i;
                    return BtStatus.Running;
                }
                if (s == BtStatus.Failure)
                {
                    state.RunningIndex = -1;
                    return BtStatus.Failure;
                }
                // Success → continue with next.
            }

            state.RunningIndex = -1;
            return BtStatus.Success;
        }

        private BtStatus TickFilter(BtFilter filt)
        {
            if (filt.Children.Count == 0) return BtStatus.Success;

            // CryEngine Filter has 1 logical child (wrapped in <Connector>). If the parser
            // collected several, treat them as a sequence — that's the CE3 default.
            string ft = filt.FilterType;          // "Until_Fails" | "Non" | null
            string fc = filt.FilterCategory;      // "Counter" | null
            string maxAttr = filt.GetAttr("Count") ?? filt.GetAttr("Max");
            int max = int.TryParse(maxAttr, out int m) ? m : int.MaxValue;

            if (string.Equals(ft, "Until_Fails", StringComparison.Ordinal))
            {
                BtCounterState cs = _ctx.GetOrCreateState<BtCounterState>(filt);
                BtStatus s = TickAllChildrenAsSequence(filt);
                if (s == BtStatus.Running) return BtStatus.Running;
                if (s == BtStatus.Failure)
                {
                    cs.Iterations = 0;
                    return BtStatus.Success;
                }
                // Success → loop again next tick.
                cs.Iterations++;
                if (string.Equals(fc, "Counter", StringComparison.Ordinal) && cs.Iterations >= max)
                {
                    cs.Iterations = 0;
                    return BtStatus.Success;
                }
                return BtStatus.Running;
            }

            // "Non" / null → pass-through grouping (no inversion in CE3 despite the name).
            return TickAllChildrenAsSequence(filt);
        }

        private BtStatus TickAllChildrenAsSequence(BtNode parent)
        {
            BtCompositeState state = _ctx.GetOrCreateState<BtCompositeState>(parent);
            int start = state.RunningIndex >= 0 ? state.RunningIndex : 0;

            for (int i = start; i < parent.Children.Count; i++)
            {
                BtStatus s = TickNode(parent.Children[i]);
                if (s == BtStatus.Running)
                {
                    state.RunningIndex = i;
                    return BtStatus.Running;
                }
                if (s == BtStatus.Failure)
                {
                    state.RunningIndex = -1;
                    return BtStatus.Failure;
                }
            }

            state.RunningIndex = -1;
            return BtStatus.Success;
        }

        private BtStatus TickReference(BtReference reff)
        {
            BtRefState state = _ctx.GetOrCreateState<BtRefState>(reff);
            if (state.Resolved == null && !state.ResolutionFailed)
            {
                if (_ctx.Loader == null)
                {
                    state.ResolutionFailed = true;
                    return BtStatus.Failure;
                }
                BtTree sub = _ctx.Loader.Resolve(reff.Reference, _tree, out string sel);
                if (sub == null)
                {
                    state.ResolutionFailed = true;
                    return BtStatus.Failure;
                }
                state.Resolved = sub;
                state.SubNodeSelector = sel;
            }

            if (state.ResolutionFailed) return BtStatus.Failure;

            BtNode entry = ResolveEntryNode(state.Resolved, state.SubNodeSelector);
            if (entry == null) return BtStatus.Failure;
            return TickNode(entry);
        }

        private static BtNode ResolveEntryNode(BtTree tree, string selector)
        {
            if (string.IsNullOrEmpty(selector)) return tree.Root;
            // Selectors are dotted node names like "Root_node.Sleep". The last segment
            // is what the reference points at; index lookup gives us the node directly.
            int lastDot = selector.LastIndexOf('.');
            string leaf = lastDot >= 0 ? selector.Substring(lastDot + 1) : selector;
            return tree.NamedNodes.TryGetValue(leaf, out BtNode node) ? node : tree.Root;
        }

        private BtStatus Dispatch(BtNode node, IBtHandler handler, string operation)
        {
            if (handler != null) return handler.Tick(node, _ctx);
            if (operation != null) _missingOps.Add(operation);
            return BtStatus.Failure;
        }

        private sealed class BtRefState
        {
            public BtTree Resolved;
            public string SubNodeSelector;
            public bool ResolutionFailed;
        }
    }
}
