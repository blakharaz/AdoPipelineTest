using AdoPipelineTest.Model;

namespace AdoPipelineTest.PipelineAssertions;

public static class PipelineTestResultExtensions
{
    public static PipelineTestResultAssertions Should(this PipelineTestResult result)
    {
        return new PipelineTestResultAssertions(result);
    }
}
