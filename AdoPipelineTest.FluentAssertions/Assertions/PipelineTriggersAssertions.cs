using System.Diagnostics;
using AdoPipelineTest.Model;

namespace AdoPipelineTest.PipelineAssertions;

[DebuggerNonUserCode]
public class PipelineTriggersAssertions
{
    public PipelineTriggersAssertions(PipelineTriggers subject)
    {
        Subject = subject;
    }

    public PipelineTriggers Subject { get; }

    [CustomAssertion]
    public AndConstraint<PipelineTriggersAssertions> IncludeBranch(string branchName, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.IncludedBranches.Contains(branchName))
            .FailWith($"Expected {{context:the triggers}} to include branch '{{0}}', but found branches: {{{1}}}",
                branchName, string.Join(", ", Subject.IncludedBranches));

        return new AndConstraint<PipelineTriggersAssertions>(this);
    }
}
