using System;
using System.Collections.Generic;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// Maps an Action/Condition <c>Operation</c> string to its handler. Phase 6.4
    /// populates this with concrete handlers (BlackBoardCheck, SetBlackBoard,
    /// CheckHealth, BTOperation, HandleDeath, …).
    /// </summary>
    public sealed class BtHandlerRegistry
    {
        private readonly Dictionary<string, IBtHandler> _actions = new(StringComparer.Ordinal);
        private readonly Dictionary<string, IBtHandler> _conditions = new(StringComparer.Ordinal);

        public void RegisterAction(string operation, IBtHandler handler)
        {
            if (string.IsNullOrEmpty(operation)) throw new ArgumentException("operation is empty", nameof(operation));
            _actions[operation] = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void RegisterCondition(string operation, IBtHandler handler)
        {
            if (string.IsNullOrEmpty(operation)) throw new ArgumentException("operation is empty", nameof(operation));
            _conditions[operation] = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public IBtHandler GetAction(string operation)
            => operation != null && _actions.TryGetValue(operation, out IBtHandler h) ? h : null;

        public IBtHandler GetCondition(string operation)
            => operation != null && _conditions.TryGetValue(operation, out IBtHandler h) ? h : null;

        public bool HasAction(string operation) => operation != null && _actions.ContainsKey(operation);
        public bool HasCondition(string operation) => operation != null && _conditions.ContainsKey(operation);
    }
}
