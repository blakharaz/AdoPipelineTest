using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.Samples.Shouldly.SimplePipeline;

[TestFixture]
public class SimpleNodeJsPipelineTests
{
    private const string YamlPath = "pipelines/SimplePipeline/simple_nodejs_pipeline.yaml";

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
        steps.Count.ShouldBe(3);
    }

    [Test]
    public void VerifyStep1()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        result.ShouldNotBeNull();
        var steps = result.Stages[0].Jobs[0].Steps;
        var step1 = steps[0].ShouldBeOfType<TaskStep>();

        step1.DisplayName.ShouldBe("Install Node.js");
        step1.ContinueOnError.ShouldBeFalse();
        step1.TaskName.ShouldBe("NodeTool@0");
    }

    [Test]
    public void VerifyStep2()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        result.ShouldNotBeNull();
        var steps = result.Stages[0].Jobs[0].Steps;
        var step2 = steps[1].ShouldBeOfType<ScriptStep>();

        step2.DisplayName.ShouldBe("npm install and build");
        step2.ContinueOnError.ShouldBeFalse();
        step2.Script.ShouldContain("npm install");
        step2.Script.ShouldContain("npm run build");
    }

    [Test]
    public void VerifyStep3()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        result.ShouldNotBeNull();
        var steps = result.Stages[0].Jobs[0].Steps;
        var step3 = steps[2].ShouldBeOfType<ScriptStep>();

        step3.DisplayName.ShouldBe("npm test");
        step3.ContinueOnError.ShouldBeTrue();
        step3.Script.ShouldContain("npm test");
    }
}
