using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdoPipelineTest.Mstest;
using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.Samples.Mstest.SimpleTemplates;

[TestClass]
public class SimplePipelineWithStepTemplateTests
{
    private static PipelineTester CreateTester()
    {
        return new PipelineTester()
            .WithPipeline("SimpleTemplates/simple_pipeline_with_step_template.yaml");
    }

    [TestMethod]
    public void Pipeline_Should_Have_One_Stage()
    {
        var result = CreateTester().Run();
        result.HasStageCount(1);
    }

    [TestMethod]
    public void Build_Stage_Should_Have_One_Job()
    {
        var result = CreateTester().Run();
        var buildStage = result.Stages[0];
        Assert.AreEqual(1, buildStage.Jobs.Count);
    }

    [TestMethod]
    public void Build_Job_Should_Have_Correct_DisplayName()
    {
        var result = CreateTester().Run();
        var buildJob = result.Stages[0].Jobs[0];
        Assert.AreEqual("Build .NET Project", buildJob.DisplayName);
    }

    [TestMethod]
    public void Build_Job_Should_Include_All_Steps_From_Template()
    {
        var result = CreateTester().Run();
        var buildJob = result.Stages[0].Jobs[0];
        Assert.AreEqual(6, buildJob.Steps.Count);
    }

    [TestMethod]
    public void First_Step_Should_Be_UseDotNet_Task()
    {
        var result = CreateTester().Run();
        var firstStep = result.Stages[0].Jobs[0].Steps[0] as TaskStep;
        Assert.IsNotNull(firstStep);
        Assert.AreEqual("Install .NET SDK", firstStep.DisplayName);
        Assert.AreEqual("UseDotNet@2", firstStep.TaskName);
    }

    [TestMethod]
    public void Second_Step_Should_Be_Restore_Task()
    {
        var result = CreateTester().Run();
        var secondStep = result.Stages[0].Jobs[0].Steps[1] as TaskStep;
        Assert.IsNotNull(secondStep);
        Assert.AreEqual("Restore NuGet packages", secondStep.DisplayName);
        Assert.AreEqual("DotNetCoreCLI@2", secondStep.TaskName);
    }

    [TestMethod]
    public void Third_Step_Should_Be_Build_From_Template()
    {
        var result = CreateTester().Run();
        var thirdStep = result.Stages[0].Jobs[0].Steps[2] as TaskStep;
        Assert.IsNotNull(thirdStep);
        Assert.AreEqual("Build .NET Project", thirdStep.DisplayName);
        Assert.AreEqual("DotNetCoreCLI@2", thirdStep.TaskName);
    }

    [TestMethod]
    public void Fourth_Step_Should_Be_Publish_From_Template()
    {
        var result = CreateTester().Run();
        var fourthStep = result.Stages[0].Jobs[0].Steps[3] as TaskStep;
        Assert.IsNotNull(fourthStep);
        Assert.AreEqual("Publish Build Output", fourthStep.DisplayName);
        Assert.AreEqual("DotNetCoreCLI@2", fourthStep.TaskName);
    }

    [TestMethod]
    public void Fifth_Step_Should_Be_Test_Task()
    {
        var result = CreateTester().Run();
        var fifthStep = result.Stages[0].Jobs[0].Steps[4] as TaskStep;
        Assert.IsNotNull(fifthStep);
        Assert.AreEqual("Run Unit Tests", fifthStep.DisplayName);
        Assert.AreEqual("DotNetCoreCLI@2", fifthStep.TaskName);
    }

    [TestMethod]
    public void Sixth_Step_Should_Be_PublishArtifacts_Task()
    {
        var result = CreateTester().Run();
        var sixthStep = result.Stages[0].Jobs[0].Steps[5] as TaskStep;
        Assert.IsNotNull(sixthStep);
        Assert.AreEqual("Publish Artifacts", sixthStep.DisplayName);
        Assert.AreEqual("PublishBuildArtifacts@1", sixthStep.TaskName);
    }

    [TestMethod]
    public void All_Steps_Should_Have_ContinueOnError_False()
    {
        var result = CreateTester().Run();
        var steps = result.Stages[0].Jobs[0].Steps;
        foreach (var step in steps)
        {
            Assert.IsFalse(step.ContinueOnError);
        }
    }

    [TestMethod]
    public void Pipeline_Should_Have_AgentPool_Configured()
    {
        var result = CreateTester().Run();
        result.HasVmImage("ubuntu-latest");
    }

    [TestMethod]
    public void Pipeline_Should_Have_Triggers_Configured()
    {
        var result = CreateTester().Run();
        result.HasTrigger();
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

        CollectionAssert.AreEqual(expectedOrder, stepDisplayNames);
    }

    [TestMethod]
    public void All_Steps_Should_Be_TaskSteps()
    {
        var result = CreateTester().Run();
        var steps = result.Stages[0].Jobs[0].Steps;
        foreach (var step in steps)
        {
            Assert.IsInstanceOfType<TaskStep>(step);
        }
    }
}
