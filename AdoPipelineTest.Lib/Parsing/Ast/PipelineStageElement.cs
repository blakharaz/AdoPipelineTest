namespace AdoPipelineTest.Parsing.Ast;

internal class PipelineStageElement : PipelineElement
{
    internal PipelineStageElement() {}

    internal PipelineStageElement(PipelineStageElement other)
    {
        Jobs = other.Jobs;
        DisplayName = other.DisplayName;
        Name = other.Name;
        DependsOn = other.DependsOn;
    }

    internal string? Name { get; init; }
    internal string? DisplayName { get; init; }
    internal IList<string> DependsOn { get; init; } = [];
    internal IList<PipelineJobElement> Jobs { get; init; } = [];
}