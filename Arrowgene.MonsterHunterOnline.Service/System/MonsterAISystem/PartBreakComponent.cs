using System.Collections.Generic;
using Arrowgene.MonsterHunterOnline.Service.Data;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem
{
    public enum PartWeaponType { Cut, Hammer, Shoot, Fire, Water, Electric, Ice, Dragon }

    public class PartBreakState
    {
        public string PartId { get; set; }
        public float CumulativeDamage { get; set; }
        public int CurrentTier { get; set; } // 0 = intact; N = past tier N
    }

    /// <summary>
    /// Tracks per-part accumulated damage and break tier progression for a single monster.
    /// Initialized from <see cref="PartsTable"/> data at spawn. Each <see cref="ApplyHit"/>
    /// returns the list of newly-crossed tier break events that the caller should surface
    /// to players (usually via a <c>Hit_PartBroken_&lt;partId&gt;</c> sequence broadcast).
    /// </summary>
    public class PartBreakComponent
    {
        private readonly int _monsterId;
        private readonly PartsTable _parts;
        private readonly Dictionary<string, PartBreakState> _state = new();

        public PartBreakComponent(int monsterId, PartsTable parts, string stateId = "Normal")
        {
            _monsterId = monsterId;
            _parts = parts;

            if (parts == null) return;
            var defs = parts.GetParts(monsterId, stateId);
            if (defs == null) return;
            foreach (var kv in defs)
            {
                // Skip entries that have no break tiers (indestructible).
                if (kv.Value.BreakTiers == null || kv.Value.BreakTiers.Count == 0) continue;
                _state[kv.Key] = new PartBreakState { PartId = kv.Key, CumulativeDamage = 0f, CurrentTier = 0 };
            }
        }

        public IReadOnlyDictionary<string, PartBreakState> States => _state;

        public bool TryGetState(string partId, out PartBreakState state) => _state.TryGetValue(partId, out state);

        /// <summary>
        /// Apply a hit to a part. Returns the list of tiers that were freshly broken by this hit
        /// (in ascending tier order). An empty list means no new break event fired.
        /// </summary>
        public List<PartBreakTier> ApplyHit(string partId, PartWeaponType weapon, float rawDamage, string stateId = "Normal")
        {
            var broken = new List<PartBreakTier>();
            if (!_state.TryGetValue(partId, out var state)) return broken;
            if (!_parts.TryGetPart(_monsterId, partId, stateId, out var def) || def.BreakTiers == null) return broken;

            // Damage-type multiplier from PartDefence (if present).
            float multiplier = 1f;
            if (_parts.TryGetDefence(_monsterId, partId, stateId, out var defence))
            {
                multiplier = weapon switch
                {
                    PartWeaponType.Cut => defence.Cut,
                    PartWeaponType.Hammer => defence.Hammer,
                    PartWeaponType.Shoot => defence.Shoot,
                    PartWeaponType.Fire => defence.Fire,
                    PartWeaponType.Water => defence.Water,
                    PartWeaponType.Electric => defence.Electric,
                    PartWeaponType.Ice => defence.Ice,
                    PartWeaponType.Dragon => defence.Dragon,
                    _ => 1f
                };
                if (multiplier <= 0f) multiplier = 0.01f; // floor so damage still accumulates
            }

            float partDamage = rawDamage * multiplier;
            state.CumulativeDamage += partDamage;

            // Cross all freshly-reached tier thresholds (damage bursts can skip tiers).
            while (state.CurrentTier < def.BreakTiers.Count
                   && state.CumulativeDamage >= def.BreakTiers[state.CurrentTier].DmgVal)
            {
                broken.Add(def.BreakTiers[state.CurrentTier]);
                state.CurrentTier++;
            }
            return broken;
        }
    }
}
