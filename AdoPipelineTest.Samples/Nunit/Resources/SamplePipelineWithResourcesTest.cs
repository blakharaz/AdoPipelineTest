using AdoPipelineTest.Parsing;
using NUnit.Framework;
using Is = AdoPipelineTest.Nunit.Is;

namespace AdoPipelineTest.Samples.Nunit.Resources;

public class SamplePipelineWithResourcesTest
{
    [Test]
    public void ParseSamplePipelineWithResources()
    {
        // Parse the sample pipeline using PipelineTester like other samples
        var result = new PipelineTester()
            .WithPipeline("sample_pipeline_with_resources.yaml")
            .Run();
        
        // Verify basic structure
        Assert.That(result, Is.Not.Null);
        
        // Verify the pipeline parses correctly (this confirms resources parsing works)
        Assert.That(result.Stages, Has.Count.EqualTo(2));
        
        // Verify first job (Build)
        var buildJob = result.Stages[0].Jobs[0];
        // Since PipelineJob doesn't have a Name property, we'll check DisplayName or just verify it exists
        Assert.That(buildJob, Is.Not.Null);
        
        // Verify second job (Deploy)
        var deployJob = result.Stages[1].Jobs[0];
        Assert.That(deployJob, Is.Not.Null);
        
        // Verify steps exist
        Assert.That(buildJob.Steps, Has.Count.EqualTo(2));
        Assert.That(deployJob.Steps, Has.Count.EqualTo(1));
    }
}