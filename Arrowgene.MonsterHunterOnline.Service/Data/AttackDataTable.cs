using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Arrowgene.Logging;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    public class AttackDataTable
    {
        public const string MonsterAttackDataFile = "attackdata_monster.dat_MonsterAttackData.csv";

        private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(AttackDataTable));

        private readonly string _staticCsvDir;
        private readonly Dictionary<int, AttackData> _byId = new();

        public AttackDataTable(string staticCsvDir)
        {
            _staticCsvDir = staticCsvDir;
            Entries = new ReadOnlyDictionary<int, AttackData>(_byId);
        }

        public ReadOnlyDictionary<int, AttackData> Entries { get; }

        public int Count => _byId.Count;

        public int LoadAll()
        {
            _byId.Clear();
            LoadFile(MonsterAttackDataFile);
            return _byId.Count;
        }

        public bool TryGet(int id, out AttackData data) => _byId.TryGetValue(id, out data);

        public AttackData Get(int id) => _byId.TryGetValue(id, out var d) ? d : null;

        public int GetDamage(int id) => _byId.TryGetValue(id, out var d) ? d.DamagePower : 0;

        private void LoadFile(string fileName)
        {
            string path = Path.Combine(_staticCsvDir, fileName);
            if (!File.Exists(path))
            {
                Logger.Error($"AttackData CSV not found: {path}");
                return;
            }

            var reader = new MonsterAttackDataCsv();
            var entries = reader.ReadPath(path);
            foreach (var e in entries)
            {
                if (_byId.ContainsKey(e.Id))
                {
                    Logger.Info($"Duplicate AttackData id {e.Id} in {fileName}, replacing");
                }
                _byId[e.Id] = e;
            }
            Logger.Info($"Loaded {entries.Count} AttackData entries from {fileName}");
        }
    }
}
