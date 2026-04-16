using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdoPipelineTest.Mstest;
using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.Samples.Mstest.TemplateExpressions;

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

        Assert.IsNotNull(result);
        result.HasStageCount(1);
        Assert.HasCount(1, result.Stages[0].Jobs);

        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.HasCount(1, steps);

        var firstStep = steps[0] as TaskStep;
        Assert.IsNotNull(firstStep);
        Assert.AreEqual("VSBuild@1", firstStep.TaskName);
    }

    [TestMethod]
    public void TestConditionalInsertionSteps_ConditionDotnet()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "two")
            .Run();

        Assert.IsNotNull(result);
        result.HasStageCount(1);
        Assert.HasCount(1, result.Stages[0].Jobs);

        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.HasCount(3, steps);

        var firstStep = steps[0] as TaskStep;
        Assert.IsNotNull(firstStep);
        Assert.AreEqual("UseDotNet@2", firstStep.TaskName);

        var secondStep = steps[1] as TaskStep;
        Assert.IsNotNull(secondStep);
        Assert.AreEqual("UseDotNet@2", secondStep.TaskName);

        var thirdStep = steps[2] as TaskStep;
        Assert.IsNotNull(thirdStep);
        Assert.AreEqual("UseDotNet@2", thirdStep.TaskName);
    }

    [TestMethod]
    public void TestConditionalInsertionSteps_ConditionNothing()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "three")
            .Run();

        Assert.IsNotNull(result);
        result.HasStageCount(1);
        Assert.HasCount(1, result.Stages[0].Jobs);

        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.HasCount(1, steps);
    }
}
