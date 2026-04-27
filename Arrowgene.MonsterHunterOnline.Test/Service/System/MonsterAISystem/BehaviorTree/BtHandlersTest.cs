using System.Collections.Generic;
using System.IO;
using Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree;
using Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree.Handlers;
using Xunit;
using Xunit.Abstractions;

namespace Arrowgene.MonsterHunterOnline.Test.Service.System.MonsterAISystem.BehaviorTree;

public class BtHandlersTest
{
    private const string BtRootDir =
        @"O:\jeux-backup\MONSTER HUNTER ONLINE\MHO_TOOL\extracted\scripts\ai\behaviortree\em001";

    private readonly ITestOutputHelper _out;

    public BtHandlersTest(ITestOutputHelper output)
    {
        _out = output;
    }

    private static (BtRunner runner, BtTree tree, BtContext ctx) BuildSyntheticRunner(
        string innerXml, IBtMonsterAdapter adapter = null, Blackboard bb = null)
    {
        string xml = "<Behavior Ver=\"1.01\">"
            + "<Node Type=\"Root\" Node_id=\"1\" Name=\"R\">"
            + "<Connector Identifier=\"GenericChildren\">" + innerXml
            + "</Connector></Node></Behavior>";
        BtNode root = BtParser.Parse(xml);
        BtTree tree = new BtTree(root, "(inline)");
        var registry = new BtHandlerRegistry();
        BtDefaultHandlers.RegisterAll(registry);
        var ctx = new BtContext(bb ?? new Blackboard(), null, registry) { Owner = adapter };
        return (new BtRunner(tree, ctx), tree, ctx);
    }

    [Fact]
    public void BlackBoardCheck_StringEquality()
    {
        var bb = new Blackboard();
        bb.LoadFromXml(global::System.Xml.Linq.XDocument.Parse(
            "<BlackBoard><Vars><Var Name=\"State\" Type=\"String\" Value=\"None\"/></Vars></BlackBoard>"));
        var (r, _, ctx) = BuildSyntheticRunner(
            "<Node Type=\"Condition\" Node_id=\"2\" Name=\"C\" Operation=\"BlackBoardCheck\" KeyName=\"State\" Value=\"Idle\"/>",
            bb: bb);

        Assert.Equal(BtStatus.Failure, r.Tick(0.033f));
        ctx.Blackboard.SetFromString("State", "Idle");
        Assert.Equal(BtStatus.Success, r.Tick(0.033f));
    }

    [Fact]
    public void BlackBoardCheck_NumericLessThan()
    {
        var bb = new Blackboard();
        bb.LoadFromXml(global::System.Xml.Linq.XDocument.Parse(
            "<BlackBoard><Vars><Var Name=\"AngryValue\" Type=\"Float\" Value=\"50\"/></Vars></BlackBoard>"));
        var (r, _, ctx) = BuildSyntheticRunner(
            "<Node Type=\"Condition\" Node_id=\"2\" Operation=\"BlackBoardCheck\" KeyName=\"AngryValue\" Value=\"100\" Operator=\"&lt;\"/>",
            bb: bb);

        Assert.Equal(BtStatus.Success, r.Tick(0.033f));
        ctx.Blackboard.Set("AngryValue", 150f);
        Assert.Equal(BtStatus.Failure, r.Tick(0.033f));
    }

    [Fact]
    public void CheckHealth_HpRatioBelowThreshold()
    {
        var bb = new Blackboard();
        bb.LoadFromXml(global::System.Xml.Linq.XDocument.Parse(
            "<BlackBoard><Vars>"
            + "<Var Name=\"Health\" Type=\"Int\" Value=\"260\"/>"
            + "<Var Name=\"MaxHealth\" Type=\"Int\" Value=\"2600\"/>"
            + "</Vars></BlackBoard>"));
        var (r, _, ctx) = BuildSyntheticRunner(
            "<Node Type=\"Condition\" Node_id=\"2\" Operation=\"CheckHealth\" Percentage=\"0.1\" OperationChar=\"&lt;\"/>",
            bb: bb);

        // 260/2600 = 0.1 → not strictly less than 0.1.
        Assert.Equal(BtStatus.Failure, r.Tick(0.033f));
        ctx.Blackboard.Set("Health", 100);
        // 100/2600 = 0.038... < 0.1.
        Assert.Equal(BtStatus.Success, r.Tick(0.033f));
    }

