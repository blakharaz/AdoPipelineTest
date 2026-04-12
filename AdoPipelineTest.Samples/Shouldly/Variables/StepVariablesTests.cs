using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.Samples.Shouldly.Variables;

[TestFixture]
public class StepVariablesTests
{
    private const string YamlPath = "pipelines/Variables/pipeline_with_variables.yaml";

    [Test]
    public void PipelineLoadedWithVariables()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        result.ShouldNotBeNull();
        result.Stages.Count.ShouldBe(1);
        result.Stages[0].Jobs.Count.ShouldBe(1);
        result.Stages[0].Jobs[0].Steps.Count.ShouldBe(4);
    }

    [Test]
    public void VariableEvaluatedInBuildStep()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2].ShouldBeOfType<TaskStep>();

        buildStep.DisplayName.ShouldBe("Build");
        var arguments = buildStep.Inputs?["arguments"]?.ToString() ?? string.Empty;
        arguments.ShouldContain("Release");
    }

    [Test]
    public void VariableEvaluatedInTestStep()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var testStep = steps[3].ShouldBeOfType<TaskStep>();

        testStep.DisplayName.ShouldBe("Test");
        var arguments = testStep.Inputs?["arguments"]?.ToString() ?? string.Empty;
        arguments.ShouldContain("Release");
    }

    [Test]
    public void AllStepsAreTaskSteps()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;

        foreach (var step in steps)
        {
            step.ShouldBeOfType<TaskStep>();
        }
    }

    [Test]
    public void VariableOverriddenWithCustomValue()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2].ShouldBeOfType<TaskStep>();

        buildStep.DisplayName.ShouldBe("Build");
        var arguments = buildStep.Inputs?["arguments"]?.ToString() ?? string.Empty;
        arguments.ShouldContain("Debug");
        arguments.ShouldNotContain("Release");
    }

    [Test]
    public void VariableOverriddenInMultipleSteps()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2].ShouldBeOfType<TaskStep>();
        var testStep = steps[3].ShouldBeOfType<TaskStep>();

        var buildArguments = buildStep.Inputs?["arguments"]?.ToString() ?? string.Empty;
        var testArguments = testStep.Inputs?["arguments"]?.ToString() ?? string.Empty;

        buildArguments.ShouldContain("Debug");
        testArguments.ShouldContain("Debug");
    }

    [Test]
    public void DefaultVariableValueUsedWhenNotOverridden()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2].ShouldBeOfType<TaskStep>();

        var arguments = buildStep.Inputs?["arguments"]?.ToString() ?? string.Empty;
        arguments.ShouldContain("Release");
    }

    [Test]
    public void VariableEvaluationPreservesStepTaskName()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2].ShouldBeOfType<TaskStep>();

        buildStep.TaskName.ShouldBe("DotNetCoreCLI@2");
    }
}
