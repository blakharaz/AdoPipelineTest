using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.Ast;
using Xunit;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Parsing;

public class PipelineParserTest
{
    [Fact]
    public void SimplePipelineWithJustSteps()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/simple_pipeline_just_steps.yaml");

        Assert.NotNull(pipeline);

        Assert.NotNull(pipeline.Triggers);
        Assert.Single(pipeline.Triggers.IncludedBranches);
        Assert.Contains("main", pipeline.Triggers.IncludedBranches);

        Assert.NotNull(pipeline.AgentPool);
        Assert.Equal("ubuntu-latest", pipeline.AgentPool.VmImage);

        Assert.Single(pipeline.Stages);
        Assert.Single(pipeline.Stages[0].Jobs);

        var steps = pipeline.Stages[0].Jobs[0].Steps;
        Assert.Equal(3, steps.Count);

        var step1 = steps[0] as TaskStepElement;
        Assert.NotNull(step1);
        Assert.Equal("Install Node.js", step1.DisplayName);
        Assert.Equal("NodeTool@0", step1.TaskName);
        Assert.Null(step1.ContinueOnError);
        Assert.NotNull(step1.Inputs);
        Assert.Contains("versionSpec", step1.Inputs!.Keys);
        Assert.Equal("20.x", step1.Inputs["versionSpec"]);

        var step2 = steps[1] as ScriptStepElement;
        Assert.NotNull(step2);
        Assert.Equal("npm install and build", step2.DisplayName);
        Assert.Null(step2.ContinueOnError);

        var step3 = steps[2] as ScriptStepElement;
        Assert.NotNull(step3);
        Assert.Equal("npm test", step3.DisplayName);
        Assert.Equal("true", step3.ContinueOnError);
    }

    [Fact]
    public void PipelineWithVariables()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_variables.yaml");

        Assert.NotNull(pipeline);

        Assert.Equal(3, pipeline.Variables.Count);

        var buildConfigVar = pipeline.Variables.FirstOrDefault(v => v.Name == "buildConfiguration");
        Assert.NotNull(buildConfigVar);
        Assert.Equal("Release", buildConfigVar!.DefaultValue);

        var debugSymbolsVar = pipeline.Variables.FirstOrDefault(v => v.Name == "debugSymbols");
        Assert.NotNull(debugSymbolsVar);
        Assert.Equal("true", debugSymbolsVar!.DefaultValue);

        var dotnetVersionVar = pipeline.Variables.FirstOrDefault(v => v.Name == "dotnetVersion");
        Assert.NotNull(dotnetVersionVar);
        Assert.Equal("8.0.x", dotnetVersionVar!.DefaultValue);

        Assert.NotNull(pipeline.Triggers);
        Assert.Single(pipeline.Triggers.IncludedBranches);
        Assert.Contains("main", pipeline.Triggers.IncludedBranches);

        Assert.NotNull(pipeline.AgentPool);
        Assert.Equal("ubuntu-latest", pipeline.AgentPool!.VmImage);

        Assert.Single(pipeline.Stages);
        Assert.Single(pipeline.Stages[0].Jobs);
        var steps = pipeline.Stages[0].Jobs[0].Steps;
        Assert.Equal(2, steps.Count);
    }

    [Fact]
    public void PipelineWithEmptyScriptNode_ThrowsInvalidPipelineException()
    {
        var ex = Assert.Throws<InvalidPipelineException>(
            () => PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_empty_script.yaml")
        );

        Assert.NotNull(ex);
        Assert.Contains("script node has no content", ex!.Message);
    }

    [Fact]
    public void PipelineWithUnterminatedStringInTemplateExpression_ThrowsInvalidPipelineException()
    {
        Assert.Throws<InvalidPipelineException>(() =>
            PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_unterminated_string.yaml")
        );
    }

    [Fact]
    public void PipelineWithConditionalStepInsertion1()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/simple_conditional_insertion.yaml");

        Assert.NotNull(pipeline);

        Assert.Single(pipeline.Stages);
        Assert.Single(pipeline.Stages[0].Jobs);
        
        var steps = pipeline.Stages[0].Jobs[0].Steps;
        Assert.Equal(2, steps.Count);
        Assert.IsType<ConditionalStepExpression>(steps[0]);
        Assert.IsType<ConditionalStepExpression>(steps[1]);
    }

    [Fact]
    public void PipelineWithConditionalStepInsertion2()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/ifelse_conditional_step_insertion.yaml");

        Assert.NotNull(pipeline);

        Assert.Single(pipeline.Stages);
        Assert.Single(pipeline.Stages[0].Jobs);
        
        var steps = pipeline.Stages[0].Jobs[0].Steps;
        
        Assert.Single(steps);
        Assert.IsType<ConditionalStepExpression>(steps[0]);
        
        var ifStatement = steps[0] as ConditionalStepExpression;
        Assert.NotNull(ifStatement);
        Assert.Single(ifStatement!.ThenSteps);
        Assert.IsType<TaskStepElement>(ifStatement.ThenSteps[0]);

        Assert.IsType<ConditionalStepExpression>(ifStatement.ElseBranch);
        var elseIfStatement = ifStatement.ElseBranch as ConditionalStepExpression;
        Assert.NotNull(elseIfStatement);
        Assert.Equal(3, elseIfStatement!.ThenSteps.Count);

        Assert.IsType<ConditionalStepExpression>(elseIfStatement.ElseBranch);
        var elseStatement = elseIfStatement.ElseBranch as ConditionalStepExpression;
        Assert.NotNull(elseStatement);
        Assert.Single(elseStatement!.ThenSteps);
        Assert.IsType<ScriptStepElement>(elseStatement.ThenSteps[0]);
    }
}
