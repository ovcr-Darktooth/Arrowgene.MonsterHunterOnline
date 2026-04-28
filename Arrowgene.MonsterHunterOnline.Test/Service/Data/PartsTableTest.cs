using System.IO;
using System.Linq;
using Arrowgene.MonsterHunterOnline.Service.Data;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.MonsterHunterOnline.Test.Service.Data;

public class PartsTableTest
{
    private static readonly string StaticCsvDir = Path.Combine(AppContext.BaseDirectory, "Files", "Static");

    private readonly ITestOutputHelper _out;

    public PartsTableTest(ITestOutputHelper output)
    {
        _out = output;
    }

    [Fact]
    public void LoadAll_PopulatesTable()
    {
        Assert.True(Directory.Exists(StaticCsvDir), $"Expected directory missing: {StaticCsvDir}");

        var table = new PartsTable(StaticCsvDir);
        table.LoadAll();

        _out.WriteLine($"Parts={table.PartsCount} Defence={table.DefenceCount}");
        Assert.True(table.PartsCount > 1000, $"Expected many Parts rows, got {table.PartsCount}");
        Assert.True(table.DefenceCount > 1000, $"Expected many PartDefence rows, got {table.DefenceCount}");
    }

    [Fact]
    public void Em001_Has_ExpectedParts()
    {
        var table = new PartsTable(StaticCsvDir);
        table.LoadAll();

        var em001Parts = table.GetParts(60010);
        Assert.NotNull(em001Parts);

        // em001 (Bulldrome) should have at least these parts in Normal state.
        string[] expected = { "WholeBody", "Head", "Body", "LeftFoot", "LeftHand", "RightFoot", "RightHand", "Tail" };
        foreach (var partId in expected)
        {
            Assert.True(em001Parts.ContainsKey(partId), $"em001 should expose part '{partId}'");
        }

        _out.WriteLine("em001 parts: " + string.Join(", ", em001Parts.Keys));
    }

    [Fact]
    public void Em001_Head_BreakTiers_Match_CSV()
    {
        var table = new PartsTable(StaticCsvDir);
        table.LoadAll();

        Assert.True(table.TryGetPart(60010, "Head", "Normal", out var head));
        // Per CSV row: Head has tier1 DmgVal=4, tier2 DmgVal=6 with zeros on subsequent tiers.
        var tiers = head.BreakTiers;
        Assert.True(tiers.Count >= 2, $"Head should expose at least 2 break tiers, got {tiers.Count}");
        Assert.Equal(4f, tiers[0].DmgVal);
        Assert.Equal(6f, tiers[1].DmgVal);
        // Unbalance / Scar values from base columns
        Assert.Equal(3f, head.Unbalance);
        Assert.Equal(1f, head.UnbalanceMulti);
        Assert.Equal(2f, head.Scar);

        _out.WriteLine($"Head tiers: {string.Join(", ", tiers.Select(t => $"t{t.Tier}={t.DmgVal}"))}");
    }

    [Fact]
    public void Em001_Head_Defence_MatchesCsv()
    {
        var table = new PartsTable(StaticCsvDir);
        table.LoadAll();

        Assert.True(table.TryGetDefence(60010, "Head", "Normal", out var def));
        // CSV row: 60010,Head,Normal,0,0,1,0.9,0.9,0.7,0.25,0,0.1,0.1,0
        Assert.Equal(1f, def.Faint);
        Assert.Equal(0.9f, def.Cut);
        Assert.Equal(0.9f, def.Hammer);
        Assert.Equal(0.7f, def.Shoot);
        Assert.Equal(0.25f, def.Fire);
        Assert.Equal(0f, def.Water);
        Assert.Equal(0.1f, def.Electric);
        Assert.Equal(0.1f, def.Ice);
        Assert.Equal(0f, def.Dragon);
    }
}
