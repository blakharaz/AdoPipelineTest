using NUnit.Framework;
using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.Samples.Nunit.SimpleTemplates;

[TestFixture]
public class SimplePipelineWithStepTemplateTest
{
    private static PipelineTester CreateTester()
    {
        return new PipelineTester()
            .WithPipeline("pipelines/SimpleTemplates/simple_pipeline_with_step_template.yaml");
    }

    [Test]
    public void Pipeline_Should_Have_One_Stage()
    {
        var result = CreateTester().Run();

        Assert.That(result.Stages, Has.Count.EqualTo(1));
    }

    [Test]
    public void Build_Stage_Should_Have_One_Job()
    {
        var result = CreateTester().Run();

        var buildStage = result.Stages[0];
        Assert.That(buildStage.Jobs, Has.Count.EqualTo(1));
    }

    [Test]
    public void Build_Job_Should_Have_Correct_DisplayName()
    {
        var result = CreateTester().Run();

        var buildJob = result.Stages[0].Jobs[0];
        Assert.That(buildJob.DisplayName, Is.EqualTo("Build .NET Project"));
    }

    [Test]
    public void Build_Job_Should_Include_All_Steps_From_Template()
    {
        var result = CreateTester().Run();

        var buildJob = result.Stages[0].Jobs[0];
        
        // 5 steps: UseDotNet + Restore + 2 from template + Test + PublishArtifacts
        Assert.That(buildJob.Steps, Has.Count.EqualTo(6));
    }

    [Test]
    public void First_Step_Should_Be_UseDotNet_Task()
    {
        var result = CreateTester().Run();

        var firstStep = result.Stages[0].Jobs[0].Steps[0];
        Assert.That(firstStep, Is.TypeOf<TaskStep>());
        
        var taskStep = (TaskStep)firstStep;
        Assert.That(taskStep.DisplayName, Is.EqualTo("Install .NET SDK"));
        Assert.That(taskStep.TaskName, Is.EqualTo("UseDotNet@2"));
    }

    [Test]
    public void Second_Step_Should_Be_Restore_Task()
    {
        var result = CreateTester().Run();

        var secondStep = result.Stages[0].Jobs[0].Steps[1];
        Assert.That(secondStep, Is.TypeOf<TaskStep>());
        
        var taskStep = (TaskStep)secondStep;
        Assert.That(taskStep.DisplayName, Is.EqualTo("Restore NuGet packages"));
        Assert.That(taskStep.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
    }

    [Test]
    public void Third_Step_Should_Be_Build_From_Template()
    {
        var result = CreateTester().Run();

        var thirdStep = result.Stages[0].Jobs[0].Steps[2];
        Assert.That(thirdStep, Is.TypeOf<TaskStep>());
        
        var taskStep = (TaskStep)thirdStep;
        Assert.That(taskStep.DisplayName, Is.EqualTo("Build .NET Project"));
        Assert.That(taskStep.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
    }

    [Test]
    public void Fourth_Step_Should_Be_Publish_From_Template()
    {
        var result = CreateTester().Run();

        var fourthStep = result.Stages[0].Jobs[0].Steps[3];
        Assert.That(fourthStep, Is.TypeOf<TaskStep>());
        
        var taskStep = (TaskStep)fourthStep;
        Assert.That(taskStep.DisplayName, Is.EqualTo("Publish Build Output"));
        Assert.That(taskStep.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
    }

    [Test]
    public void Fifth_Step_Should_Be_Test_Task()
    {
        var result = CreateTester().Run();

        var fifthStep = result.Stages[0].Jobs[0].Steps[4];
        Assert.That(fifthStep, Is.TypeOf<TaskStep>());
        
        var taskStep = (TaskStep)fifthStep;
        Assert.That(taskStep.DisplayName, Is.EqualTo("Run Unit Tests"));
        Assert.That(taskStep.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
    }

    [Test]
    public void Sixth_Step_Should_Be_PublishArtifacts_Task()
    {
        var result = CreateTester().Run();

        var sixthStep = result.Stages[0].Jobs[0].Steps[5];
        Assert.That(sixthStep, Is.TypeOf<TaskStep>());
        
        var taskStep = (TaskStep)sixthStep;
        Assert.That(taskStep.DisplayName, Is.EqualTo("Publish Artifacts"));
        Assert.That(taskStep.TaskName, Is.EqualTo("PublishBuildArtifacts@1"));
    }

    [Test]
    public void All_Steps_Should_Have_ContinueOnError_Evaluated()
    {
        var result = CreateTester().Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        foreach (var step in steps)
        {
            Assert.That(step.ContinueOnError, Is.False);
        }
    }

    [Test]
    public void Pipeline_Should_Have_AgentPool_Configured()
    {
        var result = CreateTester().Run();

        Assert.That(result.AgentPool, Is.Not.Null);
    }

    [Test]
    public void Pipeline_Should_Have_Triggers_Configured()
    {
        var result = CreateTester().Run();

        Assert.That(result.Triggers, Is.Not.Null);
    }

    [Test]
    public void Template_Steps_Should_Be_Properly_Evaluated_And_Ordered()
    {
        var result = CreateTester().Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var stepDisplayNames = steps.Select(s => s.DisplayName).ToList();
        
        var expectedOrder = new[]
        {
            "Install .NET SDK",
            "Restore NuGet packages",
            "Build .NET Project",           // from template
            "Publish Build Output",         // from template
            "Run Unit Tests",
            "Publish Artifacts"
        };
        
        Assert.That(stepDisplayNames, Is.EqualTo(expectedOrder));
    }

    [Test]
    public void All_Steps_Should_Be_TaskSteps()
    {
        var result = CreateTester().Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        foreach (var step in steps)
        {
            Assert.That(step, Is.TypeOf<TaskStep>());
        }
    }
}