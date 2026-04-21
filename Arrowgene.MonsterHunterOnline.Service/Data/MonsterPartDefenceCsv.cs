using System.Globalization;
using Arrowgene.MonsterHunterOnline.Service.System.ClientAssetSystem;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    /// <summary>
    /// Parser for <c>monsterdata.dat_PartDefence.csv</c>.
    /// 14 columns: MonsterID, PartID, StateID, BreakLevel, DefenceLevel, Faint,
    /// Cut, Hammer, Shoot, Fire, Water, Electric, Ice, Dragon.
    /// </summary>
    public class MonsterPartDefenceCsv : CsvReaderWriter<MonsterPartDefence>
    {
        protected override int NumExpectedItems => 14;

        private static bool TryIntInvariant(string value, out int result) =>
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

        private static bool TryFloatInvariant(string value, out float result) =>
            float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);

        protected override MonsterPartDefence CreateInstance(string[] properties)
        {
            if (!TryParse(properties, 0, out int monsterId, TryIntInvariant, 0)) return null;
            if (monsterId <= 0) return null;

            string partId = properties[1];
            if (string.IsNullOrEmpty(partId)) return null;

            TryParse(properties, 3, out int breakLevel, TryIntInvariant, 0);
            TryParse(properties, 4, out int defenceLevel, TryIntInvariant, 0);
            TryParse(properties, 5, out float faint, TryFloatInvariant, 0f);
            TryParse(properties, 6, out float cut, TryFloatInvariant, 0f);
            TryParse(properties, 7, out float hammer, TryFloatInvariant, 0f);
            TryParse(properties, 8, out float shoot, TryFloatInvariant, 0f);
            TryParse(properties, 9, out float fire, TryFloatInvariant, 0f);
            TryParse(properties, 10, out float water, TryFloatInvariant, 0f);
            TryParse(properties, 11, out float electric, TryFloatInvariant, 0f);
            TryParse(properties, 12, out float ice, TryFloatInvariant, 0f);
            TryParse(properties, 13, out float dragon, TryFloatInvariant, 0f);

            return new MonsterPartDefence
            {
                MonsterId = monsterId,
                PartId = partId,
                StateId = properties[2],
                BreakLevel = breakLevel,
                DefenceLevel = defenceLevel,
                Faint = faint,
                Cut = cut,
                Hammer = hammer,
                Shoot = shoot,
                Fire = fire,
                Water = water,
                Electric = electric,
                Ice = ice,
                Dragon = dragon,
            };
        }
    }
}
