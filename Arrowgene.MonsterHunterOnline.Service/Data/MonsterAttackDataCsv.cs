using System.Globalization;
using Arrowgene.MonsterHunterOnline.Service.System.ClientAssetSystem;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    public class MonsterAttackDataCsv : CsvReaderWriter<AttackData>
    {
        protected override int NumExpectedItems => 71;

        private static bool TryFloatInvariant(string value, out float result) =>
            float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);

        private static bool TryIntInvariant(string value, out int result) =>
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

        protected override AttackData CreateInstance(string[] properties)
        {
            if (!TryParse(properties, 0, out int id, TryIntInvariant, 0)) return null;
            if (id <= 0) return null;

            if (!TryParse(properties, 39, out float damageNumber, TryFloatInvariant, 0f)) return null;
            if (!TryParse(properties, 40, out int piyo, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 41, out int stamina, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 42, out int damageLevel, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 44, out int damageLevelNumber, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 45, out int damagePower, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 48, out int attackLevel, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 49, out int damageDirUse, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 51, out int fireAtk, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 52, out int waterAtk, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 53, out int dragonAtk, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 54, out int electricAtk, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 55, out int iceAtk, TryIntInvariant, 0)) return null;
            if (!TryParse(properties, 70, out int stateBuff, TryIntInvariant, 0)) return null;

            return new AttackData
            {
                Id = id,
                Note = properties[4],
                AttackName = properties[5],
                DamageNumber = damageNumber,
                Piyo = piyo,
                Stamina = stamina,
                DamageLevel = damageLevel,
                DamageLevelNumber = damageLevelNumber,
                DamagePower = damagePower,
                AttackLevel = attackLevel,
                DamageDirUse = damageDirUse,
                DamageDir = properties[50],
                FireAtk = fireAtk,
                WaterAtk = waterAtk,
                DragonAtk = dragonAtk,
                ElectricAtk = electricAtk,
                IceAtk = iceAtk,
                StateBuff = stateBuff,
            };
        }
    }
}
