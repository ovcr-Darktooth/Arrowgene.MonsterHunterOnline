using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Arrowgene.Logging;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    public class SequenceManager
    {
        private const string FileSuffix = ".xml_decrypted.xml";
        private const string LegacySkillSuffix = "skill.xml_decrypted.xml";

        private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(SequenceManager));

        private readonly ConcurrentDictionary<string, SequenceSet> _cache = new(StringComparer.OrdinalIgnoreCase);
        private readonly string _sequencesDir;

        public SequenceManager(string sequencesDir)
        {
            _sequencesDir = sequencesDir;
        }

        public int LoadedCount => _cache.Count;

        public IEnumerable<SequenceSet> All => _cache.Values;

        /// <summary>
        /// Loads every sequencegroup file in the directory. Safe to call at startup:
        /// returns (loaded, failed) counts, never throws on per-file errors.
        /// </summary>
        public (int loaded, int failed) LoadAll()
        {
            if (!Directory.Exists(_sequencesDir))
            {
                Logger.Error($"Sequence directory not found: {_sequencesDir}");
                return (0, 0);
            }

            int loaded = 0;
            int failed = 0;
            foreach (var file in Directory.EnumerateFiles(_sequencesDir, "*" + FileSuffix))
            {
                string fileName = Path.GetFileName(file);
                if (fileName.EndsWith(".bak", StringComparison.OrdinalIgnoreCase)) continue;
                string refName = ExtractRefName(fileName);

                try
                {
                    SequenceSet seqSet = ParseFile(file);
                    // Files without a SkillEditor root (e.g. MonsterCustomType archetypes that
                    // happen to live in the same folder) parse to an empty set — skip them.
                    if (seqSet == null) continue;
                    seqSet.SourceFile = fileName;
                    seqSet.RefName = refName;
                    _cache[fileName] = seqSet;
                    loaded++;
                }
                catch (Exception ex)
                {
                    failed++;
                    Logger.Error($"Failed to parse {fileName}: {ex.Message}");
                }
            }

            Logger.Info($"SequenceManager loaded {loaded} file(s), {failed} failure(s).");
            return (loaded, failed);
        }

        public SequenceSet GetOrLoad(string monsterName)
        {
            // Try the canonical "<name>skill.xml_decrypted.xml" first (matches em001, em008, etc.),
            // then fall back to "<name>.xml_decrypted.xml" for specialized files (citynpc, npc_common...).
            foreach (string fileName in new[] { monsterName + LegacySkillSuffix, monsterName + FileSuffix })
            {
                if (_cache.TryGetValue(fileName, out var cachedSet))
                {
                    return cachedSet;
                }

                string fullPath = Path.Combine(_sequencesDir, fileName);
                if (!File.Exists(fullPath)) continue;

                try
                {
                    SequenceSet seqSet = ParseFile(fullPath);
                    if (seqSet == null) return null;
                    seqSet.SourceFile = fileName;
                    seqSet.RefName = monsterName;
                    _cache[fileName] = seqSet;
                    return seqSet;
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                    return null;
                }
            }

            Logger.Error($"Sequence file not found for '{monsterName}' in {_sequencesDir}");
            return null;
        }

        /// <summary>
        /// "em001skill.xml_decrypted.xml" → "em001".
        /// "citynpc.xml_decrypted.xml"    → "citynpc".
        /// Keeps legacy GetOrLoad("em001") compatible with the dumped catalog names.
        /// </summary>
        private static string ExtractRefName(string fileName)
        {
            if (fileName.EndsWith(LegacySkillSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return fileName.Substring(0, fileName.Length - LegacySkillSuffix.Length);
            }
            if (fileName.EndsWith(FileSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return fileName.Substring(0, fileName.Length - FileSuffix.Length);
            }
            return Path.GetFileNameWithoutExtension(fileName);
        }

        private SequenceSet ParseFile(string path)
        {
            XDocument doc = XDocument.Load(path);

            var group = doc.Element("SkillEditor")?.Element("SkillSequenceGroup");
            if (group == null) return null;

            var seqSet = new SequenceSet();

            foreach (var seqElem in group.Elements("Sequence"))
            {
                var seqData = new SequenceData
                {
                    Name = (string)seqElem.Attribute("Name"),
                    TimeRange = ParseFloat(seqElem.Attribute("TimeRange")),
                    Loop = ParseBool(seqElem.Attribute("Loop")),
                    PlaySpeed = ParseFloat(seqElem.Attribute("PlaySpeed"), 1f),
                    SkillID = ParseInt(seqElem.Attribute("SkillID")),
                    Layer = ParseInt(seqElem.Attribute("Layer")),
                    CatchUpDist = ParseFloat(seqElem.Attribute("CatchUpDist")),
                    DisablePhy = ParseBool(seqElem.Attribute("DisablePhy")),
                    DisableColWithWall = ParseBool(seqElem.Attribute("DisableColWithWall")),
                    EnableMoveSplineScale = ParseBool(seqElem.Attribute("EnableMoveSplineScale")),
                };

                var nextGroup = seqElem.Element("NextSequences");
                if (nextGroup != null)
                {
                    foreach (var next in nextGroup.Elements("NextSequence"))
                    {
                        string nextName = (string)next.Attribute("Name");
                        if (!string.IsNullOrEmpty(nextName))
                        {
                            seqData.NextSequences.Add(nextName);
                        }
                    }
                }

                var physicEventGroup = seqElem.Element("PhysicEvent");
                if (physicEventGroup != null)
                {
                    foreach (var hitCol in physicEventGroup.Elements("HitCol"))
                    {
                        seqData.PhysicEvents.Add(new PhysicEventData
                        {
                            Name = (string)hitCol.Attribute("Name"),
                            EventName = (string)hitCol.Attribute("EventName"),
                            Bone = (string)hitCol.Attribute("Bone"),
                            Time = ParseFloat(hitCol.Attribute("Time")),
                            EventType = (string)hitCol.Attribute("EventType"),
                            Firemode = (string)hitCol.Attribute("Firemode"),
                            AttackData = ParseInt(hitCol.Attribute("AttackData"), -1),
                            Enable = ParseBool(hitCol.Attribute("Enable")),
                            AnimOrder = ParseInt(hitCol.Attribute("AnimOrder")),
                            SlashDir = ParseInt(hitCol.Attribute("SlashDir")),
                            ShakeTime = ParseFloat(hitCol.Attribute("ShakeTime")),
                            ShakePeriod = ParseFloat(hitCol.Attribute("ShakePeriod")),
                            ShakeStrength = ParseFloat(hitCol.Attribute("ShakeStrength")),
                            ShakeAttenuation = ParseFloat(hitCol.Attribute("ShakeAttenuation")),
                            ShakeMaxDistance = ParseFloat(hitCol.Attribute("ShakeMaxDistance")),
                        });
                    }
                }

                seqData.HitColWindows.AddRange(BuildHitColWindows(seqData.PhysicEvents, seqData.TimeRange));

                var triggerEventGroup = seqElem.Element("TriggerEvent");
                if (triggerEventGroup != null)
                {
                    foreach (var trig in triggerEventGroup.Elements("Triggers"))
                    {
                        seqData.TriggerEvents.Add(new TriggerEventData
                        {
                            Name = (string)trig.Attribute("Name"),
                            EventName = (string)trig.Attribute("EventName"),
                            Bone = (string)trig.Attribute("Bone"),
                            Params = (string)trig.Attribute("Params"),
                            Time = ParseFloat(trig.Attribute("Time")),
                        });
                    }
                }

                var positionGroup = seqElem.Element("Position");
                if (positionGroup != null)
                {
                    seqData.Position.X = ParseFloatTrack(positionGroup.Element("Pos_X"));
                    seqData.Position.Y = ParseFloatTrack(positionGroup.Element("Pos_Y"));
                    seqData.Position.Z = ParseFloatTrack(positionGroup.Element("Pos_Z"));
                }

                var rotationGroup = seqElem.Element("Rotation");
                if (rotationGroup != null)
                {
                    seqData.Rotation.X = ParseFloatTrack(rotationGroup.Element("Rot_X"));
                    seqData.Rotation.Y = ParseFloatTrack(rotationGroup.Element("Rot_Y"));
                    seqData.Rotation.Z = ParseFloatTrack(rotationGroup.Element("Rot_Z"));
                }

                if (!string.IsNullOrEmpty(seqData.Name))
                {
                    seqSet.Sequences[seqData.Name] = seqData;
                }
            }

            return seqSet;
        }

        /// <summary>
        /// Pairs HitCol events whose Name ends with "Start"/"End" (sharing the same prefix)
        /// into damage windows. Unpaired events become zero-duration windows.
        /// The "Start" event holds the real AttackData; "End" typically has -1.
        /// </summary>
        private static IEnumerable<HitColWindow> BuildHitColWindows(List<PhysicEventData> events, float seqTimeRange)
        {
            var results = new List<HitColWindow>();
            var used = new HashSet<int>();

            for (int i = 0; i < events.Count; i++)
            {
                if (used.Contains(i)) continue;
                var ev = events[i];

                if (ev.IsAttackStart)
                {
                    string prefix = ev.Name.Substring(0, ev.Name.Length - "Start".Length);
                    int matchIdx = FindMatchingEnd(events, prefix, startIdx: i + 1, used);

                    var window = new HitColWindow
                    {
                        Label = prefix,
                        Firemode = ev.Firemode,
                        EventType = ev.EventType,
                        Bone = ev.Bone,
                        StartTime = ev.Time,
                        AttackData = ev.AttackData,
                        Enable = ev.Enable,
                    };

                    if (matchIdx >= 0)
                    {
                        window.EndTime = events[matchIdx].Time;
                        used.Add(matchIdx);
                    }
                    else
                    {
                        window.EndTime = seqTimeRange > 0 ? seqTimeRange : ev.Time;
                    }

                    used.Add(i);
                    results.Add(window);
                }
                else if (!ev.IsAttackEnd)
                {
                    // Standalone HitCol (neither Start nor End): instant window at Time.
                    results.Add(new HitColWindow
                    {
                        Label = ev.Name,
                        Firemode = ev.Firemode,
                        EventType = ev.EventType,
                        Bone = ev.Bone,
                        StartTime = ev.Time,
                        EndTime = ev.Time,
                        AttackData = ev.AttackData,
                        Enable = ev.Enable,
                    });
                    used.Add(i);
                }
                // Orphan *End events are skipped silently.
            }

            return results.OrderBy(w => w.StartTime);
        }

        private static int FindMatchingEnd(List<PhysicEventData> events, string prefix, int startIdx, HashSet<int> used)
        {
            string expected = prefix + "End";
            for (int j = startIdx; j < events.Count; j++)
            {
                if (used.Contains(j)) continue;
                var e = events[j];
                if (e.Name != null && e.Name.Equals(expected, StringComparison.OrdinalIgnoreCase))
                {
                    return j;
                }
            }
            return -1;
        }

        private static FloatTrack ParseFloatTrack(XElement trackElem)
        {
            var track = new FloatTrack();
            if (trackElem == null) return track;

            foreach (var keyElem in trackElem.Elements("Key"))
            {
                var key = new TrackKey
                {
                    Time = ParseFloat(keyElem.Attribute("time")),
                    Value = ParseFloat(keyElem.Attribute("value")),
                    Ds = ParseTangent((string)keyElem.Attribute("ds")),
                    Dd = ParseTangent((string)keyElem.Attribute("dd")),
                };
                track.Keys.Add(key);
            }

            return track;
        }

        private static float ParseTangent(string tangentStr)
        {
            if (string.IsNullOrEmpty(tangentStr)) return 0f;
            var parts = tangentStr.Split(',');
            if (parts.Length > 0 && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            return 0f;
        }

        private static float ParseFloat(XAttribute attr, float fallback = 0f)
        {
            if (attr == null) return fallback;
            return float.TryParse(attr.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float v) ? v : fallback;
        }

        private static int ParseInt(XAttribute attr, int fallback = 0)
        {
            if (attr == null) return fallback;
            return int.TryParse(attr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v) ? v : fallback;
        }

        private static bool ParseBool(XAttribute attr, bool fallback = false)
        {
            if (attr == null) return fallback;
            if (int.TryParse(attr.Value, out int asInt)) return asInt != 0;
            if (bool.TryParse(attr.Value, out bool asBool)) return asBool;
            return fallback;
        }
    }
}
