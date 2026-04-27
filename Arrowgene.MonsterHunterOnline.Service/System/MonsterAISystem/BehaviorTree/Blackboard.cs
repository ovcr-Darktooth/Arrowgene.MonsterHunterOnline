using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree
{
    /// <summary>
    /// CryEngine-style typed key/value store consumed by Action/Condition handlers
    /// (e.g. <c>BlackBoardCheck KeyName=Dead Value=True</c>, <c>SetBlackBoard</c>,
    /// <c>CheckHealth Percentage=0.1</c> — though HP/Position live alongside in
    /// monster-specific shortcuts).
    ///
    /// Schema is loaded from <c>monsterblackboard001.xml_decrypted.xml</c>:
    /// <c>&lt;Var Name="..." Type="..." Value="..."/&gt;</c>. Supported types match the
    /// game's: Bool, Int, Uint32, Float, String, Vec3, Quat. Vec3/Quat are stored as
    /// raw strings — handlers that need them parse on demand.
    /// </summary>
    public sealed class Blackboard
    {
        private readonly Dictionary<string, object> _values = new(StringComparer.Ordinal);
        private readonly Dictionary<string, string> _types = new(StringComparer.Ordinal);

        public IReadOnlyDictionary<string, string> Types => _types;

        /// <summary>Loads the schema (and any default <c>Value=</c>) from a blackboard XML file.</summary>
        public void LoadFromFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);
            LoadFromXml(XDocument.Load(path));
        }

        public void LoadFromXml(XDocument doc)
        {
            XElement root = doc.Root;
            if (root == null || root.Name.LocalName != "BlackBoard")
                throw new InvalidDataException($"Expected <BlackBoard> root, got <{doc.Root?.Name}>");

            foreach (XElement var in root.Descendants("Var"))
            {
                string name = var.Attribute("Name")?.Value;
                string type = var.Attribute("Type")?.Value;
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type)) continue;

                _types[name] = type;
                string raw = var.Attribute("Value")?.Value;
                _values[name] = ParseValue(type, raw);
            }
        }

        public bool Has(string key) => _values.ContainsKey(key);
        public string TypeOf(string key) => _types.TryGetValue(key, out string t) ? t : null;
        public object GetRaw(string key) => _values.TryGetValue(key, out object v) ? v : null;

        public bool GetBool(string key) => _values.TryGetValue(key, out object v) && v is bool b && b;
        public int GetInt(string key) => _values.TryGetValue(key, out object v) && v is int i ? i : 0;
        public uint GetUint(string key) => _values.TryGetValue(key, out object v) && v is uint u ? u : 0u;
        public float GetFloat(string key) => _values.TryGetValue(key, out object v) && v is float f ? f : 0f;
        public string GetString(string key) => _values.TryGetValue(key, out object v) ? v as string : null;

        public void Set(string key, object value)
        {
            _values[key] = value;
        }

        /// <summary>
        /// CryEngine BlackBoardCheck encodes the comparison value as a string regardless of
        /// the var's declared type (e.g. <c>Value="True"</c>, <c>Value="Idle"</c>,
        /// <c>Value="0.1"</c>). This converts the BB-stored value to its canonical string
        /// for that comparison.
        /// </summary>
        public string GetAsString(string key)
        {
            if (!_values.TryGetValue(key, out object v) || v == null) return null;
            return v switch
            {
                bool b => b ? "True" : "False",
                float f => f.ToString(CultureInfo.InvariantCulture),
                int i => i.ToString(CultureInfo.InvariantCulture),
                uint u => u.ToString(CultureInfo.InvariantCulture),
                string s => s,
                _ => v.ToString()
            };
        }

        /// <summary>Sets a value, reusing the declared type if known (so SetBlackBoard with
        /// a string source coerces back to the right runtime type).</summary>
        public void SetFromString(string key, string raw)
        {
            string type = _types.TryGetValue(key, out string t) ? t : "String";
            _values[key] = ParseValue(type, raw);
        }

        private static object ParseValue(string type, string raw)
        {
            switch (type)
            {
                case "Bool":
                    if (string.IsNullOrEmpty(raw)) return false;
                    return string.Equals(raw, "True", StringComparison.OrdinalIgnoreCase);
                case "Int":
                    return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i) ? i : 0;
                case "Uint32":
                    return uint.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint u) ? u : 0u;
                case "Float":
                    return float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float f) ? f : 0f;
                case "String":
                    return raw ?? string.Empty;
                case "Vec3":
                case "Quat":
                    return raw ?? string.Empty; // raw string; parse on demand
                default:
                    return raw;
            }
        }

    }
}
