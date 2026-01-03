namespace AdoPipelineTest.Parsing.RawModel;

internal class RawPipelineStage
{
    internal RawPipelineStage() {}

    internal RawPipelineStage(RawPipelineStage other)
    {
        Jobs = other.Jobs;
        DisplayName = other.DisplayName;
    }

    internal IList<RawPipelineJob> Jobs { get; init; } = [];
    internal string? DisplayName { get; init; }
}