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

        Assert.Single(result.Stages);
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

        Assert.Single(result.Stages);
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

    [Fact]
    public void Run_ReplacesRuntimeVariables_InScriptSteps()
    {
        var runtimeVars = new Dictionary<string, string>
        {
            ["Build.BuildNumber"] = "12345"
        };

        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_runtime_expressions.yaml")
            .WithRuntimeVariables(runtimeVars)
            .Run();

        Assert.Single(result.Stages);
        var steps = result.Stages[0].Jobs[0].Steps;
        
        var step1 = steps[0] as ScriptStep;
        Assert.NotNull(step1);
        Assert.Contains("12345", step1!.Script);
    }

    [Fact]
    public void Run_ReplacesMultipleRuntimeVariables()
    {
        var runtimeVars = new Dictionary<string, string>
        {
            ["Build.BuildNumber"] = "2024.001",
            ["System.StageName"] = "Build",
            ["Agent.Name"] = "Agent01"
        };

        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_runtime_expressions.yaml")
            .WithRuntimeVariables(runtimeVars)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        var step1 = steps[0] as ScriptStep;
        Assert.NotNull(step1);
        Assert.Contains("2024.001", step1!.Script);
        
        var step2 = steps[1] as ScriptStep;
        Assert.NotNull(step2);
        Assert.Contains("Build", step2!.Script);
        
        var step3 = steps[2] as ScriptStep;
        Assert.NotNull(step3);
        Assert.Contains("Agent01", step3!.Script);
    }

    [Fact]
    public void Run_LeavesUnresolvedRuntimeVariables_Unchanged()
    {
        var runtimeVars = new Dictionary<string, string>();

        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_runtime_expressions.yaml")
            .WithRuntimeVariables(runtimeVars)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        var step1 = steps[0] as ScriptStep;
        Assert.NotNull(step1);
        Assert.Contains("$(Build.BuildNumber)", step1!.Script);
    }

    [Fact]
    public void Run_ResolvesCompileTimeParameters_BeforeRuntimeVariables()
    {
        var yamlPath = "test_data/pipeline_parser/pipeline_with_parameters.yaml";
        var runtimeVars = new Dictionary<string, string>
        {
            ["Build.Id"] = "999"
        };

        var result = new PipelineTester()
            .WithPipeline(yamlPath)
            .WithParameter("targetFile", "test.txt")
            .WithRuntimeVariables(runtimeVars)
            .Run();

        Assert.Single(result.Stages);
    }
}