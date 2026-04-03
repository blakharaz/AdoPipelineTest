using AdoPipelineTest.Model.Steps;
using Assert = AdoPipelineTest.Xunit.Assert;
using Xunit;



namespace AdoPipelineTest.Samples.Xunit.SimplePipeline;

public class SimpleJobPipelineTests
{
    private const string YamlPath = "pipelines/SimplePipeline/simple_job_pipeline.yaml";

    [Fact]
    public void VerifyBasics()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.NotNull(result);
        Assert.HasTrigger(result);
        Assert.HasVmImage(result, "ubuntu-latest");
        Assert.StageCount(result, 1);
        Assert.Single(result.Stages[0].Jobs);

        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.Equal(4, steps.Count);
    }

    [Fact]
    public void VerifyStep1()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.StageCount(result, 1);
        Assert.Single(result.Stages[0].Jobs);
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.Equal(4, steps.Count);

        var step1 = Assert.IsType<TaskStep>(steps[0]);

        Assert.Equal("Use .NET 8.0", step1.DisplayName);
        Assert.False(step1.ContinueOnError);
        Assert.Equal("UseDotNet@2", step1.TaskName);
    }

    [Fact]
    public void VerifyStep2()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.StageCount(result, 1);
        Assert.Single(result.Stages[0].Jobs);
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.Equal(4, steps.Count);

        var step2 = Assert.IsType<TaskStep>(steps[1]);

        Assert.Equal("Restore dependencies", step2.DisplayName);
        Assert.False(step2.ContinueOnError);
        Assert.Equal("DotNetCoreCLI@2", step2.TaskName);
    }

    [Fact]
    public void VerifyStep3()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.StageCount(result, 1);
        Assert.Single(result.Stages[0].Jobs);
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.Equal(4, steps.Count);

        var step3 = Assert.IsType<TaskStep>(steps[2]);

        Assert.Equal("Build", step3.DisplayName);
        Assert.False(step3.ContinueOnError);
        Assert.Equal("DotNetCoreCLI@2", step3.TaskName);
    }

    [Fact]
    public void VerifyStep4()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.StageCount(result, 1);
        Assert.Single(result.Stages[0].Jobs);
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.Equal(4, steps.Count);

        var step4 = Assert.IsType<TaskStep>(steps[3]);

        Assert.Equal("Test", step4.DisplayName);
        Assert.False(step4.ContinueOnError);
        Assert.Equal("DotNetCoreCLI@2", step4.TaskName);
    }
}
