namespace AdoPipelineTest.Model;

public class PipelineJob
{
    public IList<PipelineStep> Steps { get; init; } = [];
    public string? DisplayName { get; init; }
}