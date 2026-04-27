using System.IO;
using System.Linq;
using Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.MonsterHunterOnline.Test.Service.System.MonsterAISystem.BehaviorTree;

public class BtParserTest
{
    private const string BtRootDir =
        @"O:\jeux-backup\MONSTER HUNTER ONLINE\MHO_TOOL\extracted\scripts\ai\behaviortree\em001";

    private const string Em001Master = "em001.xml_decrypted.xml";

    private readonly ITestOutputHelper _out;

    public BtParserTest(ITestOutputHelper output)
    {
        _out = output;
    }

    [Fact(Skip = "Local-only: requires extracted, decrypted BT files at a hardcoded path.")]
    public void Parse_Em001Master_HasRootSelectorWithExpectedChildren()
    {
        string path = Path.Combine(BtRootDir, Em001Master);
        Assert.True(File.Exists(path), $"Expected BT file missing: {path}");

        BtNode root = BtParser.ParseFile(path);

        Assert.IsType<BtRoot>(root);
        Assert.Equal("Root_node", root.Name);
        Assert.Single(root.Children);

        BtNode rootChild = root.Children[0];
        BtSelector selector = Assert.IsType<BtSelector>(rootChild);
        Assert.Equal("General", selector.SelectorType);

        // Top-level priority list: Dead → Sense → Escape → CombatEsape → Attack → Idle.
        string[] expected = { "DeadCondition", "Em001Sense", "Escape", "CombatEsape", "Em001Attack", "Idle" };
        string[] actual = selector.Children.Select(c => c.Name).ToArray();
        _out.WriteLine("Selector0 children: " + string.Join(", ", actual));
        foreach (string name in expected)
            Assert.Contains(name, actual);

        // Em001Sense and Em001Attack are Reference leaves.
        BtReference sense = Assert.IsType<BtReference>(selector.Children.First(c => c.Name == "Em001Sense"));
        Assert.Equal("Em001Sense.xml", sense.Reference);

        BtReference attack = Assert.IsType<BtReference>(selector.Children.First(c => c.Name == "Em001Attack"));
        Assert.Equal("Em001Attack.xml", attack.Reference);

        // The Escape sub-tree carries a Condition with Operation=CheckHealth Percentage=0.1.
        BtNode escape = selector.Children.First(c => c.Name == "Escape");
        BtCondition health = Assert.IsType<BtCondition>(escape.Children.First(c => c.Name == "CheckHealth"));
        Assert.Equal("CheckHealth", health.Operation);
        Assert.Equal("0.1", health.GetAttr("Percentage"));
        Assert.Equal("<", health.GetAttr("OperationChar"));
    }

    [Fact(Skip = "Local-only: requires extracted, decrypted BT files at a hardcoded path.")]
    public void Loader_Resolves_DottedSubNodeSelector()
    {
        var loader = new BtTreeLoader(BtRootDir);
        BtTree master = loader.Load(Em001Master);

        // "Em001Sleep.Root_node.Sleep" — file Em001Sleep.xml + selector Root_node.Sleep.
        BtNode escape = ((BtSelector)master.Root.Children[0]).Children.First(c => c.Name == "Escape");
        BtReference sleepRef = (BtReference)escape.Children.First(c => c.Name == "Sleep");
        _out.WriteLine($"Sleep ref = '{sleepRef.Reference}'");

        BtTree sleep = loader.Resolve(sleepRef.Reference, master, out string sub);
        Assert.NotNull(sleep);
        Assert.Equal("Root_node.Sleep", sub);
    }

    [Fact(Skip = "Local-only: requires extracted, decrypted BT files at a hardcoded path.")]
    public void Loader_Resolves_RelativePath()
    {
        var loader = new BtTreeLoader(BtRootDir);
        BtTree master = loader.Load(Em001Master);

        // ".\Em001ChangeArea.xml" — same folder as parent.
        BtNode combatEscape = ((BtSelector)master.Root.Children[0]).Children.First(c => c.Name == "CombatEsape");
        BtReference changeArea = (BtReference)combatEscape.Children.First(c => c.Name == "ChangeArea");
        Assert.Equal(@".\Em001ChangeArea.xml", changeArea.Reference);

        BtTree resolved = loader.Resolve(changeArea.Reference, master, out string sub);
        Assert.NotNull(resolved);
        Assert.Null(sub);
    }
}
