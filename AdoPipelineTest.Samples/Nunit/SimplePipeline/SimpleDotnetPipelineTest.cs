using NUnit.Framework;
using AdoPipelineTest.Model.Steps;
using Assert = NUnit.Framework.Assert;

namespace AdoPipelineTest.Samples.Nunit.SimplePipeline;

using Is = AdoPipelineTest.Nunit.Is;

public class SimpleDotnetPipelineTests
{
    private const string YamlPath = "pipelines/SimplePipeline/simple_dotnet_pipeline.yaml";
    
    [Test]
    public void VerifyBasics()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Triggers, Is.BranchIncluded("main"));
            Assert.That(result.AgentPool, Is.VmImage("ubuntu-latest"));
            Assert.That(result.Stages, Has.Count.EqualTo(1));
            Assert.That(result.Stages[0].Jobs, Has.Count.EqualTo(1));

            var steps = result.Stages[0].Jobs[0].Steps;
            Assert.That(steps, Has.Count.EqualTo(4));
        }
    }

    [Test]
    public void VerifyStep1()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        Assert.That(result, Is.Not.Null);

        var steps = result.Stages[0].Jobs[0].Steps;
        var step1 = steps[0] as TaskStep;

        Assert.That(step1, Is.Not.Null);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(step1.DisplayName, Is.EqualTo("Use .NET 8.0"));
            Assert.That(step1.ContinueOnError, Is.False);
            Assert.That(step1.TaskName, Is.EqualTo("UseDotNet@2"));
        }
    }

    [Test]
    public void VerifyStep2()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step2 = steps[1] as TaskStep;

        Assert.That(step2, Is.Not.Null);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(step2.DisplayName, Is.EqualTo("Restore dependencies"));
            Assert.That(step2.ContinueOnError, Is.False);
            Assert.That(step2.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
        }
    }

    [Test]
    public void VerifyStep3()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step3 = steps[2] as TaskStep;

        Assert.That(step3, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(step3.DisplayName, Is.EqualTo("Build"));
            Assert.That(step3.ContinueOnError, Is.False);
            Assert.That(step3.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
        }
    }

    [Test]
    public void VerifyStep4()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();
        
        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step4 = steps[3] as TaskStep;

        Assert.That(step4, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(step4.DisplayName, Is.EqualTo("Test"));
            Assert.That(step4.ContinueOnError, Is.False);
            Assert.That(step4.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
        }
    }
}
