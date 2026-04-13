using AdoPipelineTest.Model;
using FluentAssertions.Execution;

namespace AdoPipelineTest.PipelineAssertions;

public static class PipelineTriggersExtensions
{
    public static PipelineTriggersAssertions Should(this PipelineTriggers triggers)
    {
        return new PipelineTriggersAssertions(triggers, AssertionChain.GetOrCreate());
    }
}
