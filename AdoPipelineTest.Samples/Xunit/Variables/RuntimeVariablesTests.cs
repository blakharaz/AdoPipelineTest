using AdoPipelineTest.Model.Steps;
using Assert = AdoPipelineTest.Xunit.Assert;
using Xunit;

namespace AdoPipelineTest.Samples.Xunit.Variables;

public class RuntimeVariablesTests
{
    private const string YamlPath = "pipelines/Variables/pipeline_with_runtime_expressions.yaml";

    [Fact]
    public void PipelineLoadedWithRuntimeExpressions()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        Assert.NotNull(result);
        Assert.StageCount(result, 1);
        Assert.Single(result.Stages[0].Jobs);
    }

    [Fact]
    public void RuntimeVariablesReplacedInScriptStep()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithRuntimeVariables(new Dictionary<string, string>
            {
                ["Build.BuildNumber"] = "12345"
            })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var step = Assert.IsType<ScriptStep>(steps[0]);

        Assert.Contains("12345", step.Script);
    }

    [Fact]
    public void MultipleRuntimeVariablesReplaced()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithRuntimeVariables(new Dictionary<string, string>
            {
                ["Build.BuildNumber"] = "2024.001",
                ["System.StageName"] = "Build"
            })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        var step1 = Assert.IsType<ScriptStep>(steps[0]);
        Assert.Contains("2024.001", step1.Script);
        
        var step2 = Assert.IsType<ScriptStep>(steps[1]);
        Assert.Contains("Build", step2.Script);
    }

    [Fact]
    public void UnresolvedRuntimeVariableLeftUnchanged()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithRuntimeVariables(new Dictionary<string, string>())
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var step = Assert.IsType<ScriptStep>(steps[0]);

        Assert.Contains("$(Build.BuildNumber)", step.Script);
    }
}