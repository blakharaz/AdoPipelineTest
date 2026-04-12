using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.Samples.Shouldly.Parameters;

[TestFixture]
public class PipelineWithParametersTests
{
    private const string YamlPath = "pipelines/Parameters/pipeline_with_parameters.yaml";

    [Test]
    public void VerifyPipelineStructure()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt")
            .Run();

        result.ShouldNotBeNull();
        result.Parameters.Count.ShouldBe(7);
        result.Stages.Count.ShouldBe(1);
        var job = result.Stages[0].Jobs[0];
        job.DisplayName.ShouldBe("Build and Test Job");
        job.Steps.Count.ShouldBe(2);
    }

    [Test]
    public void VerifyParameterDefaults()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt")
            .Run();

        var parameters = result.Parameters;
        
        parameters["projectName"].Value.ShouldBe("MySampleProject");
        parameters["enableTests"].Value.ShouldBe(true);
        parameters["timeoutMinutes"].Value.ShouldBe(30);
        parameters["buildConfiguration"].Value.ShouldBe("Release");
        parameters["outputDirectory"].Value.ShouldBe("$(Build.ArtifactStagingDirectory)");
        parameters["buildSettings"].Value.ShouldBeOfType<Dictionary<object, object>>();
    }

    [Test]
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
        
        parameters["projectName"].Value.ShouldBe("CustomProject");
        parameters["enableTests"].Value.ShouldBe(false);
        parameters["buildConfiguration"].Value.ShouldBe("Debug");
    }

    [Test]
    public void VerifyUndefinedParameterIsNotAllowed()
    {
        var tester = new PipelineTester()
            .WithPipeline(YamlPath);
        
        var ex = Should.Throw<InvalidOperationException>(() => tester.Run());
        ex.ShouldNotBeNull();
    }

    [Test]
    public void VerifyAllUndefinedParameterSet()
    {
        var tester = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "output.txt");

        Should.NotThrow(() => tester.Run());
    }
}
