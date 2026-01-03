namespace AdoPipelineTest.Parsing.RawModel;

public class RawPipelineJob
{
    public IList<RawPipelineStep> Steps { get; init; } = [];
    public string? DisplayName { get; init; }
}