    [Fact]
    public void SetBlackBoard_WritesTypedValue()
    {
        var bb = new Blackboard();
        bb.LoadFromXml(global::System.Xml.Linq.XDocument.Parse(
            "<BlackBoard><Vars>"
            + "<Var Name=\"State\" Type=\"String\" Value=\"None\"/>"
            + "<Var Name=\"NeedEscape\" Type=\"Bool\"/>"
            + "</Vars></BlackBoard>"));
        var (r, _, ctx) = BuildSyntheticRunner(
            "<Node Type=\"Sequence\" Node_id=\"2\" Name=\"S\">"
            + "<Connector Identifier=\"GenericChildren\">"
            + "<Node Type=\"Action\" Node_id=\"3\" Operation=\"SetBlackBoard\" KeyName=\"State\" Value=\"Idle\"/>"
            + "<Node Type=\"Action\" Node_id=\"4\" Operation=\"SetBlackBoard\" KeyName=\"NeedEscape\" Value=\"True\"/>"
            + "</Connector></Node>",
            bb: bb);

        Assert.Equal(BtStatus.Success, r.Tick(0.033f));
        Assert.Equal("Idle", ctx.Blackboard.GetString("State"));
        Assert.True(ctx.Blackboard.GetBool("NeedEscape"));
    }

    [Fact]
    public void SetTime_TimeCheck_Roundtrip()
    {
        var bb = new Blackboard();
        bb.LoadFromXml(global::System.Xml.Linq.XDocument.Parse(
            "<BlackBoard><Vars><Var Name=\"CombatTimeRecord\" Type=\"Float\"/></Vars></BlackBoard>"));
        var (r, _, _) = BuildSyntheticRunner(
            "<Node Type=\"Sequence\" Node_id=\"2\">"
            + "<Connector Identifier=\"GenericChildren\">"
            + "<Node Type=\"Action\" Node_id=\"3\" Operation=\"SetTime\" RecordTimeBBName=\"CombatTimeRecord\"/>"
            + "<Node Type=\"Condition\" Node_id=\"4\" Operation=\"TimeCheck\" KeyName=\"CombatTimeRecord\" OperationChar=\"&gt;\" Value=\"1.0\"/>"
            + "</Connector></Node>",
            bb: bb);

        // First tick stamps t=0.5, then TimeCheck: elapsed = 0.5 - 0.5 = 0, NOT > 1.0.
        Assert.Equal(BtStatus.Failure, r.Tick(0.5f));
        // Second tick: SetTime stamps t=2.0, TimeCheck: elapsed = 0, still fails.
        // (SetTime in a Sequence re-stamps every entry — that's the CryEngine semantic.)
        Assert.Equal(BtStatus.Failure, r.Tick(1.5f));
    }

    [Fact]
    public void DelayTime_RunningThenSuccess()
    {
        var (r, _, _) = BuildSyntheticRunner(
            "<Node Type=\"Action\" Node_id=\"2\" Operation=\"DelayTime\" Value=\"0.5\"/>");

        // First tick stamps StartedAt = TotalSeconds *after* the delta is added,
        // so the first tick's elapsed is 0 regardless of delta. Need 4 ticks of 0.2s
        // (or fewer larger ticks) before elapsed crosses 0.5.
        Assert.Equal(BtStatus.Running, r.Tick(0.2f)); // t=0.2, started=0.2, elapsed=0
        Assert.Equal(BtStatus.Running, r.Tick(0.2f)); // t=0.4, elapsed=0.2
        Assert.Equal(BtStatus.Running, r.Tick(0.2f)); // t=0.6, elapsed=0.4
        Assert.Equal(BtStatus.Success, r.Tick(0.2f)); // t=0.8, elapsed=0.6 ≥ 0.5
    }

