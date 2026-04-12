using System.Diagnostics;
using AdoPipelineTest.Model;

namespace AdoPipelineTest.PipelineAssertions;

[DebuggerNonUserCode]
public class PipelineJobAssertions
{
    public PipelineJobAssertions(PipelineJob subject)
    {
        Subject = subject;
    }

    public PipelineJob Subject { get; }

    [CustomAssertion]
    public AndConstraint<PipelineJobAssertions> HaveStepCount(int expectedCount, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Steps.Count == expectedCount)
            .FailWith($"Expected {{context:the job}} to have {expectedCount} step(s), but found {Subject.Steps.Count}");

        return new AndConstraint<PipelineJobAssertions>(this);
    }
}
