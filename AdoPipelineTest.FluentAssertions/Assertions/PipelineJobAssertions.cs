using System.Diagnostics;
using AdoPipelineTest.Model;
using FluentAssertions;
using FluentAssertions.Execution;

namespace AdoPipelineTest.PipelineAssertions;

[DebuggerNonUserCode]
public class PipelineJobAssertions
{
    public PipelineJobAssertions(PipelineJob subject, AssertionChain assertionChain)
    {
        Subject = subject;
        AssertionChain = assertionChain;
    }

    public PipelineJob Subject { get; }

    private readonly AssertionChain AssertionChain;

    [CustomAssertion]
    public AndConstraint<PipelineJobAssertions> HaveStepCount(int expectedCount, string because = "", params object[] becauseArgs)
    {
        AssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Steps.Count == expectedCount)
            .FailWith($"Expected {{context:the job}} to have {expectedCount} step(s), but found {Subject.Steps.Count}");

        return new AndConstraint<PipelineJobAssertions>(this);
    }
}
