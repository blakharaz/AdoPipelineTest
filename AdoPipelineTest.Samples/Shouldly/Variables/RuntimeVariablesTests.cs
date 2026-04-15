using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.Samples.Shouldly.Variables;

[TestFixture]
public class RuntimeVariablesTests
{
    private const string YamlPath = "pipelines/Variables/pipeline_with_runtime_expressions.yaml";

    [Test]
    public void PipelineLoadedWithRuntimeExpressions()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        result.ShouldNotBeNull();
        result.Stages.Count.ShouldBe(1);
        result.Stages[0].Jobs.Count.ShouldBe(1);
    }

    [Test]
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
        var step = steps[0].ShouldBeOfType<ScriptStep>();

        step.Script.ShouldContain("12345");
    }

    [Test]
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
        
        var step1 = steps[0].ShouldBeOfType<ScriptStep>();
        step1.Script.ShouldContain("2024.001");
        
        var step2 = steps[1].ShouldBeOfType<ScriptStep>();
        step2.Script.ShouldContain("Build");
    }

    [Test]
    public void UnresolvedRuntimeVariableLeftUnchanged()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithRuntimeVariables(new Dictionary<string, string>())
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var step = steps[0].ShouldBeOfType<ScriptStep>();

        step.Script.ShouldContain("$(Build.BuildNumber)");
    }
}