using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.Samples.Shouldly.Resources;

[TestFixture]
public class SamplePipelineWithResourcesTests
{
    private const string YamlPath = "pipelines/Resources/sample_pipeline_with_resources.yaml";
    
    [Test]
    public void ParseSamplePipelineWithResources()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();
        
        result.ShouldNotBeNull();
        result.Stages.Count.ShouldBe(2);
        
        var buildJob = result.Stages[0].Jobs[0];
        buildJob.ShouldNotBeNull();
        
        var deployJob = result.Stages[1].Jobs[0];
        deployJob.ShouldNotBeNull();

        buildJob.Steps.Count.ShouldBe(2);
        deployJob.Steps.Count.ShouldBe(1);
    }
}
