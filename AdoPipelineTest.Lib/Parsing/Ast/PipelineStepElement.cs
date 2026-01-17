namespace AdoPipelineTest.Parsing.Ast;

public class PipelineStepElement : PipelineElement
{
    public string? DisplayName { get; init; }
    public string? ContinueOnError { get; init; }
}