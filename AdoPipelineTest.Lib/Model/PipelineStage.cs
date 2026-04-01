namespace AdoPipelineTest.Model;

public class PipelineStage
{
    public string? Name { get; init; }
    public string? DisplayName { get; init; }
    public IList<string> DependsOn { get; init; } = [];
    public IList<PipelineJob> Jobs { get; init; } = [];
}