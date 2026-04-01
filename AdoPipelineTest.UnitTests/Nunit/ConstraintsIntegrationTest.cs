using PipelineIs = AdoPipelineTest.Nunit.Is;

namespace AdoPipelineTest.UnitTests.Nunit;

[TestFixture]
public class ConstraintsIntegrationTest
{
    [Test]
    public void PipelineConstraints_WorkWithRealPipeline()
    {
        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_stage_and_job_names.yaml")
            .Run();

        Assert.That(result, PipelineIs.HasStage("Build"));
        Assert.That(result, PipelineIs.HasStage("Deploy"));
        Assert.That(result, PipelineIs.HasStage("Build Stage"));
        
        Assert.That(result.Stages[0], PipelineIs.HasJob("Compile"));
        Assert.That(result.Stages[0], PipelineIs.HasJob("Compile Job"));
        
        Assert.That(result.Stages[0].Jobs[0], PipelineIs.HasStep("Build Task"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Stages[0].Jobs[0], PipelineIs.HasTask("DotNetCoreCLI@2"));

            Assert.That(result.Stages[0], PipelineIs.DependsOn("Prep"));
        }
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Stages[0].Jobs[0], PipelineIs.DependsOn("Setup"));
            Assert.That(result.Stages[1], PipelineIs.DependsOn("Build"));
        }
        Assert.That(result.Stages[1], PipelineIs.DependsOn("Test"));
    }

    [Test]
    public void PipelineConstraints_FailingAssertions()
    {
        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_stage_and_job_names.yaml")
            .Run();

        Assert.That(result, Is.Not.Null);
    }
}
