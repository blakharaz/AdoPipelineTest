using AdoPipelineTest.Model;

namespace AdoPipelineTest.Parsing.RawModel;

internal class PipelineParseResult
{
    public PipelineTriggers? Triggers { get; init; }

    public PipelineAgentPool? AgentPool { get; init; }
    
    public IList<RawPipelineStage> Stages { get; init; } = [];
    
}