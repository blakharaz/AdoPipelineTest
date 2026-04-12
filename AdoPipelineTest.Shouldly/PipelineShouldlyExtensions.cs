using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using Shouldly;

namespace AdoPipelineTest.Shouldly;

[DebuggerStepThrough]
[ShouldlyMethods]
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class PipelineShouldlyExtensions
{
    private static string FormatStageLabel(PipelineStage s) =>
        string.IsNullOrWhiteSpace(s.DisplayName)
            ? (s.Name ?? "(unnamed)")
            : $"{s.Name} ({s.DisplayName})";

    private static string FormatJobLabel(PipelineJob j) =>
        string.IsNullOrWhiteSpace(j.DisplayName)
            ? (j.Name ?? "(unnamed)")
            : $"{j.Name} ({j.DisplayName})";

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveStage(this PipelineTestResult result, string stageName, string? customMessage = null)
    {
        var stage = result.Stages.FirstOrDefault(s => s.Name == stageName || s.DisplayName == stageName);
        if (stage == null)
        {
            var message = customMessage ?? $"Stage '{stageName}' not found. Available stages: {string.Join(", ", result.Stages.Select(FormatStageLabel))}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveStageCount(this PipelineTestResult result, int count, string? customMessage = null)
    {
        if (result.Stages.Count != count)
        {
            var message = customMessage ?? $"Expected {count} stages, found {result.Stages.Count}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveJob(this PipelineTestResult result, string stageName, string jobName, string? customMessage = null)
    {
        var stage = result.Stages.FirstOrDefault(s => s.Name == stageName || s.DisplayName == stageName);
        if (stage == null)
        {
            var message = customMessage ?? $"Stage '{stageName}' not found";
            throw new ShouldAssertException(message);
        }

        var job = stage.Jobs.FirstOrDefault(j => j.Name == jobName || j.DisplayName == jobName);
        if (job == null)
        {
            var message = customMessage ?? $"Job '{jobName}' not found in stage '{stageName}'. Available jobs: {string.Join(", ", stage.Jobs.Select(FormatJobLabel))}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveJob(this PipelineStage stage, string jobName, string? customMessage = null)
    {
        var job = stage.Jobs.FirstOrDefault(j => j.Name == jobName || j.DisplayName == jobName);
        if (job == null)
        {
            var message = customMessage ?? $"Job '{jobName}' not found. Available jobs: {string.Join(", ", stage.Jobs.Select(FormatJobLabel))}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveJobCount(this PipelineStage stage, int count, string? customMessage = null)
    {
        if (stage.Jobs.Count != count)
        {
            var message = customMessage ?? $"Expected {count} jobs, found {stage.Jobs.Count}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveStep(this PipelineTestResult result, string stepDisplayName, string? customMessage = null)
    {
        var allSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .ToList();

        var step = allSteps.FirstOrDefault(s => s.DisplayName == stepDisplayName);
        if (step == null)
        {
            var message = customMessage ?? $"Step '{stepDisplayName}' not found. Available steps: {string.Join(", ", allSteps.Select(s => s.DisplayName ?? "(unnamed)"))}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveStep(this PipelineTestResult result, Func<PipelineStep, bool> predicate, string? customMessage = null)
    {
        var allSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .ToList();

        var step = allSteps.FirstOrDefault(predicate);
        if (step == null)
        {
            var message = customMessage ?? "No step matched the predicate";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveStepCount(this PipelineJob job, int count, string? customMessage = null)
    {
        if (job.Steps.Count != count)
        {
            var message = customMessage ?? $"Expected {count} steps, found {job.Steps.Count}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveTask(this PipelineTestResult result, string taskName, string? customMessage = null)
    {
        var allTasks = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<TaskStep>()
            .ToList();

        var task = allTasks.FirstOrDefault(t => t.TaskName == taskName);
        if (task == null)
        {
            var message = customMessage ?? $"Task '{taskName}' not found. Available tasks: {string.Join(", ", allTasks.Select(t => t.TaskName))}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveVariable(this PipelineTestResult result, string variableName, string? customMessage = null)
    {
        var variable = result.Variables.FirstOrDefault(v => v.Name == variableName);
        if (variable == null)
        {
            var message = customMessage ?? $"Variable '{variableName}' not found. Available variables: {string.Join(", ", result.Variables.Select(v => v.Name))}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveVariableValue(this PipelineTestResult result, string variableName, string expectedValue, string? customMessage = null)
    {
        var variable = result.Variables.FirstOrDefault(v => v.Name == variableName);
        if (variable == null)
        {
            var message = customMessage ?? $"Variable '{variableName}' not found. Available variables: {string.Join(", ", result.Variables.Select(v => v.Name))}";
            throw new ShouldAssertException(message);
        }

        if (variable.DefaultValue?.ToString() != expectedValue)
        {
            var message = customMessage ?? $"Variable '{variableName}' should have value '{expectedValue}' but was '{variable.DefaultValue}'";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveParameter(this PipelineTestResult result, string parameterName, string? customMessage = null)
    {
        if (!result.Parameters.ContainsKey(parameterName))
        {
            var message = customMessage ?? $"Parameter '{parameterName}' not found. Available parameters: {string.Join(", ", result.Parameters.Keys)}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveTrigger(this PipelineTestResult result, string? customMessage = null)
    {
        if (result.Triggers == null)
        {
            var message = customMessage ?? "Pipeline has no triggers configured";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldIncludeBranch(this PipelineTestResult result, string branchName, string? customMessage = null)
    {
        if (result.Triggers == null)
        {
            var message = customMessage ?? "Pipeline has no triggers configured";
            throw new ShouldAssertException(message);
        }

        if (!result.Triggers.IncludedBranches.Contains(branchName))
        {
            var message = customMessage ?? $"Branch '{branchName}' not in trigger branches. Available branches: {string.Join(", ", result.Triggers.IncludedBranches)}";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveVmImage(this PipelineTestResult result, string vmImage, string? customMessage = null)
    {
        if (result.AgentPool == null)
        {
            var message = customMessage ?? "Pipeline has no pool configured";
            throw new ShouldAssertException(message);
        }

        if (result.AgentPool.VmImage != vmImage)
        {
            var message = customMessage ?? $"Expected VM image '{vmImage}', found '{result.AgentPool.VmImage}'";
            throw new ShouldAssertException(message);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveScriptStepContaining(this PipelineTestResult result, string scriptPattern, string? customMessage = null)
    {
        var scriptSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<ScriptStep>()
            .ToList();

        var matchingStep = scriptSteps.FirstOrDefault(s => s.Script.Contains(scriptPattern));
        if (matchingStep == null)
        {
            var message = customMessage ?? $"No script step containing '{scriptPattern}' found";
            throw new ShouldAssertException(message);
        }
    }
}
