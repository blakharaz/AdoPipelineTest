using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdoPipelineTest.Mstest;
using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.Samples.Mstest.Parameters;

[TestClass]
public class PipelineWithParametersTests
{
    private const string YamlPath = "pipelines/Parameters/pipeline_with_parameters.yaml";

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
        Assert.AreEqual(2, job.Steps.Count);
    }

    [TestMethod]
    public void VerifyParameterDefaults()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt")
            .Run();

        var parameters = result.Parameters;

        Assert.AreEqual("MySampleProject", parameters["projectName"].Value);
        var enableTests = (bool?)parameters["enableTests"].Value;
        Assert.IsTrue(enableTests);
        Assert.AreEqual(30, parameters["timeoutMinutes"].Value);
        Assert.AreEqual("Release", parameters["buildConfiguration"].Value);
        Assert.AreEqual("$(Build.ArtifactStagingDirectory)", parameters["outputDirectory"].Value);
        Assert.IsInstanceOfType<Dictionary<object, object>>(parameters["buildSettings"].Value);
        var settingsDict = (Dictionary<object, object>)parameters["buildSettings"].Value!;
        Assert.AreEqual(0, settingsDict.Count);
    }

    [TestMethod]
    public void VerifyParameterSetForRun()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("projectName", "CustomProject")
            .WithParameter("targetFile", "output.txt")
            .WithParameter("enableTests", false)
            .WithParameter("buildConfiguration", "Debug")
            .WithParameter("timeoutMinutes", 10)
            .WithParameter("buildSettings", new Dictionary<string, string> { ["noRestore"] = "true" })
            .Run();

        var parameters = result.Parameters;

        Assert.AreEqual("CustomProject", parameters["projectName"].Value);
        var enableTests = (bool?)parameters["enableTests"].Value;
        Assert.IsFalse(enableTests);
        Assert.AreEqual("Debug", parameters["buildConfiguration"].Value);
    }

    [TestMethod]
    public void VerifyUndefinedParameterIsNotAllowed()
    {
        var tester = new PipelineTester()
            .WithPipeline(YamlPath);

        Assert.ThrowsExactly<InvalidOperationException>(() => tester.Run());
    }

    [TestMethod]
    public void VerifyAllUndefinedParameterSet()
    {
        var tester = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "output.txt");

        Assert.IsNotNull(tester.Run());
    }
}
