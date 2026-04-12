using System.Diagnostics;
using AdoPipelineTest.Model;

namespace AdoPipelineTest.PipelineAssertions;

[DebuggerNonUserCode]
public class PipelineAgentPoolAssertions
{
    public PipelineAgentPoolAssertions(PipelineAgentPool subject)
    {
        Subject = subject;
    }

    public PipelineAgentPool Subject { get; }

    [CustomAssertion]
    public AndConstraint<PipelineAgentPoolAssertions> HaveVmImage(string vmImage, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.VmImage == vmImage)
            .FailWith($"Expected {{context:the VM image}} to be '{{0}}', but found '{{1}}'", vmImage, Subject.VmImage ?? "(null)");

        return new AndConstraint<PipelineAgentPoolAssertions>(this);
    }
}
