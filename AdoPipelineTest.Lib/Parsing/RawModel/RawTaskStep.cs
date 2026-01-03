namespace AdoPipelineTest.Parsing.RawModel;

public class RawTaskStep : RawPipelineStep
{
    public required string TaskName { get; init; }
    public IDictionary<string, string> Inputs { get; init; } = new Dictionary<string, string>();
}