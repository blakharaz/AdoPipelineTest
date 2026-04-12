using AdoPipelineTest.Model;

namespace AdoPipelineTest.PipelineAssertions;

public static class PipelineTriggersExtensions
{
    public static PipelineTriggersAssertions Should(this PipelineTriggers triggers)
    {
        return new PipelineTriggersAssertions(triggers);
    }
}
