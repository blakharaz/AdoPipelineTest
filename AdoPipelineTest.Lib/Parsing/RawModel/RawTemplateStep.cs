namespace AdoPipelineTest.Parsing.RawModel;

internal class RawTemplateStep : RawPipelineStep
{
    public required string Template { get; init; }
    public required string ReferencedBy { get; init; }
}