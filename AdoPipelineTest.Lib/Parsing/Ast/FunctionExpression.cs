namespace AdoPipelineTest.Parsing.Ast;

public class FunctionExpression(string name, IList<Expression> parameters) : Expression
{
    public string FunctionName { get; } = name;
    public IList<Expression> FunctionParameters { get; } = parameters;

    public FunctionExpression(IEnumerable<char> name, IList<Expression> parameters)
    : this(new string(name.ToArray()), parameters)
    {
    }
}
