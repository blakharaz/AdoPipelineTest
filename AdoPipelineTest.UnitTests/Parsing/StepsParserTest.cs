using Xunit;
using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Parsing;

public class StepsParserTest
{
    [Fact]
    public void ParseStep_WithEmptyScriptNode_ThrowsInvalidPipelineException()
    {
        var ex = Assert.Throws<InvalidPipelineException>(
            () => PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_empty_script.yaml")
        );
        
        Assert.Contains("script node has no content", ex?.Message);
    }

    [Fact]
    public void Parse_PipelineWithStageAndJobNames_ParsesCorrectly()
    {
        var result = PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_stage_and_job_names.yaml");
        
        Assert.Equal(2, result.Stages.Count);
        
        var buildStage = result.Stages[0];
        Assert.Equal("Build", buildStage.Name);
        Assert.Equal("Build Stage", buildStage.DisplayName);
        Assert.Contains("Prep", buildStage.DependsOn);

        var compileJob = buildStage.Jobs[0];
        Assert.Equal("Compile", compileJob.Name);
        Assert.Equal("Compile Job", compileJob.DisplayName);
        Assert.Contains("Setup", compileJob.DependsOn);

        var deployStage = result.Stages[1];
        Assert.Equal("Deploy", deployStage.Name);
        Assert.Equal(2, deployStage.DependsOn.Count);
        Assert.Contains("Build", deployStage.DependsOn);
        Assert.Contains("Test", deployStage.DependsOn);
        
        var releaseJob = deployStage.Jobs[0];
        Assert.Equal("Release", releaseJob.Name);
        Assert.Contains("Package", releaseJob.DependsOn);
    }
}