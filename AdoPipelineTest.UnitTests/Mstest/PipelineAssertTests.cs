using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Mstest;
using Xunit;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Mstest;

public class PipelineAssertTests
{
    private PipelineTestResult _result = null!;

    public PipelineAssertTests()
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

    #region HasStage

    [Fact]
    public void HasStage_ByName_ShouldNotThrow()
    {
        _result.HasStage("Build");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasStage_ByDisplayName_ShouldNotThrow()
    {
        _result.HasStage("Build Stage");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasStage_NonExisting_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasStage("NonExistent"));
    }

    #endregion

    #region HasStageCount

    [Fact]
    public void HasStageCount_CorrectCount_ShouldNotThrow()
    {
        _result.HasStageCount(2);
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasStageCount_WrongCount_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasStageCount(5));
    }

    #endregion

    #region HasJob (PipelineTestResult, stageName, jobName)

    [Fact]
    public void HasJob_WithStageAndJob_ByName_ShouldNotThrow()
    {
        _result.HasJob("Build", "BuildJob");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasJob_WithStageAndJob_ByDisplayName_ShouldNotThrow()
    {
        _result.HasJob("Build Stage", "Build Job");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasJob_StageNotFound_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasJob("NonExistent", "BuildJob"));
    }

    [Fact]
    public void HasJob_JobNotFound_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasJob("Build", "NonExistent"));
    }

    #endregion

    #region HasJob (PipelineStage, jobName)

    [Fact]
    public void HasJob_OnStage_ByName_ShouldNotThrow()
    {
        _result.Stages[0].HasJob("BuildJob");
        Assert.NotNull(_result.Stages[0]);
    }

    [Fact]
    public void HasJob_OnStage_ByDisplayName_ShouldNotThrow()
    {
        _result.Stages[0].HasJob("Build Job");
        Assert.NotNull(_result.Stages[0]);
    }

    [Fact]
    public void HasJob_OnStage_NotFound_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.Stages[0].HasJob("NonExistent"));
    }

    #endregion

    #region HasStep

    [Fact]
    public void HasStep_ByDisplayName_ShouldNotThrow()
    {
        _result.HasStep("Use .NET");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasStep_NotFound_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasStep("NonExistent Step"));
    }

    [Fact]
    public void HasStep_WithPredicate_ShouldNotThrow()
    {
        _result.HasStep(s => s.DisplayName == "Use .NET");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasStep_WithPredicate_NoMatch_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasStep(s => s.DisplayName == "NonExistent"));
    }

    #endregion

    #region HasTask

    [Fact]
    public void HasTask_ByTaskName_ShouldNotThrow()
    {
        _result.HasTask("UseDotNet@2");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasTask_NotFound_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasTask("NonExistent@1"));
    }

    #endregion

    #region HasVariable

    [Fact]
    public void HasVariable_Existing_ShouldNotThrow()
    {
        _result.HasVariable("buildConfiguration");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasVariable_NotFound_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasVariable("nonExistent"));
    }

    [Fact]
    public void HasVariable_WithCorrectValue_ShouldNotThrow()
    {
        _result.HasVariable("buildConfiguration", "Release");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasVariable_WithWrongValue_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasVariable("buildConfiguration", "Debug"));
    }

    #endregion

    #region HasParameter

    [Fact]
    public void HasParameter_Existing_ShouldNotThrow()
    {
        _result.HasParameter("projectName");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasParameter_NotFound_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasParameter("nonExistent"));
    }

    #endregion

    #region HasTrigger

    [Fact]
    public void HasTrigger_WithTriggers_ShouldNotThrow()
    {
        _result.HasTrigger();
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasTrigger_NoTriggers_ShouldThrow()
    {
        var resultNoTriggers = new PipelineTestResult();
        Assert.Throws<AssertFailedException>(() => resultNoTriggers.HasTrigger());
    }

    #endregion

    #region TriggersIncludeBranch

    [Fact]
    public void TriggersIncludeBranch_ExistingBranch_ShouldNotThrow()
    {
        _result.TriggersIncludeBranch("main");
        Assert.NotNull(_result);
    }

    [Fact]
    public void TriggersIncludeBranch_NonExistingBranch_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.TriggersIncludeBranch("feature-branch"));
    }

    #endregion

    #region HasVmImage

    [Fact]
    public void HasVmImage_CorrectImage_ShouldNotThrow()
    {
        _result.HasVmImage("ubuntu-latest");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasVmImage_WrongImage_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasVmImage("windows-latest"));
    }

    [Fact]
    public void HasVmImage_NoPool_ShouldThrow()
    {
        var resultNoPool = new PipelineTestResult();
        Assert.Throws<AssertFailedException>(() => resultNoPool.HasVmImage("ubuntu-latest"));
    }

    #endregion

    #region HasScriptStep

    [Fact]
    public void HasScriptStep_ExistingPattern_ShouldNotThrow()
    {
        _result.HasScriptStep("dotnet test");
        Assert.NotNull(_result);
    }

    [Fact]
    public void HasScriptStep_NonExistingPattern_ShouldThrow()
    {
        Assert.Throws<AssertFailedException>(() => _result.HasScriptStep("npm build"));
    }

    #endregion
}
