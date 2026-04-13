using AdoPipelineTest.Model;
using FluentAssertions.Execution;

namespace AdoPipelineTest.PipelineAssertions;

public static class PipelineAgentPoolExtensions
{
    public static PipelineAgentPoolAssertions Should(this PipelineAgentPool agentPool)
    {
        return new PipelineAgentPoolAssertions(agentPool, AssertionChain.GetOrCreate());
    }
}
