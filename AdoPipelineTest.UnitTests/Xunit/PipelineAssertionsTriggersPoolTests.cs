using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using Xunit;
using PipelineAssert = AdoPipelineTest.Xunit.Assert;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Xunit;

public class PipelineAssertionsTriggersPoolTests
{
    [Fact]
    public void HasTrigger_WhenTriggersDefined_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Triggers = new PipelineTriggers()
        };

        var ex = Record.Exception(() => PipelineAssert.HasTrigger(result));
        Assert.Null(ex);
    }

    [Fact]
    public void HasTrigger_WhenTriggersNull_Throws()
    {
        var result = new PipelineTestResult();

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasTrigger(result));
    }

    [Fact]
    public void TriggersIncludeBranch_WhenBranchExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Triggers = new PipelineTriggers { IncludedBranches = ["main", "develop"] }
        };

        var ex = Record.Exception(() => PipelineAssert.TriggersIncludeBranch(result, "main"));
        Assert.Null(ex);
    }

    [Fact]
    public void TriggersIncludeBranch_WhenTriggersNull_Throws()
    {
        var result = new PipelineTestResult();

        Assert.ThrowsAny<Exception>(() => PipelineAssert.TriggersIncludeBranch(result, "main"));
    }

    [Fact]
    public void TriggersIncludeBranch_WhenBranchMissing_Throws()
    {
        var result = new PipelineTestResult
        {
            Triggers = new PipelineTriggers { IncludedBranches = ["main"] }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.TriggersIncludeBranch(result, "develop"));
    }

    [Fact]
    public void HasVmImage_WhenImageMatches_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            AgentPool = new PipelineAgentPool { VmImage = "ubuntu-latest" }
        };

        var ex = Record.Exception(() => PipelineAssert.HasVmImage(result, "ubuntu-latest"));
        Assert.Null(ex);
    }

    [Fact]
    public void HasVmImage_WhenPoolNull_Throws()
    {
        var result = new PipelineTestResult();

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasVmImage(result, "ubuntu-latest"));
    }

    [Fact]
    public void HasVmImage_WhenImageDiffers_Throws()
    {
        var result = new PipelineTestResult
        {
            AgentPool = new PipelineAgentPool { VmImage = "ubuntu-latest" }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasVmImage(result, "windows-latest"));
    }

    [Fact]
    public void HasScriptStep_WhenScriptExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            Steps = new List<PipelineStep>
                            {
                                new ScriptStep { Script = "dotnet test" }
                            }
                        }
                    }
                }
            }
        };

        var ex = Record.Exception(() => PipelineAssert.HasScriptStep(result, "dotnet test"));
        Assert.Null(ex);
    }

    [Fact]
    public void HasScriptStep_WhenNoScriptSteps_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            Steps = new List<PipelineStep>
                            {
                                new TaskStep { TaskName = "DotNetCoreCLI@2", DisplayName = "Test Task" }
                            }
                        }
                    }
                }
            }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasScriptStep(result, "dotnet test"));
    }

    [Fact]
    public void HasScriptStep_WhenPatternNotFound_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            Steps = new List<PipelineStep>
                            {
                                new ScriptStep { Script = "npm start" }
                            }
                        }
                    }
                }
            }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasScriptStep(result, "dotnet test"));
    }

    [Fact]
    public void StepCount_WhenCountMatches_DoesNotThrow()
    {
        var job = new PipelineJob
        {
            Steps = new List<PipelineStep> { new(), new(), new() }
        };

        var ex = Record.Exception(() => PipelineAssert.StepCount(job, 3));
        Assert.Null(ex);
    }

    [Fact]
    public void StepCount_WhenCountDiffers_Throws()
    {
        var job = new PipelineJob
        {
            Steps = new List<PipelineStep> { new() }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.StepCount(job, 3));
    }

    [Fact]
    public void TaskHasInput_WhenInputExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            Steps = new List<PipelineStep>
                            {
                                new TaskStep
                                {
                                    TaskName = "DotNetCoreCLI@2",
                                    Inputs = new Dictionary<string, string>
                                    {
                                        ["command"] = "test"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var ex = Record.Exception(() => PipelineAssert.TaskHasInput(result, "DotNetCoreCLI@2", "command"));
        Assert.Null(ex);
    }

    [Fact]
    public void TaskHasInput_WhenInputValueMatches_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            Steps = new List<PipelineStep>
                            {
                                new TaskStep
                                {
                                    TaskName = "DotNetCoreCLI@2",
                                    Inputs = new Dictionary<string, string>
                                    {
                                        ["command"] = "test"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var ex = Record.Exception(() => PipelineAssert.TaskHasInput(result, "DotNetCoreCLI@2", "command", "test"));
        Assert.Null(ex);
    }

    [Fact]
    public void TaskHasInput_WhenTaskMissing_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            Steps = new List<PipelineStep>
                            {
                                new TaskStep { TaskName = "UseDotNet@2" }
                            }
                        }
                    }
                }
            }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.TaskHasInput(result, "DotNetCoreCLI@2", "command"));
    }

    [Fact]
    public void TaskHasInput_WhenInputKeyMissing_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            Steps = new List<PipelineStep>
                            {
                                new TaskStep
                                {
                                    TaskName = "DotNetCoreCLI@2",
                                    Inputs = new Dictionary<string, string>()
                                }
                            }
                        }
                    }
                }
            }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.TaskHasInput(result, "DotNetCoreCLI@2", "command"));
    }

    [Fact]
    public void TaskHasInput_WhenValueDiffers_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            Steps = new List<PipelineStep>
                            {
                                new TaskStep
                                {
                                    TaskName = "DotNetCoreCLI@2",
                                    Inputs = new Dictionary<string, string>
                                    {
                                        ["command"] = "build"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.TaskHasInput(result, "DotNetCoreCLI@2", "command", "test"));
    }

    [Fact]
    public void HasStep_WithPredicate_WhenMatchExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            Steps = new List<PipelineStep>
                            {
                                new TaskStep { TaskName = "UseDotNet@2", DisplayName = "Use .NET" }
                            }
                        }
                    }
                }
            }
        };

        var ex = Record.Exception(() => PipelineAssert.HasStep(result, s => s.DisplayName == "Use .NET"));
        Assert.Null(ex);
    }

    [Fact]
    public void HasStep_WithPredicate_WhenNoMatch_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            Steps = new List<PipelineStep>
                            {
                                new TaskStep { TaskName = "UseDotNet@2", DisplayName = "Use .NET" }
                            }
                        }
                    }
                }
            }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasStep(result, s => s.DisplayName == "Missing"));
    }
}