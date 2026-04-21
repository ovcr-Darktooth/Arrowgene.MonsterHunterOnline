using System.Globalization;
using Arrowgene.MonsterHunterOnline.Service.System.ClientAssetSystem;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    /// <summary>
    /// Parser for <c>monsterdata.dat_Parts.csv</c>. 76 columns total:
    /// 21 base columns + 5 break tiers × 11 columns. Tier 5 swaps Ice &amp; LootSkillLv
    /// positions compared to tiers 1-4 (source-CSV quirk).
    /// </summary>
    public class MonsterPartsCsv : CsvReaderWriter<MonsterPartDefinition>
    {
        protected override int NumExpectedItems => 76;

        private static bool TryIntInvariant(string value, out int result) =>
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

        private static bool TryFloatInvariant(string value, out float result) =>
            float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);

        protected override MonsterPartDefinition CreateInstance(string[] properties)
        {
            if (!TryParse(properties, 0, out int monsterId, TryIntInvariant, 0)) return null;
            if (monsterId <= 0) return null;

            string partId = properties[1];
            if (string.IsNullOrEmpty(partId)) return null;

            var def = new MonsterPartDefinition
            {
                MonsterId = monsterId,
                PartId = partId,
                PartName = properties[2],
                StateId = properties[3],
            };

            TryParse(properties, 4, out float unbalance, TryFloatInvariant, 0f); def.Unbalance = unbalance;
            TryParse(properties, 5, out float unbalanceMulti, TryFloatInvariant, 0f); def.UnbalanceMulti = unbalanceMulti;
            TryParse(properties, 6, out float scar, TryFloatInvariant, 0f); def.Scar = scar;
            TryParse(properties, 7, out float fall, TryFloatInvariant, 0f); def.Fall = fall;
            TryParse(properties, 8, out float fallMulti, TryFloatInvariant, 0f); def.FallMulti = fallMulti;
            TryParse(properties, 9, out float waterAcc, TryFloatInvariant, 0f); def.WaterAcc = waterAcc;
            TryParse(properties, 10, out float waterAccMulti, TryFloatInvariant, 0f); def.WaterAccMulti = waterAccMulti;
            TryParse(properties, 11, out float fireAcc, TryFloatInvariant, 0f); def.FireAcc = fireAcc;
            TryParse(properties, 12, out float fireAccMulti, TryFloatInvariant, 0f); def.FireAccMulti = fireAccMulti;
            TryParse(properties, 13, out float electricAcc, TryFloatInvariant, 0f); def.ElectricAcc = electricAcc;
            TryParse(properties, 14, out float electricAccMulti, TryFloatInvariant, 0f); def.ElectricAccMulti = electricAccMulti;
            TryParse(properties, 15, out float dragonAcc, TryFloatInvariant, 0f); def.DragonAcc = dragonAcc;
            TryParse(properties, 16, out float dragonAccMulti, TryFloatInvariant, 0f); def.DragonAccMulti = dragonAccMulti;
            TryParse(properties, 17, out float iceAcc, TryFloatInvariant, 0f); def.IceAcc = iceAcc;
            TryParse(properties, 18, out float iceAccMulti, TryFloatInvariant, 0f); def.IceAccMulti = iceAccMulti;
            TryParse(properties, 19, out float noneAcc, TryFloatInvariant, 0f); def.NoneAcc = noneAcc;
            TryParse(properties, 20, out float noneAccMulti, TryFloatInvariant, 0f); def.NoneAccMulti = noneAccMulti;

            // Tiers 1-4: DmgVal, ProcessLv, Hammer, Cut, Shoot, Water, Fire, Electric, Dragon, Ice, LootSkillLv
            for (int tier = 1; tier <= 4; tier++)
            {
                int baseCol = 21 + (tier - 1) * 11;
                var t = ReadTier(properties, tier, baseCol, iceOffset: 9, lootOffset: 10);
                if (t != null) def.BreakTiers.Add(t);
            }

            // Tier 5: Ice & LootSkillLv swapped (source CSV quirk). Order in header is
            // ...,Dragon5,LootSkillLv5,Ice5 so LootSkillLv is at +9 and Ice at +10.
            {
                int baseCol = 21 + 4 * 11; // 65
                var t = ReadTier(properties, 5, baseCol, iceOffset: 10, lootOffset: 9);
                if (t != null) def.BreakTiers.Add(t);
            }

            return def;
        }

        private PartBreakTier ReadTier(string[] p, int tier, int baseCol, int iceOffset, int lootOffset)
        {
            TryParse(p, baseCol + 0, out float dmgVal, TryFloatInvariant, 0f);
            if (dmgVal <= 0f) return null;

            TryParse(p, baseCol + 1, out int processLv, TryIntInvariant, 0);
            TryParse(p, baseCol + 2, out float hammer, TryFloatInvariant, 0f);
            TryParse(p, baseCol + 3, out float cut, TryFloatInvariant, 0f);
            TryParse(p, baseCol + 4, out float shoot, TryFloatInvariant, 0f);
            TryParse(p, baseCol + 5, out float water, TryFloatInvariant, 0f);
            TryParse(p, baseCol + 6, out float fire, TryFloatInvariant, 0f);
            TryParse(p, baseCol + 7, out float electric, TryFloatInvariant, 0f);
            TryParse(p, baseCol + 8, out float dragon, TryFloatInvariant, 0f);
            TryParse(p, baseCol + iceOffset, out float ice, TryFloatInvariant, 0f);
            TryParse(p, baseCol + lootOffset, out int lootSkillLv, TryIntInvariant, 0);

            return new PartBreakTier
            {
                Tier = tier,
                DmgVal = dmgVal,
                ProcessLv = processLv,
                Hammer = hammer,
                Cut = cut,
                Shoot = shoot,
                Water = water,
                Fire = fire,
                Electric = electric,
                Dragon = dragon,
                Ice = ice,
                LootSkillLv = lootSkillLv,
            };
        }
    }
}
