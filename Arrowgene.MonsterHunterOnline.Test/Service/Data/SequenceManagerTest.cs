using System;
using System.IO;
using System.Linq;
using Arrowgene.MonsterHunterOnline.Service.Data;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.MonsterHunterOnline.Test.Service.Data;

public class SequenceManagerTest
{
    private static readonly string SequencesDir = Path.Combine(AppContext.BaseDirectory, "Files", "Static", "Sequences");
    private const string DiagnosticsDir = @"O:\jeux-backup\MONSTER HUNTER ONLINE\rapports-claude\sequence_catalog";

    private readonly ITestOutputHelper _out;

    public SequenceManagerTest(ITestOutputHelper output)
    {
        _out = output;
    }

    [Fact]
    public void LoadAll_ShouldParseEveryFileWithoutError()
    {
        Assert.True(Directory.Exists(SequencesDir), $"Expected directory missing: {SequencesDir}");

        var manager = new SequenceManager(SequencesDir);
        var (loaded, failed) = manager.LoadAll();

        _out.WriteLine($"Loaded: {loaded}, Failed: {failed}");
        Assert.Equal(0, failed);
        Assert.True(loaded > 0);
    }

    [Fact]
    public void DragonDash_ShouldHavePairedHitColWindows()
    {
        var manager = new SequenceManager(SequencesDir);
        var set = manager.GetOrLoad("em001");
        Assert.NotNull(set);
        Assert.True(set.Sequences.TryGetValue("DragonDash", out var seq), "em001 must have a DragonDash sequence");

        Assert.Equal(4, seq.HitColWindows.Count);

        foreach (var w in seq.HitColWindows)
        {
            _out.WriteLine($"  {w.Label}: [{w.StartTime:F3}s -> {w.EndTime:F3}s] AttackData={w.AttackData} Firemode={w.Firemode}");
            Assert.True(w.EndTime > w.StartTime, $"Window {w.Label} is not a proper interval");
            Assert.True(w.AttackData > 0, $"Window {w.Label} missing AttackData");
            Assert.Equal("DragonDash", w.Firemode);
        }
    }

    [Fact]
    public void DragonDash_ShouldHaveForwardRootMotion()
    {
        var manager = new SequenceManager(SequencesDir);
        var set = manager.GetOrLoad("em001");
        Assert.NotNull(set);
        Assert.True(set.Sequences.TryGetValue("DragonDash", out var seq));
        Assert.True(seq.Position.HasAny, "DragonDash must have Position curves");

        var p0 = seq.Position.Sample(0f);
        var pMid = seq.Position.Sample(seq.TimeRange * 0.5f);
        var pEnd = seq.Position.Sample(seq.TimeRange);

        _out.WriteLine($"DragonDash position: t=0 ({p0.x:F3},{p0.y:F3},{p0.z:F3})  t=mid ({pMid.x:F3},{pMid.y:F3},{pMid.z:F3})  t=end ({pEnd.x:F3},{pEnd.y:F3},{pEnd.z:F3})");

        // Cumulative 2D distance traveled in local frame must be > 0 (it's a dash)
        float dx = pEnd.x - p0.x;
        float dy = pEnd.y - p0.y;
        float dist = MathF.Sqrt(dx * dx + dy * dy);
        _out.WriteLine($"DragonDash net 2D displacement: {dist:F3} units");
        Assert.True(dist > 0.5f, $"Expected substantial root-motion displacement, got {dist:F3}");
    }

    [Fact(Skip = "Local-only: requires extracted game files at a hardcoded path.")]
    public void DumpCatalog_AllMonsters()
    {
        var manager = new SequenceManager(SequencesDir);
        var (loaded, failed) = manager.LoadAll();
        _out.WriteLine($"Loaded: {loaded}, Failed: {failed}");

        SequenceDiagnostics.DumpAll(manager, DiagnosticsDir);

        string summary = Path.Combine(DiagnosticsDir, "_summary.md");
        Assert.True(File.Exists(summary));
        _out.WriteLine($"Summary: {summary}");

        foreach (var set in manager.All)
        {
            Assert.True(set.Sequences.Count > 0, $"Empty set: {set.RefName}");
        }
    }

    [Fact]
    public void Em008_ShouldHaveLemonVariants()
    {
        var manager = new SequenceManager(SequencesDir);
        var set = manager.GetOrLoad("em008");
        Assert.NotNull(set);

        var lemon = set.Sequences.Keys
            .Where(k => k.EndsWith("_Lemon"))
            .ToList();

        _out.WriteLine($"em008 _Lemon variants ({lemon.Count}): {string.Join(", ", lemon)}");
        Assert.True(lemon.Count > 0, "em008 is expected to have _Lemon rage variants");
    }
}
