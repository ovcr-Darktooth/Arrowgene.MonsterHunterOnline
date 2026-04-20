using System.IO;
using System.Linq;
using Arrowgene.MonsterHunterOnline.Service.Data;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.MonsterHunterOnline.Test.Service.Data;

public class SequenceManagerTest
{
    private const string SequencesDir = @"O:\jeux-backup\MONSTER HUNTER ONLINE\MHO_TOOL\extracted\libs\sequencegroup";
    private const string DiagnosticsDir = @"O:\jeux-backup\MONSTER HUNTER ONLINE\rapports-claude\sequence_catalog";

    private readonly ITestOutputHelper _out;

    public SequenceManagerTest(ITestOutputHelper output)
    {
        _out = output;
    }

    [Fact(Skip = "Local-only: requires extracted game files at a hardcoded path.")]
    public void LoadAll_ShouldParseEveryFileWithoutError()
    {
        Assert.True(Directory.Exists(SequencesDir), $"Expected directory missing: {SequencesDir}");

        var manager = new SequenceManager(SequencesDir);
        var (loaded, failed) = manager.LoadAll();

        _out.WriteLine($"Loaded: {loaded}, Failed: {failed}");
        Assert.Equal(0, failed);
        Assert.True(loaded > 0);
    }

    [Fact(Skip = "Local-only: requires extracted game files at a hardcoded path.")]
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

    [Fact(Skip = "Local-only: requires extracted game files at a hardcoded path.")]
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
