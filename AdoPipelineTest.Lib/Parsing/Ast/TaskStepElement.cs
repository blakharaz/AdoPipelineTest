namespace AdoPipelineTest.Parsing.Ast;

public class TaskStepElement : PipelineStepElement
{
    public required string TaskName { get; init; }
    public IDictionary<string, string> Inputs { get; init; } = new Dictionary<string, string>();
}