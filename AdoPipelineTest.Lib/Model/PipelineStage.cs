namespace AdoPipelineTest.Model;

public class PipelineStage
{
    public IList<PipelineJob> Jobs { get; init; } = [];
}