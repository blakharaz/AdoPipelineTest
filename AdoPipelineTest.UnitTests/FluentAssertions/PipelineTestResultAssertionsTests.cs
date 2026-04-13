using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using FluentAssertions;
using AdoPipelineTest.PipelineAssertions;
using Xunit;

namespace AdoPipelineTest.UnitTests.FluentAssertions;

public class PipelineTestResultAssertionsTests
{
    private PipelineTestResult _result = null!;

    public PipelineTestResultAssertionsTests()
    {
        _result = new PipelineTestResult
        {
            Triggers = new PipelineTriggers { IncludedBranches = ["main", "develop"] },
            AgentPool = new PipelineAgentPool { VmImage = "ubuntu-latest" },
            Variables =
            [
                new PipelineVariable { Name = "buildConfiguration", DefaultValue = "Release" },
                new PipelineVariable { Name = "version", DefaultValue = "1.0.0" }
            ],
            Parameters = new Dictionary<string, PipelineParameter>
            {
                ["projectName"] = new PipelineParameter { Name = "projectName", Value = "MyProject" },
                ["enableTests"] = new PipelineParameter { Name = "enableTests", Value = true }
            },
            Stages =
            [
                new PipelineStage
                {
                    Name = "Build",
                    DisplayName = "Build Stage",
                    Jobs =
                    [
                        new PipelineJob
                        {
                            Name = "BuildJob",
                            DisplayName = "Build Job",
                            Steps =
                            [
                                new TaskStep { DisplayName = "Use .NET", TaskName = "UseDotNet@2", Inputs = new Dictionary<string, string>() },
                                new TaskStep { DisplayName = "Restore", TaskName = "DotNetCoreCLI@2", Inputs = new Dictionary<string, string>() },
                                new ScriptStep { DisplayName = "Run Tests", Script = "dotnet test --configuration Release" }
                            ]
                        }
                    ]
                },
                new PipelineStage
                {
                    Name = "Deploy",
                    DisplayName = "Deploy Stage",
                    Jobs =
                    [
                        new PipelineJob
                        {
                            Name = "DeployJob",
                            DisplayName = "Deploy Job",
                            Steps =
                            [
                                new TaskStep { DisplayName = "Deploy", TaskName = "AzureWebApp@1", Inputs = new Dictionary<string, string>() }
                            ]
                        }
                    ]
                }
            ]
        };
    }

    #region HaveStage

    [Fact]
    public void HaveStage_ExistingStageByName_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveStage("Build");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveStage_ExistingStageByDisplayName_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveStage("Build Stage");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveStage_NonExistingStage_ShouldThrow()
    {
        var act = () => _result.Should().HaveStage("NonExistent");
        act.Should().Throw<Exception>();
    }

    #endregion

    #region HaveStageCount

    [Fact]
    public void HaveStageCount_CorrectCount_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveStageCount(2);
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveStageCount_WrongCount_ShouldThrow()
    {
        var act = () => _result.Should().HaveStageCount(5);
        act.Should().Throw<Exception>();
    }

    #endregion

    #region HaveJob

    [Fact]
    public void HaveJob_ExistingJobByStageAndJobName_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveJob("Build", "BuildJob");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveJob_NonExistingJob_ShouldThrow()
    {
        var act = () => _result.Should().HaveJob("Build", "NonExistentJob");
        act.Should().Throw<Exception>();
    }

    #endregion

    #region HaveStep

    [Fact]
    public void HaveStep_ExistingStep_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveStep("Use .NET");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveStep_NonExistingStep_ShouldThrow()
    {
        var act = () => _result.Should().HaveStep("NonExistent Step");
        act.Should().Throw<Exception>();
    }

    #endregion

    #region HaveTask

    [Fact]
    public void HaveTask_ExistingTask_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveTask("UseDotNet@2");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveTask_MultipleTasks_ShouldNotThrow()
    {
        _result.Should().HaveTask("UseDotNet@2");
        _result.Should().HaveTask("DotNetCoreCLI@2");
        _result.Should().HaveTask("AzureWebApp@1");
    }

    [Fact]
    public void HaveTask_NonExistingTask_ShouldThrow()
    {
        var act = () => _result.Should().HaveTask("NonExistent@1");
        act.Should().Throw<Exception>();
    }

    #endregion

    #region HaveVariable

    [Fact]
    public void HaveVariable_ExistingVariable_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveVariable("buildConfiguration");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveVariable_WithCorrectValue_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveVariable("buildConfiguration", "Release");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveVariable_NonExistingVariable_ShouldThrow()
    {
        var act = () => _result.Should().HaveVariable("nonExistent");
        act.Should().Throw<Exception>();
    }

    #endregion

    #region HaveParameter

    [Fact]
    public void HaveParameter_ExistingParameter_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveParameter("projectName");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveParameter_NonExistingParameter_ShouldThrow()
    {
        var act = () => _result.Should().HaveParameter("nonExistent");
        act.Should().Throw<Exception>();
    }

    #endregion

    #region HaveTrigger

    [Fact]
    public void HaveTrigger_WithTriggers_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveTrigger();
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveTriggers_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveTriggers();
        act.Should().NotThrow();
    }

    #endregion

    #region IncludeBranch (via PipelineTriggersAssertions)

    [Fact]
    public void IncludeBranch_ExistingBranch_ShouldNotThrow()
    {
        var act = () => _result.Triggers!.Should().IncludeBranch("main");
        act.Should().NotThrow();
    }

    [Fact]
    public void IncludeBranch_NonExistingBranch_ShouldThrow()
    {
        var act = () => _result.Triggers!.Should().IncludeBranch("feature-branch");
        act.Should().Throw<Exception>();
    }

    #endregion

    #region HaveVmImage

    [Fact]
    public void HaveVmImage_CorrectImage_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveVmImage("ubuntu-latest");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveVmImage_WrongImage_ShouldThrow()
    {
        var act = () => _result.Should().HaveVmImage("windows-latest");
        act.Should().Throw<Exception>();
    }

    #endregion

    #region HaveScriptStepContaining

    [Fact]
    public void HaveScriptStepContaining_ExistingPattern_ShouldNotThrow()
    {
        var act = () => _result.Should().HaveScriptStepContaining("dotnet test");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveScriptStepContaining_NonExistingPattern_ShouldThrow()
    {
        var act = () => _result.Should().HaveScriptStepContaining("npm build");
        act.Should().Throw<Exception>();
    }

    #endregion
}
