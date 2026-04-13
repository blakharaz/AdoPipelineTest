using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;
using Xunit;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasTriggerConstraintTest
{
    [Fact]
    public void ApplyTo_PipelineTriggers_HasBranches_ReturnsSuccess()
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = ["main"]
        };
        
        var constraint = new HasTriggerConstraint();
        var result = constraint.ApplyTo(triggers);
        
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_PipelineTriggers_EmptyBranches_ReturnsFailure()
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = []
        };
        
        var constraint = new HasTriggerConstraint();
        var result = constraint.ApplyTo(triggers);
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_NotPipelineTriggers_ReturnsFailure()
    {
        var constraint = new HasTriggerConstraint();
        var result = constraint.ApplyTo("not triggers");
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasTriggerConstraint();
        Assert.Equal("Pipeline has triggers configured", constraint.Description);
    }
}
