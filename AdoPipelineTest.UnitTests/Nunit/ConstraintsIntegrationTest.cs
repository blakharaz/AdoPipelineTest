using AdoPipelineTest.Nunit;
using NUnit.Framework.Constraints;
using AIs = AdoPipelineTest.Nunit.Is;

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

        Assert.That(result, AIs.HasStage("Build"));
        Assert.That(result, AIs.HasStage("Deploy"));
        Assert.That(result, AIs.HasStage("Build Stage"));
        
        Assert.That(result.Stages[0], AIs.HasJob("Compile"));
        Assert.That(result.Stages[0], AIs.HasJob("Compile Job"));
        
        Assert.That(result.Stages[0].Jobs[0], AIs.HasStep("Build Task"));
        Assert.That(result.Stages[0].Jobs[0], AIs.HasTask("DotNetCoreCLI@2"));
        
        Assert.That(result.Stages[0], AIs.DependsOn("Prep"));
        Assert.That(result.Stages[0].Jobs[0], AIs.DependsOn("Setup"));
        Assert.That(result.Stages[1], AIs.DependsOn("Build"));
        Assert.That(result.Stages[1], AIs.DependsOn("Test"));
    }

    [Test]
    public void PipelineConstraints_FailingAssertions()
    {
        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_stage_and_job_names.yaml")
            .Run();

        Assert.That(result, NUnit.Framework.Is.Not.EqualTo(null));
        Assert.That(result, NUnit.Framework.Is.Not.Null);
    }
}
