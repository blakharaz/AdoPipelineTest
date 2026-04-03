using NUnit.Framework;
using Does = AdoPipelineTest.Nunit.Does;
using Has = AdoPipelineTest.Nunit.Has;
using Is = AdoPipelineTest.Nunit.Is;

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

        Assert.That(result, Has.Stage("Build"));
        Assert.That(result, Has.Stage("Deploy"));
        Assert.That(result, Has.Stage("Build Stage"));
        
        Assert.That(result.Stages[0], Has.Job("Compile"));
        Assert.That(result.Stages[0], Has.Job("Compile Job"));
        
        Assert.That(result.Stages[0].Jobs[0], Has.Step("Build Task"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Stages[0].Jobs[0], Has.Task("DotNetCoreCLI@2"));

            Assert.That(result.Stages[0], Does.DependOn("Prep"));
        }
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Stages[0].Jobs[0], Does.DependOn("Setup"));
            Assert.That(result.Stages[1], Does.DependOn("Build"));
        }
        Assert.That(result.Stages[1], Does.DependOn("Test"));
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
