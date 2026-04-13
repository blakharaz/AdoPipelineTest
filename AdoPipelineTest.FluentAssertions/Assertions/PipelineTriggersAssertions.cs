using System.Diagnostics;
using AdoPipelineTest.Model;
using FluentAssertions;
using FluentAssertions.Execution;

namespace AdoPipelineTest.PipelineAssertions;

[DebuggerNonUserCode]
public class PipelineTriggersAssertions
{
    public PipelineTriggersAssertions(PipelineTriggers subject, AssertionChain assertionChain)
    {
        Subject = subject;
        AssertionChain = assertionChain;
    }

    public PipelineTriggers Subject { get; }

    private readonly AssertionChain AssertionChain;

    [CustomAssertion]
    public AndConstraint<PipelineTriggersAssertions> IncludeBranch(string branchName, string because = "", params object[] becauseArgs)
    {
        AssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.IncludedBranches.Contains(branchName))
            .FailWith($"Expected {{context:the triggers}} to include branch '{{0}}', but found branches: {{{1}}}",
                branchName, string.Join(", ", Subject.IncludedBranches));

        return new AndConstraint<PipelineTriggersAssertions>(this);
    }
}
