using System.Collections.Generic;
using System.IO;
using Arrowgene.Logging;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    /// <summary>
    /// Loads and indexes <c>monsterdata.dat_Parts.csv</c> and
    /// <c>monsterdata.dat_PartDefence.csv</c>. Keyed by (monsterId, partId, stateId).
    /// </summary>
    public class PartsTable
    {
        public const string PartsFile = "monsterdata.dat_Parts.csv";
        public const string PartDefenceFile = "monsterdata.dat_PartDefence.csv";

        private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(PartsTable));

        private readonly string _staticCsvDir;

        // monsterId -> stateId -> partId -> definition
        private readonly Dictionary<int, Dictionary<string, Dictionary<string, MonsterPartDefinition>>> _parts = new();
        private readonly Dictionary<int, Dictionary<string, Dictionary<string, MonsterPartDefence>>> _defence = new();

        public PartsTable(string staticCsvDir)
        {
            _staticCsvDir = staticCsvDir;
        }

        public int PartsCount { get; private set; }
        public int DefenceCount { get; private set; }

        public void LoadAll()
        {
            _parts.Clear();
            _defence.Clear();

            string partsPath = Path.Combine(_staticCsvDir, PartsFile);
            if (File.Exists(partsPath))
            {
                var entries = new MonsterPartsCsv().ReadPath(partsPath);
                foreach (var e in entries)
                {
                    GetOrAdd(_parts, e.MonsterId, e.StateId ?? string.Empty)[e.PartId] = e;
                }
                PartsCount = entries.Count;
                Logger.Info($"Loaded {PartsCount} MonsterPart entries from {PartsFile}");
            }
            else
            {
                Logger.Error($"Parts CSV not found: {partsPath}");
            }

            string defPath = Path.Combine(_staticCsvDir, PartDefenceFile);
            if (File.Exists(defPath))
            {
                var entries = new MonsterPartDefenceCsv().ReadPath(defPath);
                foreach (var e in entries)
                {
                    GetOrAdd(_defence, e.MonsterId, e.StateId ?? string.Empty)[e.PartId] = e;
                }
                DefenceCount = entries.Count;
                Logger.Info($"Loaded {DefenceCount} MonsterPartDefence entries from {PartDefenceFile}");
            }
            else
            {
                Logger.Error($"PartDefence CSV not found: {defPath}");
            }
        }

        public IReadOnlyDictionary<string, MonsterPartDefinition> GetParts(int monsterId, string stateId = "Normal")
        {
            if (_parts.TryGetValue(monsterId, out var byState)
                && byState.TryGetValue(stateId ?? string.Empty, out var byPart))
            {
                return byPart;
            }
            return null;
        }

        public bool TryGetPart(int monsterId, string partId, string stateId, out MonsterPartDefinition def)
        {
            def = null;
            if (_parts.TryGetValue(monsterId, out var byState)
                && byState.TryGetValue(stateId ?? string.Empty, out var byPart)
                && byPart.TryGetValue(partId, out def))
            {
                return true;
            }
            return false;
        }

        public bool TryGetDefence(int monsterId, string partId, string stateId, out MonsterPartDefence defence)
        {
            defence = null;
            if (_defence.TryGetValue(monsterId, out var byState)
                && byState.TryGetValue(stateId ?? string.Empty, out var byPart)
                && byPart.TryGetValue(partId, out defence))
            {
                return true;
            }
            return false;
        }

        private static Dictionary<string, TValue> GetOrAdd<TValue>(
            Dictionary<int, Dictionary<string, Dictionary<string, TValue>>> root,
            int monsterId,
            string stateId)
        {
            if (!root.TryGetValue(monsterId, out var byState))
            {
                byState = new Dictionary<string, Dictionary<string, TValue>>();
                root[monsterId] = byState;
            }
            if (!byState.TryGetValue(stateId, out var byPart))
            {
                byPart = new Dictionary<string, TValue>();
                byState[stateId] = byPart;
            }
            return byPart;
        }
    }
}
