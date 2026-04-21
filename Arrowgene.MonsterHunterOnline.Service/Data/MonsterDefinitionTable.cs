using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Arrowgene.Logging;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    public class MonsterDefinitionTable
    {
        public const string MonstersFile = "monsterdata.dat_Monsters.csv";

        private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(MonsterDefinitionTable));

        private readonly string _staticCsvDir;
        private readonly Dictionary<int, MonsterDefinition> _byId = new();

        public MonsterDefinitionTable(string staticCsvDir)
        {
            _staticCsvDir = staticCsvDir;
            Entries = new ReadOnlyDictionary<int, MonsterDefinition>(_byId);
        }

        public ReadOnlyDictionary<int, MonsterDefinition> Entries { get; }

        public int Count => _byId.Count;

        public int LoadAll()
        {
            _byId.Clear();
            string path = Path.Combine(_staticCsvDir, MonstersFile);
            if (!File.Exists(path))
            {
                Logger.Error($"MonsterDefinition CSV not found: {path}");
                return 0;
            }
            var reader = new MonsterDefinitionCsv();
            var entries = reader.ReadPath(path);
            foreach (var e in entries) _byId[e.Id] = e;
            Logger.Info($"Loaded {_byId.Count} MonsterDefinition entries from {MonstersFile}");
            return _byId.Count;
        }

        public bool TryGet(int id, out MonsterDefinition def) => _byId.TryGetValue(id, out def);

        public MonsterDefinition Get(int id) => _byId.TryGetValue(id, out var d) ? d : null;
    }
}
