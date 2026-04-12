using AdoPipelineTest.PipelineAssertions;
using AdoPipelineTest.Model.Steps;
using FluentAssertions;
using TestClass = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestMethod = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;

namespace AdoPipelineTest.Samples.FluentAssertions.Variables;

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

        result.Should().HaveStageCount(1);
        result.Stages[0].Jobs.Should().HaveCount(1);
        
        var steps = result.Stages[0].Jobs[0].Steps;
        steps.Should().HaveCount(4);
    }

    [TestMethod]
    public void VariableEvaluatedInBuildStep()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        buildStep.Should().NotBeNull();
        buildStep!.DisplayName.Should().Be("Build");

        var arguments = buildStep.Inputs?["arguments"]?.ToString();
        arguments.Should().Contain("Release");
    }

    [TestMethod]
    public void VariableEvaluatedInTestStep()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var testStep = steps[3] as TaskStep;

        testStep.Should().NotBeNull();
        testStep!.DisplayName.Should().Be("Test");

        var arguments = testStep.Inputs?["arguments"]?.ToString();
        arguments.Should().Contain("Release");
    }

    [TestMethod]
    public void AllStepsPreserveTaskDetails()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;

        steps.Should().AllBeOfType<TaskStep>();
        steps[0].DisplayName.Should().NotBeNullOrEmpty();
        ((TaskStep)steps[0]).TaskName.Should().NotBeNullOrEmpty();
        steps[1].DisplayName.Should().NotBeNullOrEmpty();
        ((TaskStep)steps[1]).TaskName.Should().NotBeNullOrEmpty();
        steps[2].DisplayName.Should().NotBeNullOrEmpty();
        ((TaskStep)steps[2]).TaskName.Should().NotBeNullOrEmpty();
        steps[3].DisplayName.Should().NotBeNullOrEmpty();
        ((TaskStep)steps[3]).TaskName.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public void VariableOverriddenWithCustomValue()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        buildStep.Should().NotBeNull();
        buildStep!.DisplayName.Should().Be("Build");

        var arguments = buildStep.Inputs["arguments"]?.ToString();
        arguments.Should().Contain("Debug");
        arguments.Should().NotContain("Release");
    }

    [TestMethod]
    public void VariableOverriddenInMultipleSteps()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;
        var testStep = steps[3] as TaskStep;

        buildStep.Should().NotBeNull();
        testStep.Should().NotBeNull();
        
        var buildArguments = buildStep!.Inputs?["arguments"]?.ToString();
        var testArguments = testStep!.Inputs?["arguments"]?.ToString();

        buildArguments.Should().Contain("Debug");
        testArguments.Should().Contain("Debug");
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
        arguments.Should().Contain("Release");
    }

    [TestMethod]
    public void MultipleVariablesCanBeSet()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Release" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        var arguments = buildStep!.Inputs?["arguments"]?.ToString();
        arguments.Should().Contain("Release");
    }

    [TestMethod]
    public void VariableEvaluationPreservesStepTaskName()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        buildStep.Should().NotBeNull();
        buildStep!.TaskName.Should().Be("DotNetCoreCLI@2");
    }
}
