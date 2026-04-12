using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.Samples.Shouldly.SimpleTemplates;

[TestFixture]
public class SimplePipelineWithStepTemplateTests
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

        result.Stages.Count.ShouldBe(1);
    }

    [Test]
    public void Build_Stage_Should_Have_One_Job()
    {
        var result = CreateTester().Run();

        var buildStage = result.Stages[0];
        buildStage.Jobs.Count.ShouldBe(1);
    }

    [Test]
    public void Build_Job_Should_Have_Correct_DisplayName()
    {
        var result = CreateTester().Run();

        var buildJob = result.Stages[0].Jobs[0];
        buildJob.DisplayName.ShouldBe("Build .NET Project");
    }

    [Test]
    public void Build_Job_Should_Include_All_Steps_From_Template()
    {
        var result = CreateTester().Run();

        var buildJob = result.Stages[0].Jobs[0];
        buildJob.Steps.Count.ShouldBe(6);
    }

    [Test]
    public void First_Step_Should_Be_UseDotNet_Task()
    {
        var result = CreateTester().Run();

        var firstStep = result.Stages[0].Jobs[0].Steps[0].ShouldBeOfType<TaskStep>();
        firstStep.DisplayName.ShouldBe("Install .NET SDK");
        firstStep.TaskName.ShouldBe("UseDotNet@2");
    }

    [Test]
    public void Second_Step_Should_Be_Restore_Task()
    {
        var result = CreateTester().Run();

        var secondStep = result.Stages[0].Jobs[0].Steps[1].ShouldBeOfType<TaskStep>();
        secondStep.DisplayName.ShouldBe("Restore NuGet packages");
        secondStep.TaskName.ShouldBe("DotNetCoreCLI@2");
    }

    [Test]
    public void Third_Step_Should_Be_Build_From_Template()
    {
        var result = CreateTester().Run();

        var thirdStep = result.Stages[0].Jobs[0].Steps[2].ShouldBeOfType<TaskStep>();
        thirdStep.DisplayName.ShouldBe("Build .NET Project");
        thirdStep.TaskName.ShouldBe("DotNetCoreCLI@2");
    }

    [Test]
    public void Fourth_Step_Should_Be_Publish_From_Template()
    {
        var result = CreateTester().Run();

        var fourthStep = result.Stages[0].Jobs[0].Steps[3].ShouldBeOfType<TaskStep>();
        fourthStep.DisplayName.ShouldBe("Publish Build Output");
        fourthStep.TaskName.ShouldBe("DotNetCoreCLI@2");
    }

    [Test]
    public void Fifth_Step_Should_Be_Test_Task()
    {
        var result = CreateTester().Run();

        var fifthStep = result.Stages[0].Jobs[0].Steps[4].ShouldBeOfType<TaskStep>();
        fifthStep.DisplayName.ShouldBe("Run Unit Tests");
        fifthStep.TaskName.ShouldBe("DotNetCoreCLI@2");
    }

    [Test]
    public void Sixth_Step_Should_Be_PublishArtifacts_Task()
    {
        var result = CreateTester().Run();

        var sixthStep = result.Stages[0].Jobs[0].Steps[5].ShouldBeOfType<TaskStep>();
        sixthStep.DisplayName.ShouldBe("Publish Artifacts");
        sixthStep.TaskName.ShouldBe("PublishBuildArtifacts@1");
    }

    [Test]
    public void All_Steps_Should_Have_ContinueOnError_Evaluated()
    {
        var result = CreateTester().Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        foreach (var step in steps)
        {
            step.ContinueOnError.ShouldBeFalse();
        }
    }

    [Test]
    public void Pipeline_Should_Have_AgentPool_Configured()
    {
        var result = CreateTester().Run();

        result.AgentPool.ShouldNotBeNull();
    }

    [Test]
    public void Pipeline_Should_Have_Triggers_Configured()
    {
        var result = CreateTester().Run();

        result.Triggers.ShouldNotBeNull();
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
            "Build .NET Project",
            "Publish Build Output",
            "Run Unit Tests",
            "Publish Artifacts"
        };
        
        stepDisplayNames.ShouldBe(expectedOrder);
    }

    [Test]
    public void All_Steps_Should_Be_TaskSteps()
    {
        var result = CreateTester().Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        foreach (var step in steps)
        {
            step.ShouldBeOfType<TaskStep>();
        }
    }
}
