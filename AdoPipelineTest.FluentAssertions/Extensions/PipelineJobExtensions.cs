using AdoPipelineTest.Model;
using FluentAssertions.Execution;

namespace AdoPipelineTest.PipelineAssertions;

public static class PipelineJobExtensions
{
    public static PipelineJobAssertions Should(this PipelineJob job)
    {
        return new PipelineJobAssertions(job, AssertionChain.GetOrCreate());
    }
}
