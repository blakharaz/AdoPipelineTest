using AdoPipelineTest;
using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using Xunit.Sdk;

namespace AdoPipelineTest.Xunit;

/// <summary>
/// Extended assertions for Azure DevOps YAML pipeline testing.
/// Inherit from Xunit.Assert to provide custom assertions alongside built-in ones.
/// Usage: using Assert = AdoPipelineTest.Xunit.Assert;
/// </summary>
public class Assert : global::Xunit.Assert
{
    public static void HasStage(PipelineTestResult result, string stageDisplayName, string? because = null)
    {
        var stage = result.Stages.FirstOrDefault(s => s.DisplayName == stageDisplayName);
        var message = because ??
                      $"Stage '{stageDisplayName}' not found. Available stages: {string.Join(", ", result.Stages.Select(s => s.DisplayName ?? "(unnamed)"))}";
        if (stage == null)
        {
            throw new XunitException(message);
        }
    }

    public static void StageCount(PipelineTestResult result, int count, string? because = null)
    {
        if (result.Stages.Count != count)
        {
            throw new XunitException(because ?? $"Expected {count} stages, but found {result.Stages.Count}");
        }
    }

    public static void HasJob(PipelineTestResult result, string stageDisplayName, string jobDisplayName,
        string? because = null)
    {
        var stage = result.Stages.FirstOrDefault(s => s.DisplayName == stageDisplayName);
        if (stage == null)
        {
            throw new XunitException($"Stage '{stageDisplayName}' not found");
        }

        var job = stage!.Jobs.FirstOrDefault(j => j.DisplayName == jobDisplayName);
        var message = because ??
                      $"Job '{jobDisplayName}' not found in stage '{stageDisplayName}'. Available jobs: {string.Join(", ", stage.Jobs.Select(j => j.DisplayName ?? "(unnamed)"))}";
        if (job == null)
        {
            throw new XunitException(message);
        }
    }

    public static void HasJob(PipelineStage stage, string jobDisplayName, string? because = null)
    {
        var job = stage.Jobs.FirstOrDefault(j => j.DisplayName == jobDisplayName);
        var message = because ??
                      $"Job '{jobDisplayName}' not found. Available jobs: {string.Join(", ", stage.Jobs.Select(j => j.DisplayName ?? "(unnamed)"))}";
        if (job == null)
        {
            throw new XunitException(message);
        }
    }

    public static void HasStep(PipelineTestResult result, string stepDisplayName, string? because = null)
    {
        var allSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .ToList();

        var step = allSteps.FirstOrDefault(s => s.DisplayName == stepDisplayName);
        var message = because ??
                      $"Step '{stepDisplayName}' not found. Available steps: {string.Join(", ", allSteps.Select(s => s.DisplayName ?? "(unnamed)"))}";
        if (step == null)
        {
            throw new XunitException(message);
        }
    }

    public static void HasStep(PipelineTestResult result, Func<PipelineStep, bool> predicate,
        string? because = null)
    {
        var allSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .ToList();

        var step = allSteps.FirstOrDefault(predicate);
        if (step == null)
        {
            throw new XunitException(because ?? "No step matched the predicate");
        }
    }

    public static void HasTask(PipelineTestResult result, string taskName, string? because = null)
    {
        var allTasks = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<TaskStep>()
            .ToList();

        var task = allTasks.FirstOrDefault(t => t.TaskName == taskName);
        var message = because ??
                      $"Task '{taskName}' not found. Available tasks: {string.Join(", ", allTasks.Select(t => t.TaskName))}";
        if (task == null)
        {
            throw new XunitException(message);
        }
    }

    public static void HasVariable(PipelineTestResult result, string variableName, string? because = null)
    {
        var variable = result.Variables.FirstOrDefault(v => v.Name == variableName);
        var message = because ??
                      $"Variable '{variableName}' not found. Available variables: {string.Join(", ", result.Variables.Select(v => v.Name))}";
        if (variable == null)
        {
            throw new XunitException(message);
        }
    }

