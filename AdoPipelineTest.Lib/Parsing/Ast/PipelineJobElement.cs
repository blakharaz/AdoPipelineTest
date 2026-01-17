namespace AdoPipelineTest.Parsing.Ast;

internal class PipelineJobElement : PipelineElement
{
    internal PipelineJobElement()
    {
    }
    
    internal PipelineJobElement(PipelineJobElement jobWithTemplates)
    {
        Steps = jobWithTemplates.Steps;
        DisplayName = jobWithTemplates.DisplayName;
    }

    internal IList<PipelineStepElement> Steps { get; init; } = [];
    internal string? DisplayName { get; init; }
}