using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasJobConstraintTest
{
    [Test]
    public void ApplyTo_PipelineStage_HasJobByName_ReturnsSuccess()
    {
        var stage = new PipelineStage
        {
            Jobs = [new PipelineJob { Name = "Compile" }]
        };
        
        var constraint = new HasJobConstraint("Compile");
        var result = constraint.ApplyTo(stage);
        
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineStage_HasJobByDisplayName_ReturnsSuccess()
    {
        var stage = new PipelineStage
        {
            Jobs = [new PipelineJob { DisplayName = "Compile Job" }]
        };
        
        var constraint = new HasJobConstraint("Compile Job");
        var result = constraint.ApplyTo(stage);
        
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineStage_JobNotFound_ReturnsFailure()
    {
        var stage = new PipelineStage
        {
            Jobs = [new PipelineJob { Name = "Compile" }]
        };
        
        var constraint = new HasJobConstraint("Test");
        var result = constraint.ApplyTo(stage);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineStage_EmptyJobs_ReturnsFailure()
    {
        var stage = new PipelineStage { Jobs = [] };
        
        var constraint = new HasJobConstraint("Compile");
        var result = constraint.ApplyTo(stage);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_NotPipelineStage_ReturnsFailure()
    {
        var constraint = new HasJobConstraint("Compile");
        var result = constraint.ApplyTo("not a stage");
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasJobConstraint("Compile");
        Assert.That(constraint.Description, Is.EqualTo("Stage has job 'Compile'"));
    }
}
