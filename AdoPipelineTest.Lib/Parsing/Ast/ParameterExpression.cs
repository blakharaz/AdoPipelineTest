namespace AdoPipelineTest.Parsing.Ast;

/// <summary>
/// Represents "parameters.Foo" in a string expression
/// </summary>
public class ParameterExpression(string parameterName) : Expression
{
    public string ParameterName { get; } = parameterName;

    public ParameterExpression(IEnumerable<char> parameterName)
        : this(new string(parameterName.ToArray()))
    {
    }
}