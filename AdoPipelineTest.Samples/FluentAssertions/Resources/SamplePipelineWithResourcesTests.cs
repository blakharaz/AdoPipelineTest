using AdoPipelineTest.PipelineAssertions;
using FluentAssertions;

namespace AdoPipelineTest.Samples.FluentAssertions.Resources;

[TestClass]
public class SamplePipelineWithResourcesTests
{
    private const string YamlPath = "pipelines/Resources/sample_pipeline_with_resources.yaml";

    [TestMethod]
    public void ParseSamplePipelineWithResources()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        result.Should().HaveStageCount(2);

        var buildJob = result.Stages[0].Jobs[0];
        buildJob.Steps.Should().HaveCount(2);

        var deployJob = result.Stages[1].Jobs[0];
        deployJob.Steps.Should().HaveCount(1);
    }
}
