using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using XunitAssert = Xunit.Assert;

namespace AdoPipelineTest.Xunit;

public static class Assert
{
    public static void HasStage(PipelineTestResult result, string stageDisplayName, string? because = null)
    {
        var stage = result.Stages.FirstOrDefault(s => s.DisplayName == stageDisplayName);
        var message = because ?? $"Stage '{stageDisplayName}' not found. Available stages: {string.Join(", ", result.Stages.Select(s => s.DisplayName ?? "(unnamed)"))}";
        XunitAssert.True(stage != null, message);
    }

    public static void StageCount(PipelineTestResult result, int count, string? because = null)
    {
        XunitAssert.Equal(count, result.Stages.Count);
    }

    public static void HasJob(PipelineTestResult result, string stageDisplayName, string jobDisplayName, string? because = null)
    {
        var stage = result.Stages.FirstOrDefault(s => s.DisplayName == stageDisplayName);
        XunitAssert.True(stage != null, $"Stage '{stageDisplayName}' not found");

        var job = stage!.Jobs.FirstOrDefault(j => j.DisplayName == jobDisplayName);
        var message = because ?? $"Job '{jobDisplayName}' not found in stage '{stageDisplayName}'. Available jobs: {string.Join(", ", stage.Jobs.Select(j => j.DisplayName ?? "(unnamed)"))}";
        XunitAssert.True(job != null, message);
    }

    public static void HasJob(PipelineStage stage, string jobDisplayName, string? because = null)
    {
        var job = stage.Jobs.FirstOrDefault(j => j.DisplayName == jobDisplayName);
        var message = because ?? $"Job '{jobDisplayName}' not found. Available jobs: {string.Join(", ", stage.Jobs.Select(j => j.DisplayName ?? "(unnamed)"))}";
        XunitAssert.True(job != null, message);
    }

    public static void HasStep(PipelineTestResult result, string stepDisplayName, string? because = null)
    {
        var allSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .ToList();
        
        var step = allSteps.FirstOrDefault(s => s.DisplayName == stepDisplayName);
        var message = because ?? $"Step '{stepDisplayName}' not found. Available steps: {string.Join(", ", allSteps.Select(s => s.DisplayName ?? "(unnamed)"))}";
        XunitAssert.True(step != null, message);
    }

    public static void HasStep(PipelineTestResult result, Func<PipelineStep, bool> predicate, string? because = null)
    {
        var allSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .ToList();
        
        var step = allSteps.FirstOrDefault(predicate);
        XunitAssert.True(step != null, because ?? "No step matched the predicate");
    }

    public static void HasTask(PipelineTestResult result, string taskName, string? because = null)
    {
        var allTasks = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<TaskStep>()
            .ToList();
        
        var task = allTasks.FirstOrDefault(t => t.TaskName == taskName);
        var message = because ?? $"Task '{taskName}' not found. Available tasks: {string.Join(", ", allTasks.Select(t => t.TaskName))}";
        XunitAssert.True(task != null, message);
    }

    public static void HasVariable(PipelineTestResult result, string variableName, string? because = null)
    {
        var variable = result.Variables.FirstOrDefault(v => v.Name == variableName);
        var message = because ?? $"Variable '{variableName}' not found. Available variables: {string.Join(", ", result.Variables.Select(v => v.Name))}";
        XunitAssert.True(variable != null, message);
    }

    public static void HasVariable(PipelineTestResult result, string variableName, string expectedValue, string? because = null)
    {
        var variable = result.Variables.FirstOrDefault(v => v.Name == variableName);
        XunitAssert.True(variable != null, because ?? $"Variable '{variableName}' not found");
        XunitAssert.Equal(expectedValue, variable!.DefaultValue?.ToString());
    }

    public static void HasParameter(PipelineTestResult result, string parameterName, string? because = null)
    {
        XunitAssert.True(result.Parameters.ContainsKey(parameterName), because ?? $"Parameter '{parameterName}' not found");
    }

    public static void ParameterHasValue(PipelineTestResult result, string parameterName, object expectedValue, string? because = null)
    {
        XunitAssert.True(result.Parameters.ContainsKey(parameterName), because ?? $"Parameter '{parameterName}' not found");
        var parameter = result.Parameters[parameterName];
        XunitAssert.Equal(expectedValue, parameter.Value);
    }

    public static void HasTrigger(PipelineTestResult result, string? because = null)
    {
        XunitAssert.True(result.Triggers != null, because ?? "Triggers not defined");
    }

    public static void TriggersIncludeBranch(PipelineTestResult result, string branchName, string? because = null)
    {
        XunitAssert.True(result.Triggers != null, because ?? "Triggers not defined");
        XunitAssert.Contains(branchName, result.Triggers!.IncludedBranches);
    }

    public static void HasVmImage(PipelineTestResult result, string vmImage, string? because = null)
    {
        XunitAssert.True(result.AgentPool != null, because ?? "Agent pool not defined");
        XunitAssert.Equal(vmImage, result.AgentPool!.VmImage);
    }

    public static void HasScriptStep(PipelineTestResult result, string scriptContent, string? because = null)
    {
        var scriptSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<ScriptStep>()
            .ToList();
        
        var matchingStep = scriptSteps.FirstOrDefault(s => s.Script.Contains(scriptContent));
        XunitAssert.True(matchingStep != null, because ?? $"No script step containing '{scriptContent}' found");
    }

    public static void JobCount(PipelineStage stage, int expectedCount, string? because = null)
    {
        XunitAssert.Equal(expectedCount, stage.Jobs.Count);
    }

    public static void StepCount(PipelineJob job, int expectedCount, string? because = null)
    {
        XunitAssert.Equal(expectedCount, job.Steps.Count);
    }

    public static void TaskHasInput(PipelineTestResult result, string taskName, string inputKey, string? expectedValue = null, string? because = null)
    {
        var taskSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<TaskStep>()
            .ToList();
        
        var task = taskSteps.FirstOrDefault(t => t.TaskName == taskName);
        XunitAssert.True(task != null, because ?? $"Task '{taskName}' not found");
        XunitAssert.True(task!.Inputs.ContainsKey(inputKey), because ?? $"Task '{taskName}' does not have input '{inputKey}'");
        if (expectedValue != null)
        {
            XunitAssert.Equal(expectedValue, task.Inputs[inputKey]);
        }
    }
}
