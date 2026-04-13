using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using Xunit;
using PipelineAssert = AdoPipelineTest.Xunit.Assert;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Xunit;

public class PipelineAssertionsStepTests
{
    [Fact]
    public void HasTask_WhenTaskExists_DoesNotThrow()
    {
        var result = CreateResultWithTask("UseDotNet@2");

        var ex = Record.Exception(() => PipelineAssert.HasTask(result, "UseDotNet@2"));
        Assert.Null(ex);
    }

    [Fact]
    public void HasTask_WhenTaskDoesNotExist_Throws()
    {
        var result = CreateResultWithTask("UseDotNet@2");

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasTask(result, "NonExistent@1"));
    }

    [Fact]
    public void TaskHasInput_WhenInputExists_DoesNotThrow()
    {
        var result = CreateResultWithTaskAndInput("UseDotNet@2", "version", "8.0.x");

        var ex = Record.Exception(() => PipelineAssert.TaskHasInput(result, "UseDotNet@2", "version"));
        Assert.Null(ex);
    }

    [Fact]
    public void TaskHasInput_WhenInputExistsAndValueMatches_DoesNotThrow()
    {
        var result = CreateResultWithTaskAndInput("UseDotNet@2", "version", "8.0.x");

        var ex = Record.Exception(() => PipelineAssert.TaskHasInput(result, "UseDotNet@2", "version", "8.0.x"));
        Assert.Null(ex);
    }

    [Fact]
    public void TaskHasInput_WhenInputExistsButValueDiffers_Throws()
    {
        var result = CreateResultWithTaskAndInput("UseDotNet@2", "version", "8.0.x");

        Assert.ThrowsAny<Exception>(() => PipelineAssert.TaskHasInput(result, "UseDotNet@2", "version", "9.0.x"));
    }

    [Fact]
    public void TaskHasInput_WhenInputDoesNotExist_Throws()
    {
        var result = CreateResultWithTaskAndInput("UseDotNet@2", "version", "8.0.x");

        Assert.ThrowsAny<Exception>(() => PipelineAssert.TaskHasInput(result, "UseDotNet@2", "nonexistent"));
    }

    [Fact]
    public void HasScriptStep_WhenScriptContainsContent_DoesNotThrow()
    {
        var result = CreateResultWithScriptStep("echo hello world");

        var ex = Record.Exception(() => PipelineAssert.HasScriptStep(result, "hello"));
        Assert.Null(ex);
    }

    [Fact]
    public void HasScriptStep_WhenScriptDoesNotContainContent_Throws()
    {
        var result = CreateResultWithScriptStep("echo hello world");

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasScriptStep(result, "goodbye"));
    }

    private static PipelineTestResult CreateResultWithTask(string taskName)
    {
        var taskStep = new TaskStep
        {
            TaskName = taskName,
            DisplayName = "Test Task",
            Inputs = new Dictionary<string, string>()
        };

        return new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    DisplayName = "Build",
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            DisplayName = "BuildJob",
                            Steps = new List<PipelineStep> { taskStep }
                        }
                    }
                }
            }
        };
    }

    private static PipelineTestResult CreateResultWithTaskAndInput(string taskName, string inputKey, string inputValue)
    {
        var taskStep = new TaskStep
        {
            TaskName = taskName,
            DisplayName = "Test Task",
            Inputs = new Dictionary<string, string> { { inputKey, inputValue } }
        };

        return new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    DisplayName = "Build",
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            DisplayName = "BuildJob",
                            Steps = new List<PipelineStep> { taskStep }
                        }
                    }
                }
            }
        };
    }

    private static PipelineTestResult CreateResultWithScriptStep(string script)
    {
        var scriptStep = new ScriptStep
        {
            Script = script,
            DisplayName = "Test Script"
        };

        return new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    DisplayName = "Build",
                    Jobs = new List<PipelineJob>
                    {
                        new()
                        {
                            DisplayName = "BuildJob",
                            Steps = new List<PipelineStep> { scriptStep }
                        }
                    }
                }
            }
        };
    }
}