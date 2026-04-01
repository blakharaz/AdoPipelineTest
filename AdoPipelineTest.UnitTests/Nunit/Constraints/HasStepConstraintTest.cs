using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Nunit.Constraints;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasStepConstraintTest
{
    [Test]
    public void ApplyTo_PipelineJob_HasStep_ReturnsSuccess()
    {
        var job = new PipelineJob
        {
            Steps = [new TaskStep { TaskName = "DotNetCoreCLI", DisplayName = "Build" }]
        };
        
        var constraint = new HasStepConstraint("Build");
        var result = constraint.ApplyTo(job);
        
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineJob_StepNotFound_ReturnsFailure()
    {
        var job = new PipelineJob
        {
            Steps = [new TaskStep { TaskName = "DotNetCoreCLI", DisplayName = "Build" }]
        };
        
        var constraint = new HasStepConstraint("Test");
        var result = constraint.ApplyTo(job);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineJob_StepWithNullDisplayName_ReturnsFailure()
    {
        var job = new PipelineJob
        {
            Steps = [new TaskStep { TaskName = "DotNetCoreCLI" }]
        };
        
        var constraint = new HasStepConstraint("Build");
        var result = constraint.ApplyTo(job);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineJob_EmptySteps_ReturnsFailure()
    {
        var job = new PipelineJob { Steps = [] };
        
        var constraint = new HasStepConstraint("Build");
        var result = constraint.ApplyTo(job);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_NotPipelineJob_ReturnsFailure()
    {
        var constraint = new HasStepConstraint("Build");
        var result = constraint.ApplyTo("not a job");
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasStepConstraint("Build");
        Assert.That(constraint.Description, Is.EqualTo("Job has step with display name 'Build'"));
    }
}
