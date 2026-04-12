using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.Samples.Shouldly.TemplateExpressions;

[TestFixture]
public class SimpleConditionalInsertionTests
{
    private const string YamlPath = "pipelines/TemplateExpressions/simple_conditional_insertion.yaml";
    
    [Test]
    public void TestConditionalInsertionSteps_ConditionMsBuild()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("toolset", "msbuild")
            .Run();

        result.ShouldNotBeNull();
        result.Stages.Count.ShouldBe(1);
        result.Stages[0].Jobs.Count.ShouldBe(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Count.ShouldBe(2);

        var firstStep = steps[0].ShouldBeOfType<TaskStep>();
        firstStep.TaskName.ShouldBe("VSBuild@1");

        var secondStep = steps[1].ShouldBeOfType<TaskStep>();
        secondStep.TaskName.ShouldBe("VSTest@3");
    }
    
    [Test]
    public void TestConditionalInsertionSteps_ConditionDotnet()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("toolset", "dotnet")
            .Run();

        result.ShouldNotBeNull();
        result.Stages.Count.ShouldBe(1);
        result.Stages[0].Jobs.Count.ShouldBe(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Count.ShouldBe(3);

        var firstStep = steps[0].ShouldBeOfType<TaskStep>();
        firstStep.TaskName.ShouldBe("UseDotNet@2");

        var secondStep = steps[1].ShouldBeOfType<TaskStep>();
        secondStep.TaskName.ShouldBe("UseDotNet@2");

        var thirdStep = steps[2].ShouldBeOfType<TaskStep>();
        thirdStep.TaskName.ShouldBe("UseDotNet@2");
    }

    [Test]
    public void TestConditionalInsertionSteps_ConditionNothing()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("toolset", "nothing")
            .Run();

        result.ShouldNotBeNull();
        result.Stages.Count.ShouldBe(1);
        result.Stages[0].Jobs.Count.ShouldBe(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.ShouldBeEmpty();
    }
}
