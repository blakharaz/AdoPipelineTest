namespace AdoPipelineTest.Parsing.Ast;

public class StringLiteral : Expression
{
    public required string Value { get; init; }
}