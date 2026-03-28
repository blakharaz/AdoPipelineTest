using AdoPipelineTest.Model;

namespace AdoPipelineTest.Parsing.Ast;

internal class PipelineSyntaxTree
{
    public PipelineTriggers? Triggers { get; init; }

    public PipelineAgentPool? AgentPool { get; init; }
    
    public IList<PipelineVariableElement> Variables { get; init; } = [];
    
    public IList<PipelineStageElement> Stages { get; init; } = [];

    public IList<PipelineParameterElement> Parameters { get; init; } = [];

    public IList<PipelineResourceElement> Resources { get; init; } = [];
}