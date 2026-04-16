using NUnit.Framework;
using AdoPipelineTest.Model.Steps;
using Assert = NUnit.Framework.Assert;

namespace AdoPipelineTest.Samples.Nunit.Parameters;

using Is = AdoPipelineTest.Nunit.Is;

[TestFixture]
public class PipelineWithParameters
{
    private const string YamlPath = "pipelines/Parameters/pipeline_with_parameters.yaml";

    [Test]
    public void VerifyPipelineStructure()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt") // no default set, must define
            .Run();

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            // Verify parameter count
            Assert.That(result.Parameters, Has.Count.EqualTo(7));
            
            // Verify Job and Step counts
            Assert.That(result.Stages, Has.Count.EqualTo(1));
            var job = result.Stages[0].Jobs[0];
            Assert.That(job.DisplayName, Is.EqualTo("Build and Test Job"));
            Assert.That(job.Steps, Has.Count.EqualTo(2));
        }
    }

    [Test]
    public void VerifyParameterDefaults()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt") // no default set, must define
            .Run();

        var parameters = result.Parameters;
        
        using (Assert.EnterMultipleScope())
        {
            // String parameter with default
            Assert.That(parameters["projectName"].Value, Is.EqualTo("MySampleProject"));
            
            // Boolean parameter
            Assert.That(parameters["enableTests"].Value, Is.True);
            
            // Number parameter
            Assert.That(parameters["timeoutMinutes"].Value, Is.EqualTo(30));
            
            // String with allowed values
            Assert.That(parameters["buildConfiguration"].Value, Is.EqualTo("Release"));
            
            // String with environment variable
            Assert.That(parameters["outputDirectory"].Value, Is.EqualTo("$(Build.ArtifactStagingDirectory)"));
            
            // Object parameter (empty dictionary)
            Assert.That(parameters["buildSettings"].Value, Is.InstanceOf<Dictionary<object, object>>());
            var settingsDict = (Dictionary<object, object>)parameters["buildSettings"].Value!;
            Assert.That(settingsDict, Is.Empty);
        }
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
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parameters["projectName"].Value, Is.EqualTo("CustomProject"));
            Assert.That(parameters["enableTests"].Value, Is.False);
            Assert.That(parameters["buildConfiguration"].Value, Is.EqualTo("Debug"));
        }
    }

    [Test]
    public void VerifyUndefinedParameterIsNotAllowed()
    {
        var tester = new PipelineTester()
            .WithPipeline(YamlPath);
        
        Assert.Throws<InvalidOperationException>(() => tester.Run());
    }

    [Test]
    public void VerifyAllUndefinedParameterSet()
    {
        var tester = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "output.txt");

        Assert.DoesNotThrow(() => tester.Run());
    }
}