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
        private readonly Dictionary<uint, int> _idByHashLower = new();
        private readonly Dictionary<uint, int> _idByHashExact = new();

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
            _idByHashLower.Clear();
            _idByHashExact.Clear();
            LoadFile(MonsterAttackDataFile);
            BuildHashIndex();
            return _byId.Count;
        }

        public bool TryGet(int id, out AttackData data) => _byId.TryGetValue(id, out data);

        public AttackData Get(int id) => _byId.TryGetValue(id, out var d) ? d : null;

        public int GetDamage(int id) => _byId.TryGetValue(id, out var d) ? d.DamagePower : 0;

        /// <summary>
        /// Resolve a CryEngine-style name hash coming from the client (`BattleDMG.hashAttacker`)
        /// to an AttackData ID. Tries lowercase-CRC32 first (CryEngine's CCrc32::ComputeLowercase
        /// convention), falls back to raw-case CRC32 if the lowercase lookup misses.
        /// </summary>
        public bool TryGetIdByHash(uint hash, out int id)
        {
            if (_idByHashLower.TryGetValue(hash, out id)) return true;
            if (_idByHashExact.TryGetValue(hash, out id)) return true;
            return false;
        }

        private void BuildHashIndex()
        {
            foreach (var e in _byId.Values)
            {
                if (string.IsNullOrEmpty(e.AttackName)) continue;
                uint lower = Crc32.ComputeLowercase(e.AttackName);
                uint exact = Crc32.Compute(e.AttackName);
                _idByHashLower[lower] = e.Id;
                _idByHashExact[exact] = e.Id;
            }
            Logger.Info($"AttackData hash index built: {_idByHashLower.Count} lowercase, {_idByHashExact.Count} exact");
        }

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
