using NUnit.Framework;
using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Nunit.Constraints;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasTaskConstraintTest
{
    [Test]
    public void ApplyTo_PipelineJob_HasTask_ReturnsSuccess()
    {
        var job = new PipelineJob
        {
            Steps = [new TaskStep { TaskName = "DotNetCoreCLI@2" }]
        };
        
        var constraint = new HasTaskConstraint("DotNetCoreCLI@2");
        var result = constraint.ApplyTo(job);
        
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineJob_TaskNotFound_ReturnsFailure()
    {
        var job = new PipelineJob
        {
            Steps = [new TaskStep { TaskName = "DotNetCoreCLI@2" }]
        };
        
        var constraint = new HasTaskConstraint("NUnit@3");
        var result = constraint.ApplyTo(job);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineJob_IgnoresScriptSteps_ReturnsFailure()
    {
        var job = new PipelineJob
        {
            Steps = [new ScriptStep { Script = "echo hello" }]
        };
        
        var constraint = new HasTaskConstraint("SomeTask");
        var result = constraint.ApplyTo(job);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineJob_EmptySteps_ReturnsFailure()
    {
        var job = new PipelineJob { Steps = [] };
        
        var constraint = new HasTaskConstraint("DotNetCoreCLI@2");
        var result = constraint.ApplyTo(job);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_NotPipelineJob_ReturnsFailure()
    {
        var constraint = new HasTaskConstraint("DotNetCoreCLI@2");
        var result = constraint.ApplyTo("not a job");
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasTaskConstraint("DotNetCoreCLI@2");
        Assert.That(constraint.Description, Is.EqualTo("Job has task 'DotNetCoreCLI@2'"));
    }
}
