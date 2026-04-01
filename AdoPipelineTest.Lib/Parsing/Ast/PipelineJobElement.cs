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
        Name = jobWithTemplates.Name;
        DependsOn = jobWithTemplates.DependsOn;
    }

    internal string? Name { get; init; }
    internal string? DisplayName { get; init; }
    internal IList<string> DependsOn { get; init; } = [];
    internal IList<PipelineStepElement> Steps { get; init; } = [];
}