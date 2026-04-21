using System.Globalization;
using Arrowgene.MonsterHunterOnline.Service.System.ClientAssetSystem;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    public class MonsterDefinitionCsv : CsvReaderWriter<MonsterDefinition>
    {
        protected override int NumExpectedItems => 75;

        private static bool TryIntInvariant(string value, out int result) =>
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

        protected override MonsterDefinition CreateInstance(string[] properties)
        {
            if (!TryParse(properties, 0, out int id, TryIntInvariant, 0)) return null;
            if (id <= 0) return null;

            // Column 26 (0-indexed) = " BB:MaxHealth" (leading space in header). May be blank.
            TryParse(properties, 26, out int maxHealth, TryIntInvariant, 0);

            return new MonsterDefinition
            {
                Id = id,
                Name = properties.Length > 2 ? properties[2] : string.Empty,
                EntityName = properties.Length > 7 ? properties[7] : string.Empty,
                MaxHealth = maxHealth,
            };
        }
    }
}
