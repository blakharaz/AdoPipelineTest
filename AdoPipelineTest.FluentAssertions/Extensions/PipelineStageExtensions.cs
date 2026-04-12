using AdoPipelineTest.Model;

namespace AdoPipelineTest.PipelineAssertions;

public static class PipelineStageExtensions
{
    public static PipelineStageAssertions Should(this PipelineStage stage)
    {
        return new PipelineStageAssertions(stage);
    }
}
