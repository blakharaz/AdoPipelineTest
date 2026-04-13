using System.Diagnostics;
using AdoPipelineTest.Model;
using FluentAssertions;
using FluentAssertions.Execution;

namespace AdoPipelineTest.PipelineAssertions;

[DebuggerNonUserCode]
public class PipelineAgentPoolAssertions
{
    public PipelineAgentPoolAssertions(PipelineAgentPool subject, AssertionChain assertionChain)
    {
        Subject = subject;
        AssertionChain = assertionChain;
    }

    public PipelineAgentPool Subject { get; }

    private readonly AssertionChain AssertionChain;

    [CustomAssertion]
    public AndConstraint<PipelineAgentPoolAssertions> HaveVmImage(string vmImage, string because = "", params object[] becauseArgs)
    {
        AssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.VmImage == vmImage)
            .FailWith($"Expected {{context:the VM image}} to be '{{0}}', but found '{{1}}'", vmImage, Subject.VmImage ?? "(null)");

        return new AndConstraint<PipelineAgentPoolAssertions>(this);
    }
}
