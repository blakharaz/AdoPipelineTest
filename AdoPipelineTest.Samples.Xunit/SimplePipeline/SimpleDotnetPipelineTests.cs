using Xunit;
using AdoPipelineTest.Xunit;
using XunitAssert = Xunit.Assert;

namespace AdoPipelineTest.Samples.Xunit.SimplePipeline;

public class SimpleDotnetPipelineTests
{
    [Fact]
    public void Pipeline_HasCorrectStructure()
    {
        var result = new PipelineTester()
            .WithPipeline("SimplePipeline/simple_dotnet_pipeline.yaml")
            .Run();
        
        Assert.HasTrigger(result);
        Assert.HasVmImage(result, "ubuntu-latest");
        Assert.StageCount(result, 1);
        XunitAssert.Single(result.Stages[0].Jobs);
    }

    [Fact]
    public void Pipeline_HasCorrectStepCount()
    {
        var result = new PipelineTester()
            .WithPipeline("SimplePipeline/simple_dotnet_pipeline.yaml")
            .Run();
        
        var steps = result.Stages[0].Jobs[0].Steps;
        XunitAssert.Equal(4, steps.Count);
    }

    [Fact]
    public void Pipeline_HasDotNetTasks()
    {
        var result = new PipelineTester()
            .WithPipeline("SimplePipeline/simple_dotnet_pipeline.yaml")
            .Run();
        
        Assert.HasTask(result, "UseDotNet@2");
        Assert.HasTask(result, "DotNetCoreCLI@2");
    }

    [Fact]
    public void Pipeline_TriggersOnMainBranch()
    {
        var result = new PipelineTester()
            .WithPipeline("SimplePipeline/simple_dotnet_pipeline.yaml")
            .Run();
        
        Assert.TriggersIncludeBranch(result, "main");
    }
}
