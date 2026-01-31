namespace AdoPipelineTest.Parsing.Ast;

public class VariableExpression(string name) : Expression
{
    public string Name { get; } = name;

    public VariableExpression(IEnumerable<char> name)
        : this(new string(name.ToArray()))
    {
    }
}