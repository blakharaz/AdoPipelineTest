namespace AdoPipelineTest.Parsing.Ast;

internal class PipelineStageElement
{
    internal PipelineStageElement() {}

    internal PipelineStageElement(PipelineStageElement other)
    {
        Jobs = other.Jobs;
        DisplayName = other.DisplayName;
    }

    internal IList<PipelineJobElement> Jobs { get; init; } = [];
    internal string? DisplayName { get; init; }
}