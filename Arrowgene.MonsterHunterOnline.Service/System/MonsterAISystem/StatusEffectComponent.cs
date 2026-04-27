using System.Collections.Generic;
using Arrowgene.MonsterHunterOnline.Service.Data;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem
{
    public enum HitElement { None, Fire, Water, Dragon, Electric, Ice }

    public enum FallDirection { Left, Right }

    public class PartUnbalanceState
    {
        public string PartId { get; set; }
        public float Accumulated { get; set; }
        public int StaggerCount { get; set; }
    }

    public class FallBuildupState
    {
        public float Accumulated { get; set; }
        public int FallCount { get; set; }
    }

    public class FaintBuildupState
    {
        public float Accumulated { get; set; }
        public int FaintCount { get; set; }
    }

    public class ElementBuildupState
    {
        public HitElement Element { get; set; }
        public float Accumulated { get; set; }
        public int TriggerCount { get; set; }
    }

    /// <summary>
    /// Tracks status-effect buildups for a single monster:
    /// - Per-part unbalance (stagger): <see cref="ApplyHit"/> → Hit_Stun_&lt;Part&gt;
    /// - Whole-body fall buildup: <see cref="ApplyFallHit"/> → Hit_FallDown_&lt;L|R&gt;_Start
    /// - Whole-body faint buildup: <see cref="ApplyFaintHit"/> → Stun
    /// - Per-element buildup: <see cref="ApplyElementHit"/> → Abnormal_&lt;Element&gt;_Start
    /// Each accumulator resets on trigger so the same status can re-fire under sustained pressure
    /// (the standard MH loop). Thresholds default to constants tuned against em001 with the current
    /// FallbackDamage=1000; per-monster overrides come from MonsterDefinition once those columns are wired.
    /// </summary>
    public class StatusEffectComponent
    {
        // Defaults sized for em001 (HP 2600) with FallbackDamage=1000:
        //  - 2 hits to fall, 3 hits to stun, ~half a "build kit" for an elemental abnormal.
        public const float DefaultFallThreshold = 2000f;
        public const float DefaultFaintThreshold = 3000f;
        public const float DefaultElementThreshold = 500f;

        private readonly int _monsterId;
        private readonly PartsTable _parts;

        private readonly Dictionary<string, PartUnbalanceState> _unbalance = new();
        private readonly FallBuildupState _fall = new();
        private readonly FaintBuildupState _faint = new();
        private readonly Dictionary<HitElement, ElementBuildupState> _element = new();

        public float FallThreshold { get; set; } = DefaultFallThreshold;
        public float FaintThreshold { get; set; } = DefaultFaintThreshold;
        public Dictionary<HitElement, float> ElementThresholds { get; } = new();

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
        public FallBuildupState Fall => _fall;
        public FaintBuildupState Faint => _faint;
        public IReadOnlyDictionary<HitElement, ElementBuildupState> Element => _element;

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

        /// <summary>
        /// Adds raw damage to the whole-body fall accumulator. Returns true once the
        /// threshold is crossed (knockdown moment); the accumulator resets for the next loop.
        /// </summary>
        public bool ApplyFallHit(float rawDamage)
        {
            if (rawDamage <= 0f) return false;
            _fall.Accumulated += rawDamage;
            if (_fall.Accumulated >= FallThreshold)
            {
                _fall.Accumulated = 0f;
                _fall.FallCount++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds raw damage to the whole-body faint accumulator. In MH, faint is normally
        /// gated to head + blunt; the partId-mapping isn't reverse-engineered yet so we
        /// accept all hits as a pessimistic upper bound until weapon data is wired.
        /// </summary>
        public bool ApplyFaintHit(float rawDamage)
        {
            if (rawDamage <= 0f) return false;
            _faint.Accumulated += rawDamage;
            if (_faint.Accumulated >= FaintThreshold)
            {
                _faint.Accumulated = 0f;
                _faint.FaintCount++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds elemental buildup of the given element. Returns true when the per-element
        /// threshold is crossed, signalling that an Abnormal_&lt;Element&gt;_Start sequence
        /// should be broadcast. Caller drives element-specific buildup feeds from AttackData
        /// or weapon data — this component only tracks the accumulator.
        /// </summary>
        public bool ApplyElementHit(HitElement element, float amount)
        {
            if (element == HitElement.None || amount <= 0f) return false;
            if (!_element.TryGetValue(element, out var state))
            {
                state = new ElementBuildupState { Element = element, Accumulated = 0f, TriggerCount = 0 };
                _element[element] = state;
            }
            state.Accumulated += amount;
            float threshold = ElementThresholds.TryGetValue(element, out float t) ? t : DefaultElementThreshold;
            if (state.Accumulated >= threshold)
            {
                state.Accumulated = 0f;
                state.TriggerCount++;
                return true;
            }
            return false;
        }
    }
}
