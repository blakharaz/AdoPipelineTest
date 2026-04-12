using AdoPipelineTest.PipelineAssertions;
using FluentAssertions;
using TestClass = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestMethod = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;

namespace AdoPipelineTest.Samples.FluentAssertions.SimplePipeline;

[TestClass]
public class SimpleDotnetPipelineTests
{
    private const string YamlPath = "pipelines/SimplePipeline/simple_dotnet_pipeline.yaml";

    [TestMethod]
    public void VerifyPipelineBasics()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        result.Should().HaveTriggers();
        result.Triggers.Should().IncludeBranch("main");
        result.Should().HaveVmImage("ubuntu-latest");
    }

    [TestMethod]
    public void VerifyStages()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        result.Should().HaveStageCount(1);
    }

    [TestMethod]
    public void VerifySteps()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        result.Should().HaveStep("Use .NET 8.0");
        result.Should().HaveStep("Restore dependencies");
        result.Should().HaveStep("Build");
        result.Should().HaveStep("Test");
    }

    [TestMethod]
    public void VerifyTasks()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        result.Should().HaveTask("UseDotNet@2");
        result.Should().HaveTask("DotNetCoreCLI@2");
    }

    [TestMethod]
    public void VerifyVariables()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        result.Should().HaveVariable("buildConfiguration");
        result.Should().HaveVariable("buildConfiguration", "Release");
    }
}
