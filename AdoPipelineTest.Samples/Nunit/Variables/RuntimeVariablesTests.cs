using AdoPipelineTest.Model.Steps;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Does = AdoPipelineTest.Nunit.Does;
using Is = AdoPipelineTest.Nunit.Is;

namespace AdoPipelineTest.Samples.Nunit.Variables;

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

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Stages, Has.Count.EqualTo(1));
        Assert.That(result.Stages[0].Jobs, Has.Count.EqualTo(1));
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
        var step = (ScriptStep)steps[0];

        Assert.That(step.Script, Does.Contain("12345"));
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
        
        var step1 = (ScriptStep)steps[0];
        Assert.That(step1.Script, Does.Contain("2024.001"));
        
        var step2 = (ScriptStep)steps[1];
        Assert.That(step2.Script, Does.Contain("Build"));
    }

    [Test]
    public void UnresolvedRuntimeVariableLeftUnchanged()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithRuntimeVariables(new Dictionary<string, string>())
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var step = (ScriptStep)steps[0];

        Assert.That(step.Script, Does.Contain("$(Build.BuildNumber)"));
    }
}