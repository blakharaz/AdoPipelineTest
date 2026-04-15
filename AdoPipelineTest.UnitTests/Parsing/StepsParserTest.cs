using Xunit;
using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.Ast;
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

    [Fact]
    public void Parse_PipelineWithTemplateStepWithParameters_CapturesParameters()
    {
        var result = PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_template_step_with_parameters.yaml");
        
        Assert.Single(result.Stages);
        var stage = result.Stages[0];
        Assert.Single(stage.Jobs);
        var job = stage.Jobs[0];
        
        var templateStep = job.Steps.OfType<TemplateStepElement>().FirstOrDefault();
        Assert.NotNull(templateStep);
        Assert.Equal("templates/build-template.yaml", templateStep.Template);
        Assert.NotNull(templateStep.ReferencedBy);
        Assert.Equal(2, templateStep.Parameters.Count);
        Assert.Equal("MyProject.csproj", templateStep.Parameters["projectName"]);
        Assert.Equal("Release", templateStep.Parameters["configuration"]);
    }

    [Fact]
    public void Parse_PipelineWithTemplateStepWithoutParameters_HasEmptyParameters()
    {
        var result = PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_template_step_without_parameters.yaml");
        
        Assert.Single(result.Stages);
        var stage = result.Stages[0];
        Assert.Single(stage.Jobs);
        var job = stage.Jobs[0];
        
        var templateStep = job.Steps.OfType<TemplateStepElement>().FirstOrDefault();
        Assert.NotNull(templateStep);
        Assert.Equal("templates/simple-template.yaml", templateStep.Template);
        Assert.NotNull(templateStep.ReferencedBy);
        Assert.Empty(templateStep.Parameters);
    }
}