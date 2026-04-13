using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Nunit.Constraints;
using Xunit;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasTaskConstraintTest
{
    [Fact]
    public void ApplyTo_PipelineJob_HasTask_ReturnsSuccess()
    {
        var job = new PipelineJob
        {
            Steps = [new TaskStep { TaskName = "DotNetCoreCLI@2" }]
        };
        
        var constraint = new HasTaskConstraint("DotNetCoreCLI@2");
        var result = constraint.ApplyTo(job);
        
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_PipelineJob_TaskNotFound_ReturnsFailure()
    {
        var job = new PipelineJob
        {
            Steps = [new TaskStep { TaskName = "DotNetCoreCLI@2" }]
        };
        
        var constraint = new HasTaskConstraint("NUnit@3");
        var result = constraint.ApplyTo(job);
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_PipelineJob_IgnoresScriptSteps_ReturnsFailure()
    {
        var job = new PipelineJob
        {
            Steps = [new ScriptStep { Script = "echo hello" }]
        };
        
        var constraint = new HasTaskConstraint("SomeTask");
        var result = constraint.ApplyTo(job);
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_PipelineJob_EmptySteps_ReturnsFailure()
    {
        var job = new PipelineJob { Steps = [] };
        
        var constraint = new HasTaskConstraint("DotNetCoreCLI@2");
        var result = constraint.ApplyTo(job);
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_NotPipelineJob_ReturnsFailure()
    {
        var constraint = new HasTaskConstraint("DotNetCoreCLI@2");
        var result = constraint.ApplyTo("not a job");
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasTaskConstraint("DotNetCoreCLI@2");
        Assert.Equal("Job has task 'DotNetCoreCLI@2'", constraint.Description);
    }
}
