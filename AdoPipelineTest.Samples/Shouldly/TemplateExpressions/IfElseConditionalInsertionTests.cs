using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.Samples.Shouldly.TemplateExpressions;

[TestFixture]
public class IfElseConditionalInsertionTests
{
    private const string YamlPath = "pipelines/TemplateExpressions/ifelse_conditional_step_insertion.yaml";
    
    [Test]
    public void TestConditionalInsertionSteps_ConditionOne()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "one")
            .Run();

        result.ShouldNotBeNull();
        result.Stages.Count.ShouldBe(1);
        result.Stages[0].Jobs.Count.ShouldBe(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Count.ShouldBe(1);

        var firstStep = steps[0].ShouldBeOfType<TaskStep>();
        firstStep.TaskName.ShouldBe("VSBuild@1");
    }
    
    [Test]
    public void TestConditionalInsertionSteps_ConditionTwo()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "two")
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
    public void TestConditionalInsertionSteps_ConditionThree()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "three")
            .Run();

        result.ShouldNotBeNull();
        result.Stages.Count.ShouldBe(1);
        result.Stages[0].Jobs.Count.ShouldBe(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Count.ShouldBe(1);
    }
}
