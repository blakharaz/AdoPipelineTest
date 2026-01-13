namespace AdoPipelineTest.Parsing.Ast;

internal class PipelineVariableElement
{
    internal string Name { get; init; } = string.Empty;
    
    internal object? DefaultValue { get; init; }
}
