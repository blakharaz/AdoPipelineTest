namespace AdoPipelineTest.Parsing.Ast;

internal class PipelineResourceElement : PipelineElement
{
    internal string Name { get; init; } = string.Empty;
    internal string Type { get; init; } = string.Empty;
    internal string? Source { get; init; }
    internal string? Version { get; init; }
    internal IList<string>? Trigger { get; init; }
    internal IList<PipelineResourceEndpoint>? Endpoints { get; init; }

    public override string ToString() => $"PipelineResourceElement(Name={Name}, Type={Type})";
}