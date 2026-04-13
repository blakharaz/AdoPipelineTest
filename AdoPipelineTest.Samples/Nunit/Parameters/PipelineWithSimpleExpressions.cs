using NUnit.Framework;
using AdoPipelineTest.Model.Steps;
using Assert = NUnit.Framework.Assert;

namespace AdoPipelineTest.Samples.Nunit.Parameters;

using Is = AdoPipelineTest.Nunit.Is;

[TestFixture]
public class PipelineWithSimpleExpressions
{
    private const string YamlPath = "pipelines/Parameters/pipeline_with_simple_expressions.yaml";

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
            Assert.That(job.Steps, Has.Count.EqualTo(4));
        }
    }

    [Test]
    public void VerifyStepInputsWithParameters()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt") // no default set, must define
            .WithParameter("buildConfiguration", "Debug")
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        // Verify the Build task (Step 2) uses the passed parameter
        var buildStep = steps[1] as TaskStep;
        Assert.That(buildStep, Is.Not.Null);
        Assert.That(buildStep.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
        Assert.That(buildStep.Inputs["arguments"], Is.EqualTo("--configuration Debug"));

        // Verify the Summary script (Step 4) contains the expected values
        var summaryStep = steps[3] as ScriptStep;
        Assert.That(summaryStep, Is.Not.Null);
        Assert.That(summaryStep.Script, Does.Contain("Configuration: Debug"));
    }
}