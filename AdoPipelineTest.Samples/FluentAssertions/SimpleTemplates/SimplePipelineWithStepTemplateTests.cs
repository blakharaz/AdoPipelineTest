using AdoPipelineTest.PipelineAssertions;
using AdoPipelineTest.Model.Steps;
using FluentAssertions;
using TestClass = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestMethod = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;

namespace AdoPipelineTest.Samples.FluentAssertions.SimpleTemplates;

[TestClass]
public class SimplePipelineWithStepTemplateTests
{
    private static PipelineTester CreateTester()
    {
        return new PipelineTester()
            .WithPipeline("pipelines/SimpleTemplates/simple_pipeline_with_step_template.yaml");
    }

    [TestMethod]
    public void Pipeline_Should_Have_One_Stage()
    {
        var result = CreateTester().Run();

        result.Should().HaveStageCount(1);
    }

    [TestMethod]
    public void Build_Stage_Should_Have_One_Job()
    {
        var result = CreateTester().Run();

        var buildStage = result.Stages[0];
        buildStage.Jobs.Should().HaveCount(1);
    }

    [TestMethod]
    public void Build_Job_Should_Have_Correct_DisplayName()
    {
        var result = CreateTester().Run();

        var buildJob = result.Stages[0].Jobs[0];
        buildJob.DisplayName.Should().Be("Build .NET Project");
    }

    [TestMethod]
    public void Build_Job_Should_Include_All_Steps_From_Template()
    {
        var result = CreateTester().Run();

        var buildJob = result.Stages[0].Jobs[0];
        buildJob.Steps.Should().HaveCount(6);
    }

    [TestMethod]
    public void First_Step_Should_Be_UseDotNet_Task()
    {
        var result = CreateTester().Run();

        var firstStep = result.Stages[0].Jobs[0].Steps[0];
        firstStep.Should().BeOfType<TaskStep>();
        
        var taskStep = (TaskStep)firstStep;
        taskStep.DisplayName.Should().Be("Install .NET SDK");
        taskStep.TaskName.Should().Be("UseDotNet@2");
    }

    [TestMethod]
    public void Second_Step_Should_Be_Restore_Task()
    {
        var result = CreateTester().Run();

        var secondStep = result.Stages[0].Jobs[0].Steps[1];
        secondStep.Should().BeOfType<TaskStep>();
        
        var taskStep = (TaskStep)secondStep;
        taskStep.DisplayName.Should().Be("Restore NuGet packages");
        taskStep.TaskName.Should().Be("DotNetCoreCLI@2");
    }

    [TestMethod]
    public void Third_Step_Should_Be_Build_From_Template()
    {
        var result = CreateTester().Run();

        var thirdStep = result.Stages[0].Jobs[0].Steps[2];
        thirdStep.Should().BeOfType<TaskStep>();
        
        var taskStep = (TaskStep)thirdStep;
        taskStep.DisplayName.Should().Be("Build .NET Project");
        taskStep.TaskName.Should().Be("DotNetCoreCLI@2");
    }

    [TestMethod]
    public void Fourth_Step_Should_Be_Publish_From_Template()
    {
        var result = CreateTester().Run();

        var fourthStep = result.Stages[0].Jobs[0].Steps[3];
        fourthStep.Should().BeOfType<TaskStep>();
        
        var taskStep = (TaskStep)fourthStep;
        taskStep.DisplayName.Should().Be("Publish Build Output");
        taskStep.TaskName.Should().Be("DotNetCoreCLI@2");
    }

    [TestMethod]
    public void Fifth_Step_Should_Be_Test_Task()
    {
        var result = CreateTester().Run();

        var fifthStep = result.Stages[0].Jobs[0].Steps[4];
        fifthStep.Should().BeOfType<TaskStep>();
        
        var taskStep = (TaskStep)fifthStep;
        taskStep.DisplayName.Should().Be("Run Unit Tests");
        taskStep.TaskName.Should().Be("DotNetCoreCLI@2");
    }

    [TestMethod]
    public void Sixth_Step_Should_Be_PublishArtifacts_Task()
    {
        var result = CreateTester().Run();

        var sixthStep = result.Stages[0].Jobs[0].Steps[5];
        sixthStep.Should().BeOfType<TaskStep>();
        
        var taskStep = (TaskStep)sixthStep;
        taskStep.DisplayName.Should().Be("Publish Artifacts");
        taskStep.TaskName.Should().Be("PublishBuildArtifacts@1");
    }

    [TestMethod]
    public void All_Steps_Should_Have_ContinueOnError_Evaluated()
    {
        var result = CreateTester().Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        steps.Should().AllSatisfy(step => step.ContinueOnError.Should().BeFalse());
    }

    [TestMethod]
    public void Pipeline_Should_Have_AgentPool_Configured()
    {
        var result = CreateTester().Run();

        result.AgentPool!.Should().HaveVmImage("ubuntu-latest");
    }

    [TestMethod]
    public void Pipeline_Should_Have_Triggers_Configured()
    {
        var result = CreateTester().Run();

        result.Should().HaveTriggers();
    }

    [TestMethod]
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
        
        stepDisplayNames.Should().Equal(expectedOrder);
    }

    [TestMethod]
    public void All_Steps_Should_Be_TaskSteps()
    {
        var result = CreateTester().Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        steps.Should().AllBeOfType<TaskStep>();
    }
}