    [Fact]
    public void HandleDeath_FiresOnceAndSetsBlackboard()
    {
        var bb = new Blackboard();
        bb.LoadFromXml(global::System.Xml.Linq.XDocument.Parse(
            "<BlackBoard><Vars><Var Name=\"Dead\" Type=\"Bool\"/></Vars></BlackBoard>"));
        var fake = new FakeMonster();
        var (r, _, ctx) = BuildSyntheticRunner(
            "<Node Type=\"Action\" Node_id=\"2\" Operation=\"HandleDeath\"/>",
            adapter: fake, bb: bb);

        Assert.Equal(BtStatus.Success, r.Tick(0.033f));
        Assert.Equal(1, fake.HandleDeathCalls);
        Assert.True(ctx.Blackboard.GetBool("Dead"));

        Assert.Equal(BtStatus.Success, r.Tick(0.033f));
        Assert.Equal(1, fake.HandleDeathCalls); // idempotent
    }

    [Fact]
    public void AnimSequencePlay_StartsAndWaitsForCompletion()
    {
        var fake = new FakeMonster();
        var (r, _, _) = BuildSyntheticRunner(
            "<Node Type=\"Action\" Node_id=\"2\" Operation=\"AnimSequencePlay\" SequenceName=\"DragonDash\"/>",
            adapter: fake);

        fake.NextStartShouldSucceed = true;
        fake.PlayingSequence = null;

        // Tick 1: start succeeds, returns Running and remembers.
        Assert.Equal(BtStatus.Running, r.Tick(0.033f));
        Assert.Equal("DragonDash", fake.LastStartCalled);

        // Tick 2: still playing.
        fake.PlayingSequence = "DragonDash";
        Assert.Equal(BtStatus.Running, r.Tick(0.033f));

        // Tick 3: sequence ended.
        fake.PlayingSequence = null;
        Assert.Equal(BtStatus.Success, r.Tick(0.033f));
    }

    [Fact(Skip = "Local-only: requires extracted, decrypted BT files at a hardcoded path.")]
    public void Em001Master_TicksWithFullDefaultHandlers_NoMissingOps()
    {
        var loader = new BtTreeLoader(BtRootDir);
        BtTree master = loader.Load("em001.xml_decrypted.xml");
        var bb = new Blackboard();
        bb.LoadFromFile(Path.Combine(BtRootDir, "monsterblackboard001.xml_decrypted.xml"));

        var registry = new BtHandlerRegistry();
        BtDefaultHandlers.RegisterAll(registry);

        var ctx = new BtContext(bb, loader, registry) { Owner = new FakeMonster { IsAliveValue = true } };
        var runner = new BtRunner(master, ctx);

        // Tick a handful of times to drive the BT through several branches (loaders
        // resolve sub-trees lazily on first tick, so multiple ticks exercise more code).
        for (int i = 0; i < 5; i++) runner.Tick(0.033f);

        _out.WriteLine($"Missing ops after 5 ticks: [{string.Join(", ", runner.MissingOperations)}]");
        Assert.Empty(runner.MissingOperations);
    }

    private sealed class FakeMonster : IBtMonsterAdapter
    {
        public bool IsAliveValue { get; set; } = true;
        public bool IsAlive => IsAliveValue;
        public string PlayingSequence { get; set; }
        public bool NextStartShouldSucceed { get; set; }
        public string LastStartCalled { get; private set; }
        public int HandleDeathCalls { get; private set; }
        public List<string> StartCalls { get; } = new();

        public bool IsSequencePlaying() => !string.IsNullOrEmpty(PlayingSequence);
        public bool IsSequencePlaying(string sequenceName) => PlayingSequence == sequenceName;

        public bool TryStartSequence(string sequenceName)
        {
            LastStartCalled = sequenceName;
            StartCalls.Add(sequenceName);
            if (!NextStartShouldSucceed) return false;
            PlayingSequence = sequenceName;
            return true;
        }

        public void HandleDeath() { HandleDeathCalls++; }
    }
}
