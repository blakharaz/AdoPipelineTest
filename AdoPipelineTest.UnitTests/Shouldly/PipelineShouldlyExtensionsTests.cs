using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.UnitTests.Shouldly;

[TestFixture]
public class PipelineShouldlyExtensionsTests
{
    private PipelineTestResult _result = null!;

    [SetUp]
    public void Setup()
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

    #region ShouldHaveStage

    [Test]
    public void ShouldHaveStage_ExistingStageByDisplayName_DoesNotThrow()
    {
        _result.ShouldHaveStage("Build Stage");
    }

    [Test]
    public void ShouldHaveStage_ExistingStageByName_DoesNotThrow()
    {
        _result.ShouldHaveStage("Build");
    }

    [Test]
    public void ShouldHaveStage_NonExistingStage_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveStage("NonExistent");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Stage 'NonExistent' not found");
    }

    #endregion

    #region ShouldHaveStageCount

    [Test]
    public void ShouldHaveStageCount_CorrectCount_DoesNotThrow()
    {
        _result.ShouldHaveStageCount(2);
    }

    [Test]
    public void ShouldHaveStageCount_WrongCount_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveStageCount(5);
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Expected 5 stages, found 2");
    }

    #endregion

    #region ShouldHaveJob (on PipelineTestResult)

    [Test]
    public void ShouldHaveJob_ExistingJobByDisplayName_DoesNotThrow()
    {
        _result.ShouldHaveJob("Build Stage", "Build Job");
    }

    [Test]
    public void ShouldHaveJob_ExistingJobByName_DoesNotThrow()
    {
        _result.ShouldHaveJob("Build", "BuildJob");
    }

    [Test]
    public void ShouldHaveJob_NonExistingStage_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveJob("NonExistent", "Build Job");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Stage 'NonExistent' not found");
    }

    [Test]
    public void ShouldHaveJob_NonExistingJob_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveJob("Build Stage", "NonExistent");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Job 'NonExistent' not found");
    }

    #endregion

    #region ShouldHaveJob (on PipelineStage)

    [Test]
    public void ShouldHaveJobOnStage_ExistingJobByDisplayName_DoesNotThrow()
    {
        var stage = _result.Stages[0];
        stage.ShouldHaveJob("Build Job");
    }

    [Test]
    public void ShouldHaveJobOnStage_ExistingJobByName_DoesNotThrow()
    {
        var stage = _result.Stages[0];
        stage.ShouldHaveJob("BuildJob");
    }

    [Test]
    public void ShouldHaveJobOnStage_NonExistingJob_ThrowsShouldAssertException()
    {
        var stage = _result.Stages[0];
        var act = () => stage.ShouldHaveJob("NonExistent");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Job 'NonExistent' not found");
    }

    #endregion

    #region ShouldHaveJobCount

    [Test]
    public void ShouldHaveJobCount_CorrectCount_DoesNotThrow()
    {
        var stage = _result.Stages[0];
        stage.ShouldHaveJobCount(1);
    }

    [Test]
    public void ShouldHaveJobCount_WrongCount_ThrowsShouldAssertException()
    {
        var stage = _result.Stages[0];
        var act = () => stage.ShouldHaveJobCount(5);
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Expected 5 jobs, found 1");
    }

    #endregion

    #region ShouldHaveStep (by name)

    [Test]
    public void ShouldHaveStep_ExistingStep_DoesNotThrow()
    {
        _result.ShouldHaveStep("Use .NET");
    }

    [Test]
    public void ShouldHaveStep_NonExistingStep_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveStep("NonExistent");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Step 'NonExistent' not found");
    }

    #endregion

    #region ShouldHaveStep (by predicate)

    [Test]
    public void ShouldHaveStepByPredicate_MatchingStep_DoesNotThrow()
    {
        _result.ShouldHaveStep(s => s is ScriptStep);
    }

    [Test]
    public void ShouldHaveStepByPredicate_NoMatchingStep_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveStep(s => s.DisplayName == "NonExistent");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("No step matched the predicate");
    }

    #endregion

    #region ShouldHaveStepCount

    [Test]
    public void ShouldHaveStepCount_CorrectCount_DoesNotThrow()
    {
        var job = _result.Stages[0].Jobs[0];
        job.ShouldHaveStepCount(3);
    }

    [Test]
    public void ShouldHaveStepCount_WrongCount_ThrowsShouldAssertException()
    {
        var job = _result.Stages[0].Jobs[0];
        var act = () => job.ShouldHaveStepCount(10);
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Expected 10 steps, found 3");
    }

    #endregion

    #region ShouldHaveTask

    [Test]
    public void ShouldHaveTask_ExistingTask_DoesNotThrow()
    {
        _result.ShouldHaveTask("UseDotNet@2");
    }

    [Test]
    public void ShouldHaveTask_NonExistingTask_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveTask("NonExistent@1");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Task 'NonExistent@1' not found");
    }

    #endregion

    #region ShouldHaveVariable (existence)

    [Test]
    public void ShouldHaveVariable_ExistingVariable_DoesNotThrow()
    {
        _result.ShouldHaveVariable("buildConfiguration");
    }

    [Test]
    public void ShouldHaveVariable_NonExistingVariable_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveVariable("nonExistent");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Variable 'nonExistent' not found");
    }

    #endregion

    #region ShouldHaveVariable (with value)

    [Test]
    public void ShouldHaveVariableValue_CorrectValue_DoesNotThrow()
    {
        _result.ShouldHaveVariableValue("buildConfiguration", "Release");
    }

    [Test]
    public void ShouldHaveVariableValue_WrongValue_ThrowsShouldAssertException()
    {
        var ex = Should.Throw<ShouldAssertException>(() => _result.ShouldHaveVariableValue("buildConfiguration", "Debug"));
        ex.Message.ShouldNotBeNullOrEmpty();
    }

    [Test]
    public void ShouldHaveVariableValue_NonExistingVariable_ThrowsShouldAssertException()
    {
        var ex = Should.Throw<ShouldAssertException>(() => _result.ShouldHaveVariableValue("nonExistent", "value"));
        ex.Message.ShouldNotBeNullOrEmpty();
    }

    #endregion

    #region ShouldHaveParameter

    [Test]
    public void ShouldHaveParameter_ExistingParameter_DoesNotThrow()
    {
        _result.ShouldHaveParameter("projectName");
    }

    [Test]
    public void ShouldHaveParameter_NonExistingParameter_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveParameter("nonExistent");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Parameter 'nonExistent' not found");
    }

    #endregion

    #region ShouldHaveTrigger

    [Test]
    public void ShouldHaveTrigger_WithTriggers_DoesNotThrow()
    {
        _result.ShouldHaveTrigger();
    }

    [Test]
    public void ShouldHaveTrigger_NoTriggers_ThrowsShouldAssertException()
    {
        var resultWithoutTriggers = new PipelineTestResult
        {
            Stages = [],
            Variables = [],
            Parameters = new Dictionary<string, PipelineParameter>()
        };
        var act = () => resultWithoutTriggers.ShouldHaveTrigger();
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Pipeline has no triggers configured");
    }

    #endregion

    #region ShouldIncludeBranch

    [Test]
    public void ShouldIncludeBranch_ExistingBranch_DoesNotThrow()
    {
        _result.ShouldIncludeBranch("main");
    }

    [Test]
    public void ShouldIncludeBranch_NonExistingBranch_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldIncludeBranch("feature/xyz");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Branch 'feature/xyz' not in trigger branches");
    }

    [Test]
    public void ShouldIncludeBranch_NoTriggers_ThrowsShouldAssertException()
    {
        var resultWithoutTriggers = new PipelineTestResult
        {
            Stages = [],
            Variables = [],
            Parameters = new Dictionary<string, PipelineParameter>()
        };
        var act = () => resultWithoutTriggers.ShouldIncludeBranch("main");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Pipeline has no triggers configured");
    }

    #endregion

    #region ShouldHaveVmImage

    [Test]
    public void ShouldHaveVmImage_CorrectImage_DoesNotThrow()
    {
        _result.ShouldHaveVmImage("ubuntu-latest");
    }

    [Test]
    public void ShouldHaveVmImage_WrongImage_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveVmImage("windows-latest");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Expected VM image 'windows-latest', found 'ubuntu-latest'");
    }

    [Test]
    public void ShouldHaveVmImage_NoPool_ThrowsShouldAssertException()
    {
        var resultWithoutPool = new PipelineTestResult
        {
            Stages = [],
            Variables = [],
            Parameters = new Dictionary<string, PipelineParameter>()
        };
        var act = () => resultWithoutPool.ShouldHaveVmImage("ubuntu-latest");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Pipeline has no pool configured");
    }

    #endregion

    #region ShouldHaveScriptStepContaining

    [Test]
    public void ShouldHaveScriptStepContaining_ExistingPattern_DoesNotThrow()
    {
        _result.ShouldHaveScriptStepContaining("dotnet test");
    }

    [Test]
    public void ShouldHaveScriptStepContaining_NonExistingPattern_ThrowsShouldAssertException()
    {
        var act = () => _result.ShouldHaveScriptStepContaining("nonexistent pattern");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("No script step containing 'nonexistent pattern' found");
    }

    [Test]
    public void ShouldHaveScriptStepContaining_NoScriptSteps_ThrowsShouldAssertException()
    {
        var resultWithoutScripts = new PipelineTestResult
        {
            Stages =
            [
                new PipelineStage
                {
                    Jobs =
                    [
                        new PipelineJob
                        {
                            Steps =
                            [
                                new TaskStep { DisplayName = "Task", TaskName = "Task@1", Inputs = new Dictionary<string, string>() }
                            ]
                        }
                    ]
                }
            ],
            Variables = [],
            Parameters = new Dictionary<string, PipelineParameter>()
        };
        var act = () => resultWithoutScripts.ShouldHaveScriptStepContaining("anything");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("No script step containing 'anything' found");
    }

    #endregion

    #region Custom Message

    [Test]
    public void ShouldHaveStage_WithCustomMessage_UsesCustomMessage()
    {
        var act = () => _result.ShouldHaveStage("NonExistent", "My custom message");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("My custom message");
    }

    [Test]
    public void ShouldHaveVariable_WithCustomMessage_UsesCustomMessage()
    {
        var act = () => _result.ShouldHaveVariable("nonExistent", "Custom var message");
        act.ShouldThrow<ShouldAssertException>()
            .Message.ShouldContain("Custom var message");
    }

    #endregion
}
