namespace AdoPipelineTest.Parsing.Ast;

/// <summary>
/// Represents $(...) runtime expressions in ADO pipelines
/// </summary>
public class RuntimeExpression : Expression
{
    public string VariableName { get; init; } = string.Empty;
}