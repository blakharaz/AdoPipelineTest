using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdoPipelineTest.Mstest;

namespace AdoPipelineTest.Samples.Mstest.SimplePipeline;

[TestClass]
public class SimpleDotnetPipelineTests
{
    [TestMethod]
    public void Pipeline_HasCorrectStructure()
    {
        var result = new PipelineTester()
            .WithPipeline("SimplePipeline/simple_dotnet_pipeline.yaml")
            .Run();
        
        result.HasTrigger();
        result.HasVmImage("ubuntu-latest");
        result.HasStageCount(1);
        Assert.AreEqual(1, result.Stages[0].Jobs.Count);
    }

    [TestMethod]
    public void Pipeline_HasCorrectStepCount()
    {
        var result = new PipelineTester()
            .WithPipeline("SimplePipeline/simple_dotnet_pipeline.yaml")
            .Run();
        
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.AreEqual(4, steps.Count);
    }

    [TestMethod]
    public void Pipeline_HasDotNetTasks()
    {
        var result = new PipelineTester()
            .WithPipeline("SimplePipeline/simple_dotnet_pipeline.yaml")
            .Run();
        
        result.HasTask("UseDotNet@2");
        result.HasTask("DotNetCoreCLI@2");
    }

    [TestMethod]
    public void Pipeline_TriggersOnMainBranch()
    {
        var result = new PipelineTester()
            .WithPipeline("SimplePipeline/simple_dotnet_pipeline.yaml")
            .Run();
        
        result.TriggersIncludeBranch("main");
    }
}
