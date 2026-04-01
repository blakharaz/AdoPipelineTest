namespace AdoPipelineTest.Model;

public class PipelineJob
{
    public string? Name { get; init; }
    public string? DisplayName { get; init; }
    public IList<string> DependsOn { get; init; } = [];
    public IList<PipelineStep> Steps { get; init; } = [];
}