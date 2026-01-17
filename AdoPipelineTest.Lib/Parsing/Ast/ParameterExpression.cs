namespace AdoPipelineTest.Parsing.Ast;

/// <summary>
/// Represents "parameters.Foo" in a string expression
/// </summary>
public class ParameterExpression : Expression
{
    public required string ParameterName { get; init; }
}