using AdoPipelineTest.Model;

namespace AdoPipelineTest.PipelineAssertions;

public static class PipelineAgentPoolExtensions
{
    public static PipelineAgentPoolAssertions Should(this PipelineAgentPool agentPool)
    {
        return new PipelineAgentPoolAssertions(agentPool);
    }
}
