using AdoPipelineTest.Mstest;
using AdoPipelineTest.Model.Steps;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace AdoPipelineTest.Samples.Mstest.Variables;

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

        Assert.IsNotNull(result);
        result.HasStageCount(1);
        Assert.HasCount(1, result.Stages[0].Jobs);
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

        Assert.IsNotNull(step);
        Assert.Contains("12345", step.Script);
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
        Assert.IsNotNull(step1);
        Assert.Contains("2024.001", step1.Script);
        
        var step2 = steps[1] as ScriptStep;
        Assert.IsNotNull(step2);
        Assert.Contains("Build", step2.Script);
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

        Assert.IsNotNull(step);
        Assert.Contains("$(Build.BuildNumber)", step.Script);
    }
}