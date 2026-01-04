namespace AdoPipelineTest.Model.Steps;

public class ScriptStep : PipelineStep
{
    public required string Script { get; init; }
    public Dictionary<string, object> Variables { get; init; } = [];
}