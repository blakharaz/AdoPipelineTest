using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.Samples.Shouldly.SimplePipeline;

[TestFixture]
public class SimpleDotnetPipelineTests
{
    private const string YamlPath = "pipelines/SimplePipeline/simple_dotnet_pipeline.yaml";
    
    [Test]
    public void VerifyBasics()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        result.ShouldNotBeNull();
        result.ShouldHaveTrigger();
        result.ShouldIncludeBranch("main");
        result.ShouldHaveVmImage("ubuntu-latest");
        result.ShouldHaveStageCount(1);
        result.Stages[0].ShouldHaveJobCount(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Count.ShouldBe(4);
    }

    [Test]
    public void VerifyStep1()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        result.ShouldNotBeNull();

        var steps = result.Stages[0].Jobs[0].Steps;
        var step1 = steps[0].ShouldBeOfType<TaskStep>();

        step1.DisplayName.ShouldBe("Use .NET 8.0");
        step1.ContinueOnError.ShouldBeFalse();
        step1.TaskName.ShouldBe("UseDotNet@2");
    }

    [Test]
    public void VerifyStep2()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        result.ShouldNotBeNull();
        var steps = result.Stages[0].Jobs[0].Steps;
        var step2 = steps[1].ShouldBeOfType<TaskStep>();

        step2.DisplayName.ShouldBe("Restore dependencies");
        step2.ContinueOnError.ShouldBeFalse();
        step2.TaskName.ShouldBe("DotNetCoreCLI@2");
    }

    [Test]
    public void VerifyStep3()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        result.ShouldNotBeNull();
        var steps = result.Stages[0].Jobs[0].Steps;
        var step3 = steps[2].ShouldBeOfType<TaskStep>();

        step3.DisplayName.ShouldBe("Build");
        step3.ContinueOnError.ShouldBeFalse();
        step3.TaskName.ShouldBe("DotNetCoreCLI@2");
    }

    [Test]
    public void VerifyStep4()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        result.ShouldNotBeNull();
        var steps = result.Stages[0].Jobs[0].Steps;
        var step4 = steps[3].ShouldBeOfType<TaskStep>();

        step4.DisplayName.ShouldBe("Test");
        step4.ContinueOnError.ShouldBeFalse();
        step4.TaskName.ShouldBe("DotNetCoreCLI@2");
    }
}
