using Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem.BehaviorTree;
using Xunit;

namespace Arrowgene.MonsterHunterOnline.Test.Service.System.MonsterAISystem.BehaviorTree;

public class MonsterAiFallbackBtTest
{
    [Fact]
    public void Builds_WithKnownNodes()
    {
        // The BuildRunner factory only needs an owner reference for handlers — the runner
        // itself can be constructed with a null MonsterAI as long as we don't tick it.
        // We just want to prove the embedded XML parses cleanly and registers all handlers.
        var bb = new Blackboard();
        BtRunner runner = MonsterAiFallbackBt.BuildRunner(null, bb);

        Assert.NotNull(runner);
        Assert.NotNull(runner.Tree);
        Assert.NotNull(runner.Tree.Root);
        Assert.Equal("FallbackRoot", runner.Tree.Root.Name);
        Assert.Single(runner.Tree.Root.Children); // Top selector
        Assert.Equal("Top", runner.Tree.Root.Children[0].Name);
    }

    [Fact]
    public void Tick_WithNullOwner_FallsThroughToFailure()
    {
        // Every fallback handler bails out with Failure when Owner isn't a MonsterAI,
        // so the entire tree must walk to completion and return Failure (HasTarget fails,
        // GoIdle fails, Selector exhausted). Confirms no crash, no MissingOperations.
        var bb = new Blackboard();
        BtRunner runner = MonsterAiFallbackBt.BuildRunner(null, bb);

        BtStatus status = runner.Tick(0.2f);

        Assert.Equal(BtStatus.Failure, status);
        Assert.Empty(runner.MissingOperations);
    }
}
