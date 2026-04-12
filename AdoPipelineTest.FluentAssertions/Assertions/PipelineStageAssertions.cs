using System.Diagnostics;
using AdoPipelineTest.Model;

namespace AdoPipelineTest.PipelineAssertions;

[DebuggerNonUserCode]
public class PipelineStageAssertions
{
    public PipelineStageAssertions(PipelineStage subject)
    {
        Subject = subject;
    }

    public PipelineStage Subject { get; }

    private static string FormatJobLabel(PipelineJob j) =>
        string.IsNullOrWhiteSpace(j.DisplayName)
            ? (j.Name ?? "(unnamed)")
            : $"{j.Name} ({j.DisplayName})";

    private static string GetAvailableJobs(PipelineStage stage) =>
        string.Join(", ", stage.Jobs.Select(FormatJobLabel));

    [CustomAssertion]
    public AndConstraint<PipelineStageAssertions> HaveJob(string jobName, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Jobs.Any(j => j.Name == jobName || j.DisplayName == jobName))
            .FailWith($"Expected {{context:the stage}} to have job '{{0}}', but found jobs: {{{1}}}",
                jobName, GetAvailableJobs(Subject));

        return new AndConstraint<PipelineStageAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<PipelineStageAssertions> HaveJobCount(int expectedCount, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Jobs.Count == expectedCount)
            .FailWith($"Expected {{context:the stage}} to have {expectedCount} job(s), but found {Subject.Jobs.Count}");

        return new AndConstraint<PipelineStageAssertions>(this);
    }
}
