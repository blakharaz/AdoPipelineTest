using AdoPipelineTest.Model.Steps;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace AdoPipelineTest.Samples.Nunit.NestedTemplates;

using Is = AdoPipelineTest.Nunit.Is;

[TestFixture]
public class NestedTemplatesTest
{
    private static PipelineTester CreatePipelineTester()
    {
        return new PipelineTester().WithPipeline("pipelines/NestedTemplates/nested_pipeline.yaml");
    }

    [Test]
    public void VerifyBasics()
    {
        var result = CreatePipelineTester().Run();
        
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Triggers, Is.BranchIncluded("main"));
            Assert.That(result.AgentPool, Is.VmImage("ubuntu-latest"));
            Assert.That(result.Stages, Has.Count.EqualTo(1));
            Assert.That(result.Stages[0].Jobs, Has.Count.EqualTo(1));

            var steps = result.Stages[0].Jobs[0].Steps;
            Assert.That(steps, Has.Count.EqualTo(6));
        }
    }

    [Test]
    public void VerifyStep1_InstallDotNetSdk()
    {
        var result = CreatePipelineTester().Run();
        
        Assert.That(result, Is.Not.Null);

        var steps = result.Stages[0].Jobs[0].Steps;
        var step1 = steps[0] as TaskStep;

        Assert.That(step1, Is.Not.Null);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(step1.DisplayName, Is.EqualTo("Install .NET SDK"));
            Assert.That(step1.ContinueOnError, Is.False);
            Assert.That(step1.TaskName, Is.EqualTo("UseDotNet@2"));
        }
    }

    [Test]
    public void VerifyStep2_RestoreNuGetPackages()
    {
        var result = CreatePipelineTester().Run();
        
        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step2 = steps[1] as TaskStep;

        Assert.That(step2, Is.Not.Null);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(step2.DisplayName, Is.EqualTo("Restore NuGet packages"));
            Assert.That(step2.ContinueOnError, Is.False);
            Assert.That(step2.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
        }
    }

    [Test]
    public void VerifyStep3_BuildDotNetProject()
    {
        var result = CreatePipelineTester().Run();
        
        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step3 = steps[2] as TaskStep;

        Assert.That(step3, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(step3.DisplayName, Is.EqualTo("Build .NET Project"));
            Assert.That(step3.ContinueOnError, Is.False);
            Assert.That(step3.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
        }
    }

    [Test]
    public void VerifyStep4_PublishBuildOutput()
    {
        var result = CreatePipelineTester().Run();
        
        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step4 = steps[3] as TaskStep;

        Assert.That(step4, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(step4.DisplayName, Is.EqualTo("Publish Build Output"));
            Assert.That(step4.ContinueOnError, Is.False);
            Assert.That(step4.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
        }
    }

    [Test]
    public void VerifyStep5_RunUnitTests()
    {
        var result = CreatePipelineTester().Run();
        
        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step5 = steps[4] as TaskStep;

        Assert.That(step5, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(step5.DisplayName, Is.EqualTo("Run Unit Tests"));
            Assert.That(step5.ContinueOnError, Is.False);
            Assert.That(step5.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
        }
    }

    [Test]
    public void VerifyStep6_PublishArtifacts()
    {
        var result = CreatePipelineTester().Run();
        
        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step6 = steps[5] as TaskStep;

        Assert.That(step6, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(step6.DisplayName, Is.EqualTo("Publish Artifacts"));
            Assert.That(step6.ContinueOnError, Is.False);
            Assert.That(step6.TaskName, Is.EqualTo("PublishBuildArtifacts@1"));
        }
    }
}