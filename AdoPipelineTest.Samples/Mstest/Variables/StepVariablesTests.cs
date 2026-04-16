using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdoPipelineTest.Mstest;
using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.Samples.Mstest.Variables;

[TestClass]
public class StepVariablesTests
{
    private const string YamlPath = "pipelines/Variables/pipeline_with_variables.yaml";

    [TestMethod]
    public void PipelineLoadedWithVariables()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        Assert.IsNotNull(result);
        result.HasStageCount(1);
        Assert.HasCount(1, result.Stages[0].Jobs);

        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.HasCount(4, steps);
    }

    [TestMethod]
    public void VariableEvaluatedInBuildStep()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        Assert.IsNotNull(buildStep);
        Assert.AreEqual("Build", buildStep.DisplayName);

        var arguments = buildStep.Inputs?["arguments"]?.ToString();
        Assert.Contains("Release", arguments);
    }

    [TestMethod]
    public void VariableEvaluatedInTestStep()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var testStep = steps[3] as TaskStep;

        Assert.IsNotNull(testStep);
        Assert.AreEqual("Test", testStep.DisplayName);

        var arguments = testStep.Inputs?["arguments"]?.ToString();
        Assert.Contains("Release", arguments);
    }

    [TestMethod]
    public void AllStepsPreserveTaskDetails()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;

        foreach (var step in steps)
        {
            Assert.IsInstanceOfType<TaskStep>(step);
        }

        Assert.IsNotNull(steps[0].DisplayName);
        Assert.IsNotNull((steps[0] as TaskStep)?.TaskName);

        Assert.IsNotNull(steps[1].DisplayName);
        Assert.IsNotNull((steps[1] as TaskStep)?.TaskName);

        Assert.IsNotNull(steps[2].DisplayName);
        Assert.IsNotNull((steps[2] as TaskStep)?.TaskName);

        Assert.IsNotNull(steps[3].DisplayName);
        Assert.IsNotNull((steps[3] as TaskStep)?.TaskName);
    }

    [TestMethod]
    public void VariableOverriddenWithCustomValue()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object?> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        Assert.IsNotNull(buildStep);
        Assert.AreEqual("Build", buildStep.DisplayName);

        var arguments = buildStep.Inputs["arguments"]?.ToString();
        Assert.Contains("Debug", arguments);
        Assert.DoesNotContain("Release", arguments);
    }

    [TestMethod]
    public void VariableOverriddenInMultipleSteps()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object?> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;
        var testStep = steps[3] as TaskStep;

        Assert.IsNotNull(buildStep);
        Assert.IsNotNull(testStep);

        var buildArguments = buildStep!.Inputs?["arguments"]?.ToString();
        var testArguments = testStep!.Inputs?["arguments"]?.ToString();

        Assert.Contains("Debug", buildArguments);
        Assert.Contains("Debug", testArguments);
    }

    [TestMethod]
    public void DefaultVariableValueUsedWhenNotOverridden()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        var arguments = buildStep!.Inputs?["arguments"]?.ToString();
        Assert.Contains("Release", arguments);
    }

    [TestMethod]
    public void MultipleVariablesCanBeSet()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object?> { ["buildConfiguration"] = "Release" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        var arguments = buildStep!.Inputs?["arguments"]?.ToString();
        Assert.Contains("Release", arguments);
    }

    [TestMethod]
    public void VariableEvaluationPreservesStepTaskName()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object?> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        Assert.IsNotNull(buildStep);
        Assert.AreEqual("DotNetCoreCLI@2", buildStep.TaskName);
    }
}
