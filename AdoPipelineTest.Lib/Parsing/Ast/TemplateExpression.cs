namespace AdoPipelineTest.Parsing.Ast;

/// <summary>
/// Represents ${{ ... }} in ADO pipelines
/// </summary>
public class TemplateExpression : Expression
{
    public IList<Expression> Children { get; init; } = new List<Expression>();
}