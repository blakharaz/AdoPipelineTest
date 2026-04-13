using AdoPipelineTest.Model.Steps;
using Xunit;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests;

public class PipelineTesterVariablesTest
{
    [Fact]
    public void Run_UsesVariableDefaults_InTaskInputs()
    {
        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_variables.yaml")
            .Run();

        Assert.Equal(3, result.Variables.Count);
        Assert.Contains(result.Variables.Select(v => v.Name), v => v == "buildConfiguration");

        Assert.Equal(1, result.Stages.Count);
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.Equal(2, steps.Count);

        var buildTask = steps[1] as TaskStep;
        Assert.NotNull(buildTask);
        Assert.Contains("arguments", buildTask!.Inputs.Keys);
        Assert.Contains("Release", buildTask.Inputs["arguments"]);
    }

    [Fact]
    public void Run_OverridesVariableDefaults_WithUserProvidedVariables()
    {
        var customVariables = new Dictionary<string, object?>
        {
            ["buildConfiguration"] = "Debug"
        };

        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_variables.yaml")
            .WithVariables(customVariables)
            .Run();

        Assert.Equal(1, result.Stages.Count);
        var steps = result.Stages[0].Jobs[0].Steps;

        var buildTask = steps[1] as TaskStep;
        Assert.NotNull(buildTask);
        Assert.Contains("arguments", buildTask!.Inputs.Keys);
        Assert.Contains("Debug", buildTask.Inputs["arguments"]);
        Assert.DoesNotContain("Release", buildTask.Inputs["arguments"]);
    }

    [Fact]
    public void Run_MergesMultipleVariables()
    {
        var customVariables = new Dictionary<string, object?>
        {
            ["customVar"] = "CustomValue"
        };

        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_variables.yaml")
            .WithVariables(customVariables)
            .Run();

        Assert.Equal(4, result.Variables.Count);
        var varNames = result.Variables.Select(v => v.Name);
        Assert.Contains(varNames, n => n == "buildConfiguration");
        Assert.Contains(varNames, n => n == "customVar");
    }

    [Fact]
    public void Run_HandlesNullVariableValues()
    {
        var customVariables = new Dictionary<string, object?>
        {
            ["buildConfiguration"] = null
        };

        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_variables.yaml")
            .WithVariables(customVariables)
            .Run();

        var variable = result.Variables.FirstOrDefault(v => v.Name == "buildConfiguration");
        Assert.NotNull(variable);
        Assert.Null(variable!.DefaultValue);
    }
}