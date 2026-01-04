using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.RawModel;

namespace AdoPipelineTest.UnitTests.Parsing;

[TestFixture]
public class PipelineParserTest
{
    [Test]
    public void SimplePipelineWithJustSteps()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/simple_pipeline_just_steps.yaml");

        Assert.That(pipeline, Is.Not.Null);

        // Verify triggers
        Assert.That(pipeline.Triggers, Is.Not.Null);
        Assert.That(pipeline.Triggers.IncludedBranches, Has.Count.EqualTo(1));
        Assert.That(pipeline.Triggers.IncludedBranches, Does.Contain("main"));

        // Verify pool
        Assert.That(pipeline.AgentPool, Is.Not.Null);
        Assert.That(pipeline.AgentPool.VmImage, Is.EqualTo("ubuntu-latest"));

        // Verify stages and jobs
        Assert.That(pipeline.Stages, Has.Count.EqualTo(1));
        Assert.That(pipeline.Stages[0].Jobs, Has.Count.EqualTo(1));

        // Verify steps
        var steps = pipeline.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Has.Count.EqualTo(3));

        // Verify step 1 - NodeTool task
        var step1 = steps[0] as RawTaskStep;
        Assert.That(step1, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(step1.DisplayName, Is.EqualTo("Install Node.js"));
            Assert.That(step1.TaskName, Is.EqualTo("NodeTool@0"));
            Assert.That(step1.ContinueOnError, Is.Null);

            // Verify step 1 inputs
            Assert.That(step1.Inputs, Is.Not.Null);
            Assert.That(step1.Inputs, Does.ContainKey("versionSpec"));
            Assert.That(step1.Inputs["versionSpec"], Is.EqualTo("20.x"));
        }

        // Verify step 2 - npm install and build script
        var step2 = steps[1] as RawScriptStep;
        Assert.That(step2, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(step2.DisplayName, Is.EqualTo("npm install and build"));
            Assert.That(step2.ContinueOnError, Is.Null);
        }

        // Verify step 3 - npm test script with continueOnError
        var step3 = steps[2] as RawScriptStep;
        Assert.That(step3, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(step3.DisplayName, Is.EqualTo("npm test"));
            Assert.That(step3.ContinueOnError, Is.EqualTo("true"));
        }
    }
}