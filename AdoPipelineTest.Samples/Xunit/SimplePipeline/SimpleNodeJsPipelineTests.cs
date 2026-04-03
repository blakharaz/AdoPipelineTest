using AdoPipelineTest.Model.Steps;
using Assert = AdoPipelineTest.Xunit.Assert;
using Xunit;



namespace AdoPipelineTest.Samples.Xunit.SimplePipeline;

public class SimpleNodeJsPipelineTests
{
    private const string YamlPath = "pipelines/SimplePipeline/simple_nodejs_pipeline.yaml";

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
        Assert.Equal(3, steps.Count);
    }

    [Fact]
    public void VerifyStep1()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.StageCount(result, 1);
        Assert.Single(result.Stages[0].Jobs);
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.Equal(3, steps.Count);

        var step1 = Assert.IsType<TaskStep>(steps[0]);

        Assert.Equal("Install Node.js", step1.DisplayName);
        Assert.False(step1.ContinueOnError);
        Assert.Equal("NodeTool@0", step1.TaskName);
    }

    [Fact]
    public void VerifyStep2()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.StageCount(result, 1);
        Assert.Single(result.Stages[0].Jobs);
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.Equal(3, steps.Count);

        var step2 = Assert.IsType<ScriptStep>(steps[1]);

        Assert.Equal("npm install and build", step2.DisplayName);
        Assert.False(step2.ContinueOnError);
        Assert.Contains("npm install", step2.Script);
        Assert.Contains("npm run build", step2.Script);
    }

    [Fact]
    public void VerifyStep3()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.StageCount(result, 1);
        Assert.Single(result.Stages[0].Jobs);
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.Equal(3, steps.Count);

        var step3 = Assert.IsType<ScriptStep>(steps[2]);

        Assert.Equal("npm test", step3.DisplayName);
        Assert.True(step3.ContinueOnError);
        Assert.Contains("npm test", step3.Script);
    }
}
