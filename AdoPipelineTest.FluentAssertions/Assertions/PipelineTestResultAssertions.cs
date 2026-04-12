using System.Diagnostics;
using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.PipelineAssertions;

[DebuggerNonUserCode]
public class PipelineTestResultAssertions
{
    public PipelineTestResultAssertions(PipelineTestResult subject)
    {
        Subject = subject;
    }

    public PipelineTestResult Subject { get; }

    private static string FormatStageLabel(PipelineStage s) =>
        string.IsNullOrWhiteSpace(s.DisplayName)
            ? (s.Name ?? "(unnamed)")
            : $"{s.Name} ({s.DisplayName})";

    private static string FormatJobLabel(PipelineJob j) =>
        string.IsNullOrWhiteSpace(j.DisplayName)
            ? (j.Name ?? "(unnamed)")
            : $"{j.Name} ({j.DisplayName})";

    private static string FormatStepLabel(PipelineStep s) =>
        s.DisplayName ?? "(unnamed)";

    private static string GetAvailableStages(PipelineTestResult result) =>
        string.Join(", ", result.Stages.Select(FormatStageLabel));

    private static string GetAvailableJobs(PipelineStage stage) =>
        string.Join(", ", stage.Jobs.Select(FormatJobLabel));

    private static string GetAvailableSteps(PipelineTestResult result) =>
        string.Join(", ", result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .Select(FormatStepLabel));

    private static string GetAvailableTasks(PipelineTestResult result) =>
        string.Join(", ", result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .OfType<TaskStep>()
            .Select(t => t.TaskName));

    private static string GetAvailableVariables(PipelineTestResult result) =>
        string.Join(", ", result.Variables.Select(v => v.Name));

    private static string GetAvailableParameters(PipelineTestResult result) =>
        string.Join(", ", result.Parameters.Keys);

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveStage(string stageName, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Stages.Any(s => s.Name == stageName || s.DisplayName == stageName))
            .FailWith($"Expected {{context:the pipeline}} to have stage '{{0}}', but found stages: {{{1}}}",
                stageName, GetAvailableStages(Subject));

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveStageCount(int expectedCount, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Stages.Count == expectedCount)
            .FailWith($"Expected {{context:the pipeline}} to have {expectedCount} stage(s), but found {Subject.Stages.Count}");

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveJob(string stageName, string jobName, string because = "", params object[] becauseArgs)
    {
        var stage = Subject.Stages.FirstOrDefault(s => s.Name == stageName || s.DisplayName == stageName);

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(stage != null)
            .FailWith($"Expected {{context:the pipeline}} to have stage '{{0}}', but found stages: {{{1}}}",
                stageName, GetAvailableStages(Subject))
            .Then
            .ForCondition(stage!.Jobs.Any(j => j.Name == jobName || j.DisplayName == jobName))
            .FailWith($"Expected stage '{stageName}' to have job '{{0}}', but found jobs: {{{1}}}",
                jobName, GetAvailableJobs(stage));

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveStep(string stepDisplayName, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Stages.SelectMany(s => s.Jobs).SelectMany(j => j.Steps).Any(s => s.DisplayName == stepDisplayName))
            .FailWith($"Expected {{context:the pipeline}} to have step '{{0}}', but found steps: {{{1}}}",
                stepDisplayName, GetAvailableSteps(Subject));

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveTask(string taskName, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Stages.SelectMany(s => s.Jobs).SelectMany(j => j.Steps).OfType<TaskStep>().Any(t => t.TaskName == taskName))
            .FailWith($"Expected {{context:the pipeline}} to have task '{{0}}', but found tasks: {{{1}}}",
                taskName, GetAvailableTasks(Subject));

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveVariable(string variableName, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Variables.Any(v => v.Name == variableName))
            .FailWith($"Expected {{context:the pipeline}} to have variable '{{0}}', but found variables: {{{1}}}",
                variableName, GetAvailableVariables(Subject));

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveVariable(string variableName, string expectedValue, string because = "", params object[] becauseArgs)
    {
        var variable = Subject.Variables.FirstOrDefault(v => v.Name == variableName);

        if (variable == null)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .FailWith($"Expected {{context:the pipeline}} to have variable '{{0}}', but found variables: {{{1}}}",
                    variableName, GetAvailableVariables(Subject));
        }
        else if (variable.DefaultValue?.ToString() != expectedValue)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .FailWith($"Expected variable '{variableName}' to have value '{{0}}', but found '{{1}}'",
                    expectedValue, variable.DefaultValue?.ToString());
        }

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveParameter(string parameterName, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Parameters.ContainsKey(parameterName))
            .FailWith($"Expected {{context:the pipeline}} to have parameter '{{0}}', but found parameters: {{{1}}}",
                parameterName, GetAvailableParameters(Subject));

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveTrigger(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Triggers != null)
            .FailWith("Expected {{context:the pipeline}} to have triggers configured, but none were found");

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineTriggersAssertions> HaveTriggers(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Triggers != null)
            .FailWith("Expected {{context:the pipeline}} to have triggers configured, but none were found");

        return new AndConstraint<PipelineTriggersAssertions>(new PipelineTriggersAssertions(Subject.Triggers!));
    }

    [CustomAssertion]
    public AndConstraint<PipelineAgentPoolAssertions> HaveAgentPool(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.AgentPool != null)
            .FailWith("Expected {{context:the pipeline}} to have an agent pool configured, but none was found");

        return new AndConstraint<PipelineAgentPoolAssertions>(new PipelineAgentPoolAssertions(Subject.AgentPool!));
    }

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveVmImage(string vmImage, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.AgentPool != null)
            .FailWith("Expected {{context:the pipeline}} to have an agent pool configured, but none was found")
            .Then
            .ForCondition(Subject.AgentPool!.VmImage == vmImage)
            .FailWith($"Expected VM image to be '{{0}}', but found '{{1}}'", vmImage, Subject.AgentPool.VmImage ?? "(null)");

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineTestResultAssertions> HaveScriptStepContaining(string scriptPattern, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Stages.SelectMany(s => s.Jobs).SelectMany(j => j.Steps).OfType<ScriptStep>().Any(s => s.Script.Contains(scriptPattern)))
            .FailWith($"Expected {{context:the pipeline}} to have a script step containing '{{0}}', but none was found", scriptPattern);

        return new AndConstraint<PipelineTestResultAssertions>(this);
    }
}
