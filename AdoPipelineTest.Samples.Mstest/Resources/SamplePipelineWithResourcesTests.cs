using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdoPipelineTest.Mstest;

namespace AdoPipelineTest.Samples.Mstest.Resources;

[TestClass]
public class SamplePipelineWithResourcesTests
{
    [TestMethod]
    public void ParseSamplePipelineWithResources()
    {
        var result = new PipelineTester()
            .WithPipeline("sample_pipeline_with_resources.yaml")
            .Run();

        Assert.IsNotNull(result);
        result.HasStageCount(2);

        var buildJob = result.Stages[0].Jobs[0];
        Assert.IsNotNull(buildJob);

        var deployJob = result.Stages[1].Jobs[0];
        Assert.IsNotNull(deployJob);

        Assert.AreEqual(2, buildJob.Steps.Count);
        Assert.AreEqual(1, deployJob.Steps.Count);
    }
}
