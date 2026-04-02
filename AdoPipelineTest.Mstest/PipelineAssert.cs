using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdoPipelineTest.Mstest;

public static class PipelineAssert
{
    public static void HasStage(this PipelineTestResult result, string stageName, string? because = null)
    {
        var stage = result.Stages.FirstOrDefault(s => s.Name == stageName || s.DisplayName == stageName);
        Assert.IsNotNull(stage, because ?? $"Stage '{stageName}' not found. Available stages: {string.Join(", ", result.Stages.Select(s => FormatStageLabel(s)))}");
    }

    public static void HasStageCount(this PipelineTestResult result, int count, string? because = null)
    {
        Assert.AreEqual(count, result.Stages.Count, because ?? $"Expected {count} stages, found {result.Stages.Count}");
    }

    public static void HasJob(this PipelineTestResult result, string stageName, string jobName, string? because = null)
    {
        var stage = result.Stages.FirstOrDefault(s => s.Name == stageName || s.DisplayName == stageName);
        Assert.IsNotNull(stage, $"Stage '{stageName}' not found");
        
        var job = stage.Jobs.FirstOrDefault(j => j.Name == jobName || j.DisplayName == jobName);
        Assert.IsNotNull(job, because ?? $"Job '{jobName}' not found in stage '{stageName}'. Available jobs: {string.Join(", ", stage.Jobs.Select(j => FormatJobLabel(j)))}");
    }

    public static void HasJob(this PipelineStage stage, string jobName, string? because = null)
    {
        var job = stage.Jobs.FirstOrDefault(j => j.Name == jobName || j.DisplayName == jobName);
        Assert.IsNotNull(job, because ?? $"Job '{jobName}' not found. Available jobs: {string.Join(", ", stage.Jobs.Select(j => FormatJobLabel(j)))}");
    }

    private static string FormatStageLabel(PipelineStage s) =>
        string.IsNullOrWhiteSpace(s.DisplayName)
            ? (s.Name ?? "(unnamed)")
            : $"{s.Name} ({s.DisplayName})";

    private static string FormatJobLabel(PipelineJob j) =>
        string.IsNullOrWhiteSpace(j.DisplayName)
            ? (j.Name ?? "(unnamed)")
            : $"{j.Name} ({j.DisplayName})";

    public static void HasStep(this PipelineTestResult result, string stepDisplayName, string? because = null)
    {
        var allSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .ToList();
        
        var step = allSteps.FirstOrDefault(s => s.DisplayName == stepDisplayName);
        Assert.IsNotNull(step, because ?? $"Step '{stepDisplayName}' not found. Available steps: {string.Join(", ", allSteps.Select(s => s.DisplayName ?? "(unnamed)"))}");
    }

    public static void HasStep(this PipelineTestResult result, Func<PipelineStep, bool> predicate, string? because = null)
    {
        var allSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .ToList();
        
        var step = allSteps.FirstOrDefault(predicate);
        Assert.IsNotNull(step, because ?? "No step matched the predicate");
    }

    public static void HasTask(this PipelineTestResult result, string taskName, string? because = null)
    {
        var allTasks = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<TaskStep>()
            .ToList();
        
        var task = allTasks.FirstOrDefault(t => t.TaskName == taskName);
        Assert.IsNotNull(task, because ?? $"Task '{taskName}' not found. Available tasks: {string.Join(", ", allTasks.Select(t => t.TaskName))}");
    }

    public static void HasVariable(this PipelineTestResult result, string variableName)
    {
        var variable = result.Variables.FirstOrDefault(v => v.Name == variableName);
        Assert.IsNotNull(variable, $"Variable '{variableName}' not found. Available variables: {string.Join(", ", result.Variables.Select(v => v.Name))}");
    }

    public static void HasVariable(this PipelineTestResult result, string variableName, string expectedValue, string? because = null)
    {
        var variable = result.Variables.FirstOrDefault(v => v.Name == variableName);
        Assert.IsNotNull(variable, because ?? $"Variable '{variableName}' not found");
        Assert.AreEqual(expectedValue, variable?.DefaultValue?.ToString(), because ?? $"Variable '{variableName}' has wrong value");
    }

    public static void HasParameter(this PipelineTestResult result, string parameterName, string? because = null)
    {
        Assert.IsTrue(result.Parameters.ContainsKey(parameterName), because ?? $"Parameter '{parameterName}' not found. Available parameters: {string.Join(", ", result.Parameters.Keys)}");
    }

    public static void HasTrigger(this PipelineTestResult result, string? because = null)
    {
        Assert.IsNotNull(result.Triggers, because ?? "Pipeline has no triggers configured");
    }

    public static void TriggersIncludeBranch(this PipelineTestResult result, string branchName, string? because = null)
    {
        Assert.IsNotNull(result.Triggers, because ?? "Pipeline has no triggers configured");
        CollectionAssert.Contains(result.Triggers.IncludedBranches.ToList(), branchName, because ?? $"Branch '{branchName}' not in trigger branches");
    }

    public static void HasVmImage(this PipelineTestResult result, string vmImage, string? because = null)
    {
        Assert.IsNotNull(result.AgentPool, because ?? "Pipeline has no pool configured");
        Assert.AreEqual(vmImage, result.AgentPool.VmImage, because ?? $"Expected VM image '{vmImage}', found '{result.AgentPool.VmImage}'");
    }

    public static void HasScriptStep(this PipelineTestResult result, string scriptPattern, string? because = null)
    {
        var scriptSteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<ScriptStep>()
            .ToList();
        
        var matchingStep = scriptSteps.FirstOrDefault(s => s.Script.Contains(scriptPattern));
        Assert.IsNotNull(matchingStep, because ?? $"No script step containing '{scriptPattern}' found");
    }
}
