using AdoPipelineTest.PipelineAssertions;
using FluentAssertions;

namespace AdoPipelineTest.Samples.FluentAssertions.Parameters;

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

        result.Should().HaveStageCount(1);
        
        var job = result.Stages[0].Jobs[0];
        job.DisplayName.Should().Be("Build and Test Job");
        job.Steps.Should().HaveCount(2);
    }

    [TestMethod]
    public void VerifyParameterDefaults()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt")
            .Run();

        var parameters = result.Parameters;

        result.Should().HaveParameter("projectName");
        parameters["projectName"].Value.Should().Be("MySampleProject");
        
        parameters["enableTests"].Value.Should().Be(true);
        parameters["timeoutMinutes"].Value.Should().Be(30);
        parameters["buildConfiguration"].Value.Should().Be("Release");
        parameters["outputDirectory"].Value.Should().Be("$(Build.ArtifactStagingDirectory)");
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

        parameters["projectName"].Value.Should().Be("CustomProject");
        parameters["enableTests"].Value.Should().Be(false);
        parameters["buildConfiguration"].Value.Should().Be("Debug");
    }

    [TestMethod]
    public void VerifyUndefinedParameterIsNotAllowed()
    {
        var tester = new PipelineTester()
            .WithPipeline(YamlPath);

        var act = () => tester.Run();
        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void VerifyAllUndefinedParameterSet()
    {
        var tester = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "output.txt");

        var act = () => tester.Run();
        act.Should().NotThrow();
    }
}
