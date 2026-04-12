using AdoPipelineTest.PipelineAssertions;
using AdoPipelineTest.Model.Steps;
using FluentAssertions;
using TestClass = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestMethod = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;

namespace AdoPipelineTest.Samples.FluentAssertions.SimplePipeline;

[TestClass]
public class SimpleJobPipelineTests
{
    private const string YamlPath = "pipelines/SimplePipeline/simple_job_pipeline.yaml";

    [TestMethod]
    public void VerifyBasics()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        result.Triggers!.Should().IncludeBranch("main");
        result.Should().HaveVmImage("ubuntu-latest");
        result.Should().HaveStageCount(1);
        result.Stages[0].Jobs.Should().HaveCount(1);
        result.Stages[0].Jobs[0].Steps.Should().HaveCount(4);
    }

    [TestMethod]
    public void VerifyStep1()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var step1 = steps[0] as TaskStep;

        step1.Should().NotBeNull();
        step1!.DisplayName.Should().Be("Use .NET 8.0");
        step1.ContinueOnError.Should().BeFalse();
        step1.TaskName.Should().Be("UseDotNet@2");
    }

    [TestMethod]
    public void VerifyStep2()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var step2 = steps[1] as TaskStep;

        step2.Should().NotBeNull();
        step2!.DisplayName.Should().Be("Restore dependencies");
        step2.ContinueOnError.Should().BeFalse();
        step2.TaskName.Should().Be("DotNetCoreCLI@2");
    }

    [TestMethod]
    public void VerifyStep3()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var step3 = steps[2] as TaskStep;

        step3.Should().NotBeNull();
        step3!.DisplayName.Should().Be("Build");
        step3.ContinueOnError.Should().BeFalse();
        step3.TaskName.Should().Be("DotNetCoreCLI@2");
    }

    [TestMethod]
    public void VerifyStep4()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var step4 = steps[3] as TaskStep;

        step4.Should().NotBeNull();
        step4!.DisplayName.Should().Be("Test");
        step4.ContinueOnError.Should().BeFalse();
        step4.TaskName.Should().Be("DotNetCoreCLI@2");
    }
}
