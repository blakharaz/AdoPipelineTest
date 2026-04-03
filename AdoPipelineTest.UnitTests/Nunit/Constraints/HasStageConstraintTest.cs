using NUnit.Framework;
using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasStageConstraintTest
{
    [Test]
    public void ApplyTo_PipelineTestResult_HasStageByName_ReturnsSuccess()
    {
        var result = new PipelineTestResult
        {
            Stages = [new PipelineStage { Name = "Build" }]
        };
        
        var constraint = new HasStageConstraint("Build");
        var res = constraint.ApplyTo(result);
        
        Assert.That(res.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineTestResult_HasStageByDisplayName_ReturnsSuccess()
    {
        var result = new PipelineTestResult
        {
            Stages = [new PipelineStage { DisplayName = "Build Stage" }]
        };
        
        var constraint = new HasStageConstraint("Build Stage");
        var res = constraint.ApplyTo(result);
        
        Assert.That(res.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineTestResult_StageNotFound_ReturnsFailure()
    {
        var result = new PipelineTestResult
        {
            Stages = [new PipelineStage { Name = "Build" }]
        };
        
        var constraint = new HasStageConstraint("Deploy");
        var res = constraint.ApplyTo(result);
        
        Assert.That(res.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineTestResult_EmptyStages_ReturnsFailure()
    {
        var result = new PipelineTestResult { Stages = [] };
        
        var constraint = new HasStageConstraint("Build");
        var res = constraint.ApplyTo(result);
        
        Assert.That(res.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_NotPipelineTestResult_ReturnsFailure()
    {
        var constraint = new HasStageConstraint("Build");
        var res = constraint.ApplyTo("not a result");
        
        Assert.That(res.IsSuccess, Is.False);
    }

    [Test]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasStageConstraint("Build");
        Assert.That(constraint.Description, Is.EqualTo("Pipeline has stage 'Build'"));
    }
}
