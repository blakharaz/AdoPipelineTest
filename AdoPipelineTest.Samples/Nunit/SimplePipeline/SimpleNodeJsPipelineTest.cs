using NUnit.Framework;
using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.Samples.Nunit.SimplePipeline;

using Is = AdoPipelineTest.Nunit.Is;

public class SimpleNodeJsPipelineTest
{
    private const string YamlPath = "pipelines/SimplePipeline/simple_nodejs_pipeline.yaml";

    [Test]
    public void VerifyBasics()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Triggers, Is.BranchIncluded("main"));
            Assert.That(result.AgentPool, Is.VmImage("ubuntu-latest"));
            Assert.That(result.Stages, Has.Count.EqualTo(1));
            Assert.That(result.Stages[0].Jobs, Has.Count.EqualTo(1));

            var steps = result.Stages[0].Jobs[0].Steps;
            Assert.That(steps, Has.Count.EqualTo(3));
        }
    }

    [Test]
    public void VerifyStep1()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step1 = steps[0] as TaskStep;
            
        Assert.That(step1, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(step1.DisplayName, Is.EqualTo("Install Node.js"));
            Assert.That(step1.ContinueOnError, Is.False);
            Assert.That(step1.TaskName, Is.EqualTo("NodeTool@0"));
        }
    }

    [Test]
    public void VerifyStep2()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step2 = steps[1] as ScriptStep;

        Assert.That(step2, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(step2.DisplayName, Is.EqualTo("npm install and build"));
            Assert.That(step2.ContinueOnError, Is.False);
            Assert.That(step2.Script, Does.Contain("npm install").And.Contain("npm run build"));
        }
    }
    [Test]
    public void VerifyStep3()
    {
        var result = new PipelineTester().WithPipeline(YamlPath).Run();

        Assert.That(result, Is.Not.Null);
        var steps = result.Stages[0].Jobs[0].Steps;
        var step3 = steps[2] as ScriptStep;

        Assert.That(step3, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(step3.DisplayName, Is.EqualTo("npm test"));
            Assert.That(step3.ContinueOnError, Is.True);
            Assert.That(step3.Script, Does.Contain("npm test"));
        }
    }
}