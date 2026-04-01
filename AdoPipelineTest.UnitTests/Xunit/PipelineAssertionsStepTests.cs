using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Xunit;
using NUnitAssert = NUnit.Framework.Assert;
using PipelineAssert = AdoPipelineTest.Xunit.Assert;

namespace AdoPipelineTest.UnitTests.XunitHelpers;

public class PipelineAssertionsStepTests
{
    [Test]
    public void HasTask_WhenTaskExists_DoesNotThrow()
    {
        var result = CreateResultWithTask("UseDotNet@2");

        NUnitAssert.DoesNotThrow(() => PipelineAssert.HasTask(result, "UseDotNet@2"));
    }

    [Test]
    public void HasTask_WhenTaskDoesNotExist_Throws()
    {
        var result = CreateResultWithTask("UseDotNet@2");

        NUnitAssert.That(() => PipelineAssert.HasTask(result, "NonExistent@1"), Throws.Exception);
    }

    [Test]
    public void TaskHasInput_WhenInputExists_DoesNotThrow()
    {
        var result = CreateResultWithTaskAndInput("UseDotNet@2", "version", "8.0.x");

        NUnitAssert.DoesNotThrow(() => PipelineAssert.TaskHasInput(result, "UseDotNet@2", "version"));
    }

    [Test]
    public void TaskHasInput_WhenInputExistsAndValueMatches_DoesNotThrow()
    {
        var result = CreateResultWithTaskAndInput("UseDotNet@2", "version", "8.0.x");

        NUnitAssert.DoesNotThrow(() => PipelineAssert.TaskHasInput(result, "UseDotNet@2", "version", "8.0.x"));
    }

    [Test]
    public void TaskHasInput_WhenInputExistsButValueDiffers_Throws()
    {
        var result = CreateResultWithTaskAndInput("UseDotNet@2", "version", "8.0.x");

        NUnitAssert.That(() => PipelineAssert.TaskHasInput(result, "UseDotNet@2", "version", "9.0.x"), Throws.Exception);
    }

    [Test]
    public void TaskHasInput_WhenInputDoesNotExist_Throws()
    {
        var result = CreateResultWithTaskAndInput("UseDotNet@2", "version", "8.0.x");

        NUnitAssert.That(() => PipelineAssert.TaskHasInput(result, "UseDotNet@2", "nonexistent"), Throws.Exception);
    }

    [Test]
    public void HasScriptStep_WhenScriptContainsContent_DoesNotThrow()
    {
        var result = CreateResultWithScriptStep("echo hello world");

        NUnitAssert.DoesNotThrow(() => PipelineAssert.HasScriptStep(result, "hello"));
    }

    [Test]
    public void HasScriptStep_WhenScriptDoesNotContainContent_Throws()
    {
        var result = CreateResultWithScriptStep("echo hello world");

        NUnitAssert.That(() => PipelineAssert.HasScriptStep(result, "goodbye"), Throws.Exception);
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
