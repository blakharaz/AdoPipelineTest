using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdoPipelineTest.Mstest;
using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.Samples.Mstest.Parameters;

[TestClass]
public class PipelineWithSimpleExpressionsTests
{
    private const string YamlPath = "pipelines/Parameters/pipeline_with_simple_expressions.yaml";

    [TestMethod]
    public void VerifyPipelineStructure()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt")
            .Run();

        Assert.IsNotNull(result);
        result.HasParameter("projectName");
        result.HasStageCount(1);
        var job = result.Stages[0].Jobs[0];
        Assert.AreEqual("Build and Test Job", job.DisplayName);
        Assert.HasCount(4, job.Steps);
    }

    [TestMethod]
    public void VerifyStepInputsWithParameters()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt")
            .WithParameter("buildConfiguration", "Debug")
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;

        var buildStep = steps[1] as TaskStep;
        Assert.IsNotNull(buildStep);
        Assert.AreEqual("DotNetCoreCLI@2", buildStep.TaskName);
        Assert.AreEqual("--configuration Debug", buildStep.Inputs["arguments"]);

        var summaryStep = steps[3] as ScriptStep;
        Assert.IsNotNull(summaryStep);
        Assert.Contains("Configuration: Debug", summaryStep.Script);
    }
}
