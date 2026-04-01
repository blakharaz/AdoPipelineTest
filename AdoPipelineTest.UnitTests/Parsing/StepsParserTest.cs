using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;

namespace AdoPipelineTest.UnitTests.Parsing;

[TestFixture]
public class StepsParserTest
{
    [Test]
    public void ParseStep_WithEmptyScriptNode_ThrowsInvalidPipelineException()
    {
        var ex = Assert.Throws<InvalidPipelineException>(
            () => PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_empty_script.yaml")
        );
        
        Assert.That(ex?.Message, Does.Contain("script node has no content"));
    }

    [Test]
    public void Parse_PipelineWithStageAndJobNames_ParsesCorrectly()
    {
        var result = PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_stage_and_job_names.yaml");
        
        Assert.That(result.Stages, Has.Count.EqualTo(2));
        
        var buildStage = result.Stages[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(buildStage.Name, Is.EqualTo("Build"));
            Assert.That(buildStage.DisplayName, Is.EqualTo("Build Stage"));
            Assert.That(buildStage.DependsOn, Does.Contain("Prep"));
        }

        var compileJob = buildStage.Jobs[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(compileJob.Name, Is.EqualTo("Compile"));
            Assert.That(compileJob.DisplayName, Is.EqualTo("Compile Job"));
            Assert.That(compileJob.DependsOn, Does.Contain("Setup"));
        }

        var deployStage = result.Stages[1];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deployStage.Name, Is.EqualTo("Deploy"));
            Assert.That(deployStage.DependsOn, Has.Count.EqualTo(2));
        }
        Assert.That(deployStage.DependsOn, Does.Contain("Build"));
        Assert.That(deployStage.DependsOn, Does.Contain("Test"));
        
        var releaseJob = deployStage.Jobs[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(releaseJob.Name, Is.EqualTo("Release"));
            Assert.That(releaseJob.DependsOn, Does.Contain("Package"));
        }
    }
}
