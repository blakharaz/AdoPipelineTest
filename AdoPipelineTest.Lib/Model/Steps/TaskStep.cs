namespace AdoPipelineTest.Model.Steps;

public class TaskStep : PipelineStep
{
    public required string TaskName { get; init; }
    public IDictionary<string, string> Inputs { get; init; } = new Dictionary<string, string>();
}