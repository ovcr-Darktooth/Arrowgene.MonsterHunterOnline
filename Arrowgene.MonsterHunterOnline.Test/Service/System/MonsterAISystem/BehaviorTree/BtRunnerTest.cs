using System.IO;
using Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.MonsterHunterOnline.Test.Service.System.MonsterAISystem.BehaviorTree;

public class BtRunnerTest
{
    private const string BtRootDir =
        @"O:\jeux-backup\MONSTER HUNTER ONLINE\MHO_TOOL\extracted\scripts\ai\behaviortree\em001";

    private readonly ITestOutputHelper _out;

    public BtRunnerTest(ITestOutputHelper output)
    {
        _out = output;
    }

    [Fact(Skip = "Local-only: requires extracted, decrypted BT files at a hardcoded path.")]
    public void Blackboard_LoadFromEm001Schema_ParsesTypedDefaults()
    {
        var bb = new Blackboard();
        bb.LoadFromFile(Path.Combine(BtRootDir, "monsterblackboard001.xml_decrypted.xml"));

        // Typed default values from the schema.
        Assert.Equal(2600, bb.GetInt("Health"));
        Assert.Equal(2600, bb.GetInt("MaxHealth"));
        Assert.Equal(335f, bb.GetFloat("UnbalanceValueHead"));
        Assert.Equal(818f, bb.GetFloat("FallValueLeftFoot"));
        Assert.Equal(1200f, bb.GetFloat("PartBrokenValueHead"));
        Assert.Equal("None", bb.GetString("State"));

        // Bool default missing → false.
        Assert.False(bb.GetBool("Dead"));
        Assert.False(bb.GetBool("FallDown"));

        // BlackBoardCheck encodes everything as strings — verify round-trip.
        Assert.Equal("False", bb.GetAsString("Dead"));
        bb.SetFromString("Dead", "True");
        Assert.True(bb.GetBool("Dead"));
        Assert.Equal("True", bb.GetAsString("Dead"));

        // SetBlackBoard with State=Idle.
        bb.SetFromString("State", "Idle");
        Assert.Equal("Idle", bb.GetString("State"));
        Assert.Equal("Idle", bb.GetAsString("State"));
    }

    [Fact(Skip = "Local-only: requires extracted, decrypted BT files at a hardcoded path.")]
    public void Runner_TicksEm001Master_FailsCleanlyWithoutHandlers()
    {
        // With no handlers registered every Action/Condition returns Failure, so the
        // priority-list Selector exhausts every branch and itself returns Failure.
        // What we're actually asserting is: the runner walks the whole tree without
        // throwing, including descending into Reference sub-trees and Filter decorators.
        var loader = new BtTreeLoader(BtRootDir);
        BtTree master = loader.Load("em001.xml_decrypted.xml");

        var bb = new Blackboard();
        bb.LoadFromFile(Path.Combine(BtRootDir, "monsterblackboard001.xml_decrypted.xml"));

        var registry = new BtHandlerRegistry();
        var ctx = new BtContext(bb, loader, registry);
        var runner = new BtRunner(master, ctx);

        BtStatus s = runner.Tick(0.033f);
        _out.WriteLine($"Tick → {s}");
        _out.WriteLine($"Missing ops ({runner.MissingOperations.Count}): "
            + string.Join(", ", runner.MissingOperations));

        Assert.Equal(BtStatus.Failure, s);
        // Sanity: real ops were referenced (so 6.4 has a non-empty target list).
        Assert.Contains("BlackBoardCheck", runner.MissingOperations);
        Assert.Contains("CheckHealth", runner.MissingOperations);
    }

    [Fact]
    public void Runner_RespectsRunningChildIndexAcrossTicks()
    {
        // Tiny synthetic tree: Sequence[Cond,Cond] — first cond returns Running on tick 1,
        // Success on tick 2. Sequence must resume at the same child, not restart from 0.
        var bb = new Blackboard();
        var registry = new BtHandlerRegistry();

        int firstCondTickCount = 0;
        int secondCondTickCount = 0;

        registry.RegisterCondition("FirstCond", new LambdaHandler((node, c) =>
        {
            firstCondTickCount++;
            return firstCondTickCount == 1 ? BtStatus.Running : BtStatus.Success;
        }));
        registry.RegisterCondition("SecondCond", new LambdaHandler((node, c) =>
        {
            secondCondTickCount++;
            return BtStatus.Success;
        }));

        const string xml = "<Behavior Ver=\"1.01\">"
            + "<Node Type=\"Root\" Node_id=\"1\" Name=\"R\">"
            + "<Connector Identifier=\"GenericChildren\">"
            + "<Node Type=\"Sequence\" Node_id=\"2\" Name=\"Seq\">"
            + "<Connector Identifier=\"GenericChildren\">"
            + "<Node Type=\"Condition\" Node_id=\"3\" Name=\"A\" Operation=\"FirstCond\"/>"
            + "<Node Type=\"Condition\" Node_id=\"4\" Name=\"B\" Operation=\"SecondCond\"/>"
            + "</Connector></Node></Connector></Node></Behavior>";

        BtNode root = BtParser.Parse(xml);
        BtTree tree = new BtTree(root, "(inline)");
        var ctx = new BtContext(bb, null, registry);
        var runner = new BtRunner(tree, ctx);

        Assert.Equal(BtStatus.Running, runner.Tick(0.033f));
        Assert.Equal(1, firstCondTickCount);
        Assert.Equal(0, secondCondTickCount);

        Assert.Equal(BtStatus.Success, runner.Tick(0.033f));
        Assert.Equal(2, firstCondTickCount);   // resumed at the running child
        Assert.Equal(1, secondCondTickCount);  // then advanced to next
    }

    private sealed class LambdaHandler : IBtHandler
    {
        private readonly global::System.Func<BtNode, BtContext, BtStatus> _fn;
        public LambdaHandler(global::System.Func<BtNode, BtContext, BtStatus> fn) { _fn = fn; }
        public BtStatus Tick(BtNode node, BtContext ctx) => _fn(node, ctx);
    }
}
