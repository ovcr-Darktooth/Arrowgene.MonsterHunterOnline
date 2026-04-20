using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Arrowgene.Logging;

namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    /// <summary>
    /// Produces Markdown catalogs from parsed SequenceSets — validates the parser
    /// and gives a human-readable overview of each monster's action set.
    /// </summary>
    public static class SequenceDiagnostics
    {
        private static readonly ServiceLogger Logger = LogProvider.Logger<ServiceLogger>(typeof(SequenceDiagnostics));

        public static void DumpAll(SequenceManager manager, string outputDir)
        {
            Directory.CreateDirectory(outputDir);

            var summary = new StringBuilder();
            summary.AppendLine("# Sequence Library — Summary");
            summary.AppendLine();
            summary.AppendLine("| RefName | Sequences | HitWindows | Triggers | TopAttacks |");
            summary.AppendLine("|---|---:|---:|---:|---|");

            foreach (var set in manager.All.OrderBy(s => s.RefName, StringComparer.OrdinalIgnoreCase))
            {
                int seqCount = set.Sequences.Count;
                int winCount = set.Sequences.Values.Sum(s => s.HitColWindows.Count);
                int trigCount = set.Sequences.Values.Sum(s => s.TriggerEvents.Count);

                string topAttacks = string.Join(", ", set.Sequences.Values
                    .Where(s => s.HitColWindows.Any(w => w.AttackData > 0))
                    .OrderByDescending(s => s.HitColWindows.Count)
                    .Take(5)
                    .Select(s => s.Name));

                summary.AppendLine($"| {set.RefName} | {seqCount} | {winCount} | {trigCount} | {topAttacks} |");

                string monsterPath = Path.Combine(outputDir, $"{set.RefName}.md");
                File.WriteAllText(monsterPath, DumpMonster(set));
            }

            File.WriteAllText(Path.Combine(outputDir, "_summary.md"), summary.ToString());
            Logger.Info($"Sequence diagnostics written to {outputDir}");
        }

        public static string DumpMonster(SequenceSet set)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# {set.RefName} — sequence catalog");
            sb.AppendLine();
            sb.AppendLine($"Source: `{set.SourceFile}`  ");
            sb.AppendLine($"Sequences: **{set.Sequences.Count}**");
            sb.AppendLine();

            foreach (var seq in set.Sequences.Values.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase))
            {
                sb.AppendLine($"## {seq.Name}");
                sb.Append($"- duration: `{F(seq.TimeRange)}s`");
                if (seq.Loop) sb.Append("  |  loop");
                if (seq.PlaySpeed != 1f) sb.Append($"  |  playSpeed: `{F(seq.PlaySpeed)}`");
                if (seq.SkillID != 0) sb.Append($"  |  skillId: `{seq.SkillID}`");
                if (seq.Layer != 0) sb.Append($"  |  layer: `{seq.Layer}`");
                if (seq.CatchUpDist != 0) sb.Append($"  |  catchUp: `{F(seq.CatchUpDist)}`");
                sb.AppendLine();

                if (seq.NextSequences.Count > 0)
                {
                    sb.AppendLine($"- next: {string.Join(", ", seq.NextSequences.Select(n => $"`{n}`"))}");
                }

                if (seq.HitColWindows.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("| Window | Start | End | AttackData | Firemode | Bone |");
                    sb.AppendLine("|---|---:|---:|---:|---|---|");
                    foreach (var w in seq.HitColWindows)
                    {
                        string atk = w.AttackData > 0 ? w.AttackData.ToString() : "-";
                        sb.AppendLine($"| {w.Label} | {F(w.StartTime)} | {F(w.EndTime)} | {atk} | {w.Firemode ?? ""} | {w.Bone ?? ""} |");
                    }
                }

                if (seq.TriggerEvents.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("Triggers:");
                    foreach (var t in seq.TriggerEvents)
                    {
                        sb.AppendLine($"- `{t.Name}` @ {F(t.Time)}s — event:`{t.EventName}` params:`{t.Params}`");
                    }
                }

                if (seq.Position.HasAny)
                {
                    sb.AppendLine();
                    sb.Append("Position keys: ");
                    sb.Append($"X={seq.Position.X.Keys.Count}, Y={seq.Position.Y.Keys.Count}, Z={seq.Position.Z.Keys.Count}");
                    sb.AppendLine();
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string F(float v) => v.ToString("0.###", CultureInfo.InvariantCulture);
    }
}
