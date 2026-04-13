using AdoPipelineTest.PipelineAssertions;
using AdoPipelineTest.Model.Steps;
using FluentAssertions;

namespace AdoPipelineTest.Samples.FluentAssertions.TemplateExpressions;

[TestClass]
public class IfElseConditionalInsertionTests
{
    private const string YamlPath = "pipelines/TemplateExpressions/ifelse_conditional_step_insertion.yaml";
    
    [TestMethod]
    public void TestConditionalInsertionSteps_ConditionMsBuild()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "one")
            .Run();

        result.Should().HaveStageCount(1);
        result.Stages[0].Jobs.Should().HaveCount(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Should().HaveCount(1);

        var firstStep = steps[0] as TaskStep;
        firstStep.Should().NotBeNull();
        firstStep!.TaskName.Should().Be("VSBuild@1");
    }
    
    [TestMethod]
    public void TestConditionalInsertionSteps_ConditionDotnet()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "two")
            .Run();

        result.Should().HaveStageCount(1);
        result.Stages[0].Jobs.Should().HaveCount(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Should().HaveCount(3);

        var firstStep = steps[0] as TaskStep;
        firstStep.Should().NotBeNull();
        firstStep!.TaskName.Should().Be("UseDotNet@2");

        var secondStep = steps[1] as TaskStep;
        secondStep.Should().NotBeNull();
        secondStep!.TaskName.Should().Be("UseDotNet@2");

        var thirdStep = steps[2] as TaskStep;
        thirdStep.Should().NotBeNull();
        thirdStep!.TaskName.Should().Be("UseDotNet@2");
    }

    [TestMethod]
    public void TestConditionalInsertionSteps_ConditionNothing()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "three")
            .Run();

        result.Should().HaveStageCount(1);
        result.Stages[0].Jobs.Should().HaveCount(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Should().HaveCount(1);
    }
}
