namespace AdoPipelineTest.Parsing.RawModel;

public class RawPipelineStage
{
    public IList<RawPipelineJob> Jobs { get; init; } = [];
    public string? DisplayName { get; init; }
}