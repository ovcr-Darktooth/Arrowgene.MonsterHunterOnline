using System.IO;
using Arrowgene.MonsterHunterOnline.Service.Data;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.MonsterHunterOnline.Test.Service.Data;

public class AttackDataTableTest
{
    private static readonly string StaticCsvDir = Path.Combine(AppContext.BaseDirectory, "Files", "Static");

    private readonly ITestOutputHelper _out;

    public AttackDataTableTest(ITestOutputHelper output)
    {
        _out = output;
    }

    [Fact]
    public void LoadAll_ShouldPopulateTable()
    {
        Assert.True(Directory.Exists(StaticCsvDir), $"Expected directory missing: {StaticCsvDir}");

        var table = new AttackDataTable(StaticCsvDir);
        int count = table.LoadAll();

        _out.WriteLine($"Loaded {count} AttackData entries");
        Assert.True(count > 100, $"Expected many entries, got {count}");
    }

    [Fact]
    public void DragonDash_em001_ShouldHaveExpectedDamage()
    {
        var table = new AttackDataTable(StaticCsvDir);
        table.LoadAll();

        // em001 DragonDash claw windows (LeftClaw / RightClaw) → AttackData 20271
        Assert.True(table.TryGet(20271, out var claw), "20271 (Template_Attack_20) must exist");
        _out.WriteLine($"20271: {claw.AttackName} '{claw.Note}' DamagePower={claw.DamagePower} Piyo={claw.Piyo} Lv={claw.DamageLevel}");
        Assert.Equal("Template_Attack_20", claw.AttackName);
        Assert.Equal(15, claw.DamagePower);
        Assert.Equal(15, table.GetDamage(20271));

        // em001 DragonDash body/head windows → AttackData 20269
        Assert.True(table.TryGet(20269, out var body), "20269 (Template_Attack_10) must exist");
        _out.WriteLine($"20269: {body.AttackName} '{body.Note}' DamagePower={body.DamagePower} Piyo={body.Piyo} Lv={body.DamageLevel}");
        Assert.Equal("Template_Attack_10", body.AttackName);
        Assert.Equal(10, body.DamagePower);
        Assert.Equal(10, table.GetDamage(20269));
    }

    [Fact]
    public void GetDamage_UnknownId_ReturnsZero()
    {
        var table = new AttackDataTable(StaticCsvDir);
        table.LoadAll();
        Assert.Equal(0, table.GetDamage(int.MaxValue));
    }
}