    public static void HasVariable(PipelineTestResult result, string variableName, string expectedValue,
        string? because = null)
    {
        var variable = result.Variables.FirstOrDefault(v => v.Name == variableName);
        if (variable == null)
        {
            throw new XunitException(because ?? $"Variable '{variableName}' not found");
        }
        if (expectedValue != variable!.DefaultValue?.ToString())
        {
            throw new XunitException(because ?? $"Variable '{variableName}' has value '{variable.DefaultValue}', expected '{expectedValue}'");
        }
    }

    public static void HasParameter(PipelineTestResult result, string parameterName, string? because = null)
    {
        if (!result.Parameters.ContainsKey(parameterName))
        {
            throw new XunitException(because ?? $"Parameter '{parameterName}' not found");
        }
    }

    public static void ParameterHasValue(PipelineTestResult result, string parameterName, object expectedValue,
        string? because = null)
    {
        if (!result.Parameters.TryGetValue(parameterName, out var parameter))
        {
            throw new XunitException(because ?? $"Parameter '{parameterName}' not found");
        }

        if (!object.Equals(expectedValue, parameter.Value))
        {
            throw new XunitException(because ??
                                     $"Parameter '{parameterName}' has value '{parameter.Value}', expected '{expectedValue}'");
        }
    }

    public static void HasTrigger(PipelineTestResult result, string? because = null)
    {
        if (result.Triggers == null)
        {
            throw new XunitException(because ?? "Triggers not defined");
        }
    }

    public static void TriggersIncludeBranch(PipelineTestResult result, string branchName, string? because = null)
    {
        if (result.Triggers == null)
        {
            throw new XunitException(because ?? "Triggers not defined");
        }
        if (!result.Triggers!.IncludedBranches.Contains(branchName))
        {
            throw new XunitException(because ?? $"Branch '{branchName}' not found in included branches");
        }
    }

    public static void HasVmImage(PipelineTestResult result, string vmImage, string? because = null)
    {
        if (result.AgentPool == null)
        {
            throw new XunitException(because ?? "Agent pool not defined");
        }
        if (vmImage != result.AgentPool!.VmImage)
        {
            throw new XunitException(because ?? $"VmImage is '{result.AgentPool.VmImage}', expected '{vmImage}'");
        }
    }

    public static void HasScriptStep(PipelineTestResult result, string scriptContent, string? because = null)
    {
        var scriptSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<ScriptStep>()
            .ToList();

        var matchingStep = scriptSteps.FirstOrDefault(s => s.Script.Contains(scriptContent));
        if (matchingStep == null)
        {
            throw new XunitException(because ?? $"No script step containing '{scriptContent}' found");
        }
    }

    public static void JobCount(PipelineStage stage, int expectedCount, string? because = null)
    {
        if (stage.Jobs.Count != expectedCount)
        {
            throw new XunitException(because ?? $"Expected {expectedCount} jobs, but found {stage.Jobs.Count}");
        }
    }

    public static void StepCount(PipelineJob job, int expectedCount, string? because = null)
    {
        if (job.Steps.Count != expectedCount)
        {
            throw new XunitException(because ?? $"Expected {expectedCount} steps, but found {job.Steps.Count}");
        }
    }

    public static void TaskHasInput(PipelineTestResult result, string taskName, string inputKey,
        string? expectedValue = null, string? because = null)
    {
        var taskSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<TaskStep>()
            .ToList();

        var task = taskSteps.FirstOrDefault(t => t.TaskName == taskName);
        if (task == null)
        {
            throw new XunitException(because ?? $"Task '{taskName}' not found");
        }
        if (!task!.Inputs.ContainsKey(inputKey))
        {
            throw new XunitException(because ?? $"Task '{taskName}' does not have input '{inputKey}'");
        }
        if (expectedValue != null && expectedValue != task.Inputs[inputKey])
        {
            throw new XunitException(because ?? $"Task '{taskName}' input '{inputKey}' has value '{task.Inputs[inputKey]}', expected '{expectedValue}'");
        }
    }
}
