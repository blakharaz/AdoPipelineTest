using AdoPipelineTest.Model;

namespace AdoPipelineTest.PipelineAssertions;

public static class PipelineJobExtensions
{
    public static PipelineJobAssertions Should(this PipelineJob job)
    {
        return new PipelineJobAssertions(job);
    }
}
