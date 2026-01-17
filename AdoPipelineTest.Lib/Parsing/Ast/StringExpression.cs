namespace AdoPipelineTest.Parsing.Ast;

public class StringExpression : Expression
{
    public IList<Expression> Children { get; } = new List<Expression>();
}