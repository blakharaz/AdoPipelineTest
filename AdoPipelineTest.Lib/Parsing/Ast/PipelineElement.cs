namespace AdoPipelineTest.Parsing.Ast;

public abstract class PipelineElement
{
    public string Filename { get; init; } = "";
    public int StartLine { get; init; }
    public int StartColumn { get; init; }
    public int EndLine { get; init; }
    public int EndColumn { get; init; }
}