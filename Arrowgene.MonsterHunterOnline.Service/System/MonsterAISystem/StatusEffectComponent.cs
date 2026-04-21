using System.Collections.Generic;
using Arrowgene.MonsterHunterOnline.Service.Data;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem
{
    public class PartUnbalanceState
    {
        public string PartId { get; set; }
        public float Accumulated { get; set; }
        public int StaggerCount { get; set; }
    }

    /// <summary>
    /// Tracks per-part unbalance buildup. Each hit adds <c>rawDamage × UnbalanceMulti</c>
    /// to the part's accumulator; when it crosses the <c>Unbalance</c> threshold, the
    /// part staggers (sequence <c>Hit_Stun_&lt;PartId&gt;</c>) and the accumulator resets,
    /// matching the standard MH loop where the same part can be re-staggered repeatedly.
    /// </summary>
    public class StatusEffectComponent
    {
        private readonly int _monsterId;
        private readonly PartsTable _parts;
        private readonly Dictionary<string, PartUnbalanceState> _unbalance = new();

        public StatusEffectComponent(int monsterId, PartsTable parts, string stateId = "Normal")
        {
            _monsterId = monsterId;
            _parts = parts;

            if (parts == null) return;
            var defs = parts.GetParts(monsterId, stateId);
            if (defs == null) return;
            foreach (var kv in defs)
            {
                // Only parts with a non-zero Unbalance threshold are staggerable.
                if (kv.Value.Unbalance <= 0f) continue;
                _unbalance[kv.Key] = new PartUnbalanceState { PartId = kv.Key, Accumulated = 0f, StaggerCount = 0 };
            }
        }

        public IReadOnlyDictionary<string, PartUnbalanceState> Unbalance => _unbalance;

        /// <summary>
        /// Applies a hit to the given part's unbalance buildup. Returns true if the hit
        /// just triggered a stagger (threshold crossed); the caller should broadcast the
        /// matching reaction sequence. The accumulator is reset so the part can stagger again.
        /// </summary>
        public bool ApplyHit(string partId, float rawDamage, string stateId = "Normal")
        {
            if (!_unbalance.TryGetValue(partId, out var state)) return false;
            if (!_parts.TryGetPart(_monsterId, partId, stateId, out var def)) return false;
            if (def.Unbalance <= 0f) return false;

            float multi = def.UnbalanceMulti > 0f ? def.UnbalanceMulti : 1f;
            state.Accumulated += rawDamage * multi;

            if (state.Accumulated >= def.Unbalance)
            {
                state.Accumulated = 0f;
                state.StaggerCount++;
                return true;
            }
            return false;
        }
    }
}
