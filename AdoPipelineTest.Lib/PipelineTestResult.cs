using AdoPipelineTest.Model;

namespace AdoPipelineTest;

public class PipelineTestResult
{
    public PipelineTriggers? Triggers { get; init; }

    public PipelineAgentPool? AgentPool { get; init; }
    
    public IList<PipelineVariable> Variables { get; init; } = [];
    
    public IList<PipelineStage> Stages { get; init; } = [];
}