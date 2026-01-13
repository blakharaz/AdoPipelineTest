namespace AdoPipelineTest.Parsing.Ast;

internal class TemplateStepElement : PipelineStepElement
{
    public required string Template { get; init; }
    public required string ReferencedBy { get; init; }
}