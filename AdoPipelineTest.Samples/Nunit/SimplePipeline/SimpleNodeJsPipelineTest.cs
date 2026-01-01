namespace AdoPipelineTest.Samples.Nunit.SimplePipeline;

using Is = AdoPipelineTest.Nunit.Is;

public class SimpleNodeJsPipelineTest
{
    [Test]
    public void Test1()
    {
        var result = new PipelineTester().Run("Nunit/SimplePipeline/simple_nodejs_pipeline.yaml");
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Triggers, Is.BranchIncluded("main"));
        Assert.That(result.AgentPool, Is.VmImage("ubuntu-latest"));
        Assert.That(result.Stages, Has.Count.EqualTo(1));
        Assert.That(result.Stages[0].Jobs, Has.Count.EqualTo(1));

        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Has.Count.EqualTo(3));
    }
}