using AdoPipelineTest.PipelineAssertions;
using AdoPipelineTest.Model.Steps;
using FluentAssertions;
using TestClass = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestMethod = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;

namespace AdoPipelineTest.Samples.FluentAssertions.TemplateExpressions;

[TestClass]
public class SimpleConditionalInsertionTests
{
    private const string YamlPath = "pipelines/TemplateExpressions/simple_conditional_insertion.yaml";
    
    [TestMethod]
    public void TestConditionalInsertionSteps_ConditionMsBuild()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("toolset", "msbuild")
            .Run();

        result.Should().HaveStageCount(1);
        result.Stages[0].Jobs.Should().HaveCount(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Should().HaveCount(2);

        var firstStep = steps[0] as TaskStep;
        firstStep.Should().NotBeNull();
        firstStep!.TaskName.Should().Be("VSBuild@1");

        var secondStep = steps[1] as TaskStep;
        secondStep.Should().NotBeNull();
        secondStep!.TaskName.Should().Be("VSTest@3");
    }
    
    [TestMethod]
    public void TestConditionalInsertionSteps_ConditionDotnet()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("toolset", "dotnet")
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
            .WithParameter("toolset", "nothing")
            .Run();

        result.Should().HaveStageCount(1);
        result.Stages[0].Jobs.Should().HaveCount(1);

        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Should().BeEmpty();
    }
}
