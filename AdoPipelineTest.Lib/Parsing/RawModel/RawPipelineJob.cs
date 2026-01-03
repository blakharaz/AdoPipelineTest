namespace AdoPipelineTest.Parsing.RawModel;

internal class RawPipelineJob
{
    internal RawPipelineJob()
    {
    }
    
    internal RawPipelineJob(RawPipelineJob jobWithTemplates)
    {
        Steps = jobWithTemplates.Steps;
        DisplayName = jobWithTemplates.DisplayName;
    }

    internal IList<RawPipelineStep> Steps { get; init; } = [];
    internal string? DisplayName { get; init; }
}