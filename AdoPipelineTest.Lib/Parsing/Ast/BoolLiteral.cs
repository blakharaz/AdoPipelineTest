namespace AdoPipelineTest.Parsing.Ast;

public class BoolLiteral : Expression
{
    public required bool Value { get; init; }
}