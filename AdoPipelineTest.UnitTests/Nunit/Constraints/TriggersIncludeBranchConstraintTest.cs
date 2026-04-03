using NUnit.Framework;
using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class TriggersIncludeBranchConstraintTest
{
    [Test]
    public void ApplyTo_PipelineTriggers_IncludesBranch_ReturnsSuccess()
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = ["main", "develop", "feature/*"]
        };
        
        var constraint = new TriggersIncludeBranchConstraint("main");
        var result = constraint.ApplyTo(triggers);
        
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineTriggers_DoesNotIncludeBranch_ReturnsFailure()
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = ["main", "develop"]
        };
        
        var constraint = new TriggersIncludeBranchConstraint("feature/test");
        var result = constraint.ApplyTo(triggers);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineTriggers_EmptyBranches_ReturnsFailure()
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = []
        };
        
        var constraint = new TriggersIncludeBranchConstraint("main");
        var result = constraint.ApplyTo(triggers);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_NotPipelineTriggers_ReturnsFailure()
    {
        var constraint = new TriggersIncludeBranchConstraint("main");
        var result = constraint.ApplyTo("not a pipeline triggers object");
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new TriggersIncludeBranchConstraint("main");
        Assert.That(constraint.Description, Is.EqualTo("Triggers include branch main"));
    }

    [TestCase(new[] { "*" }, "*", ExpectedResult = true)]
    [TestCase(new[] { "main" }, "main", ExpectedResult = true)]
    [TestCase(new[] { "main" }, "other", ExpectedResult = false)]
    [TestCase(new[] { "feature/*" }, "feature/*", ExpectedResult = true)]
    [TestCase(new[] { "release/*" }, "release/*", ExpectedResult = true)]
    public bool ApplyTo_WithExactBranchMatch_HandlesCorrectly(string[] branches, string branchToCheck)
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = branches.ToList<string?>()
        };
        
        var constraint = new TriggersIncludeBranchConstraint(branchToCheck);
        var result = constraint.ApplyTo(triggers);
        
        return result.IsSuccess;
    }
}
