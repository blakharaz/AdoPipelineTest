using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;
using Xunit;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class TriggersIncludeBranchConstraintTest
{
    [Fact]
    public void ApplyTo_PipelineTriggers_IncludesBranch_ReturnsSuccess()
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = ["main", "develop", "feature/*"]
        };
        
        var constraint = new TriggersIncludeBranchConstraint("main");
        var result = constraint.ApplyTo(triggers);
        
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_PipelineTriggers_DoesNotIncludeBranch_ReturnsFailure()
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = ["main", "develop"]
        };
        
        var constraint = new TriggersIncludeBranchConstraint("feature/test");
        var result = constraint.ApplyTo(triggers);
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_PipelineTriggers_EmptyBranches_ReturnsFailure()
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = []
        };
        
        var constraint = new TriggersIncludeBranchConstraint("main");
        var result = constraint.ApplyTo(triggers);
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_NotPipelineTriggers_ReturnsFailure()
    {
        var constraint = new TriggersIncludeBranchConstraint("main");
        var result = constraint.ApplyTo("not a pipeline triggers object");
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new TriggersIncludeBranchConstraint("main");
        Assert.Equal("Triggers include branch main", constraint.Description);
    }

    [Theory]
    [InlineData(new[] { "*" }, "*", true)]
    [InlineData(new[] { "main" }, "main", true)]
    [InlineData(new[] { "main" }, "other", false)]
    [InlineData(new[] { "feature/*" }, "feature/*", true)]
    [InlineData(new[] { "release/*" }, "release/*", true)]
    public void ApplyTo_WithExactBranchMatch_HandlesCorrectly(string[] branches, string branchToCheck, bool expected)
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = branches.ToList<string?>()
        };
        
        var constraint = new TriggersIncludeBranchConstraint(branchToCheck);
        var result = constraint.ApplyTo(triggers);
        
        Assert.Equal(expected, result.IsSuccess);
    }
}
