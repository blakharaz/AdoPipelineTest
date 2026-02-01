namespace AdoPipelineTest.Parsing.Ast;

public class Identifier(string name) : Expression
{
    public string Name { get; } = name;
}