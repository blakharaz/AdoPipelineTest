namespace AdoPipelineTest.Parsing.Ast;

internal class PipelineResourceEndpoint
{
    internal string Name { get; init; } = string.Empty;
    internal IDictionary<string, object?>? Auth { get; init; }
    internal string? Value { get; init; }
}