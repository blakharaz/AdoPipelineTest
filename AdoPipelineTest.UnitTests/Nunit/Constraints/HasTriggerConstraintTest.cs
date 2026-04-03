using NUnit.Framework;
using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasTriggerConstraintTest
{
    [Test]
    public void ApplyTo_PipelineTriggers_HasBranches_ReturnsSuccess()
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = ["main"]
        };
        
        var constraint = new HasTriggerConstraint();
        var result = constraint.ApplyTo(triggers);
        
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineTriggers_EmptyBranches_ReturnsFailure()
    {
        var triggers = new PipelineTriggers
        {
            IncludedBranches = []
        };
        
        var constraint = new HasTriggerConstraint();
        var result = constraint.ApplyTo(triggers);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_NotPipelineTriggers_ReturnsFailure()
    {
        var constraint = new HasTriggerConstraint();
        var result = constraint.ApplyTo("not triggers");
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasTriggerConstraint();
        Assert.That(constraint.Description, Is.EqualTo("Pipeline has triggers configured"));
    }
}
