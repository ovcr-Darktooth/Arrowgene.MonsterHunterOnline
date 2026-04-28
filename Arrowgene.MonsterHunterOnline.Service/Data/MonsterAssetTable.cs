using System;
using System.Collections.Generic;
using System.IO;
using Arrowgene.Logging;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    /// <summary>
    /// Maps a server-side monster info id (e.g. spawn-point id 39002, definition id 60010) to
    /// the CryEngine asset key ("em001", "em002", ...). Source: the player-extracted spreadsheet
    /// "MHO Locations &amp; IDs - Monster Data.csv". Header columns:
    /// <c>Internal ID, Asset ID, English Name, Chinese Name, ...</c>.
    ///
    /// Some legacy spawn ids (e.g. 39002 used by <c>LevelDataNPCs.csv</c>) aren't in the canon
    /// CSV; the optional <see cref="AddOverride"/> method seeds those at startup.
    /// </summary>
    public class MonsterAssetTable
    {
        public const string AssetCsvFile = "MHO Locations & IDs - Monster Data.csv";

        private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(MonsterAssetTable));

        private readonly string _csvDir;
        private readonly Dictionary<int, string> _byInternalId = new();

        public MonsterAssetTable(string csvDir)
        {
            _csvDir = csvDir;
        }

        public int Count => _byInternalId.Count;

        public int LoadAll()
        {
            _byInternalId.Clear();
            string path = Path.Combine(_csvDir, AssetCsvFile);
            if (!File.Exists(path))
            {
                Logger.Error($"MonsterAsset CSV not found: {path}");
                return 0;
            }

            string[] lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++) // skip header
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                int firstComma = line.IndexOf(',');
                if (firstComma <= 0) continue;
                int secondComma = line.IndexOf(',', firstComma + 1);
                if (secondComma < 0) continue;

                string idText = line.Substring(0, firstComma).Trim();
                string assetText = line.Substring(firstComma + 1, secondComma - firstComma - 1).Trim();
                if (string.IsNullOrEmpty(assetText) || assetText == "???") continue;
                if (!int.TryParse(idText, out int internalId)) continue;

                _byInternalId[internalId] = assetText.ToLowerInvariant();
            }

            // Legacy spawn-point ids that aren't in the canon CSV but are referenced by
            // LevelDataNPCs.csv. Seeded here so existing spawns keep working.
            AddOverride(39002, "em001");
            AddOverride(39003, "em002");
            AddOverride(39004, "em003");

            Logger.Info($"Loaded {_byInternalId.Count} MonsterAsset entries from {AssetCsvFile}");
            return _byInternalId.Count;
        }

        public void AddOverride(int internalId, string assetId)
        {
            if (internalId <= 0 || string.IsNullOrEmpty(assetId)) return;
            _byInternalId[internalId] = assetId.ToLowerInvariant();
        }

        public bool TryGetAssetId(int internalId, out string assetId) => _byInternalId.TryGetValue(internalId, out assetId);

        public string GetAssetId(int internalId, string fallback = "em001")
        {
            return _byInternalId.TryGetValue(internalId, out string a) ? a : fallback;
        }
    }
}
