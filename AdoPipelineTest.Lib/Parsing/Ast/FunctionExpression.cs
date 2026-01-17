namespace AdoPipelineTest.Parsing.Ast;

public class FunctionExpression : Expression
{
    public required string FunctionName { get; init; }
    public IList<Expression> FunctionParameters { get; init; } = [];
}
