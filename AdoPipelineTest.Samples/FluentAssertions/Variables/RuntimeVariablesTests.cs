using AdoPipelineTest.PipelineAssertions;
using AdoPipelineTest.Model.Steps;
using FluentAssertions;

namespace AdoPipelineTest.Samples.FluentAssertions.Variables;

[TestClass]
public class RuntimeVariablesTests
{
    private const string YamlPath = "pipelines/Variables/pipeline_with_runtime_expressions.yaml";

    [TestMethod]
    public void PipelineLoadedWithRuntimeExpressions()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        result.Should().HaveStageCount(1);
        result.Stages[0].Jobs.Should().HaveCount(1);
    }

    [TestMethod]
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
        var step = steps[0] as ScriptStep;

        step.Should().NotBeNull();
        step!.Script.Should().Contain("12345");
    }

    [TestMethod]
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
        
        var step1 = steps[0] as ScriptStep;
        step1.Should().NotBeNull();
        step1!.Script.Should().Contain("2024.001");
        
        var step2 = steps[1] as ScriptStep;
        step2.Should().NotBeNull();
        step2!.Script.Should().Contain("Build");
    }

    [TestMethod]
    public void UnresolvedRuntimeVariableLeftUnchanged()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithRuntimeVariables(new Dictionary<string, string>())
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var step = steps[0] as ScriptStep;

        step.Should().NotBeNull();
        step!.Script.Should().Contain("$(Build.BuildNumber)");
    }
}