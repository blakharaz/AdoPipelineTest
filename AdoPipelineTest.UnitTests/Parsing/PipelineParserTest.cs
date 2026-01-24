using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.Ast;

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
        var step1 = steps[0] as TaskStepElement;
        Assert.That(step1, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(step1.DisplayName, Is.EqualTo("Install Node.js"));
            Assert.That(step1.TaskName, Is.EqualTo("NodeTool@0"));
            Assert.That(step1.ContinueOnError, Is.Null);

            // Verify step 1 inputs
            Assert.That(step1.Inputs, Is.Not.Null);
            Assert.That(step1.Inputs!, Does.ContainKey("versionSpec"));
            Assert.That(step1.Inputs!["versionSpec"], Is.EqualTo("20.x"));
        }

        // Verify step 2 - npm install and build script
        var step2 = steps[1] as ScriptStepElement;
        Assert.That(step2, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(step2.DisplayName, Is.EqualTo("npm install and build"));
            Assert.That(step2.ContinueOnError, Is.Null);
        }

        // Verify step 3 - npm test script with continueOnError
        var step3 = steps[2] as ScriptStepElement;
        Assert.That(step3, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(step3.DisplayName, Is.EqualTo("npm test"));
            Assert.That(step3.ContinueOnError, Is.EqualTo("true"));
        }
    }

    [Test]
    public void PipelineWithVariables()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_variables.yaml");

        Assert.That(pipeline, Is.Not.Null);

        // Verify variables exist
        Assert.That(pipeline.Variables, Has.Count.EqualTo(3));

        // Verify buildConfiguration variable
        var buildConfigVar = pipeline.Variables.FirstOrDefault(v => v.Name == "buildConfiguration");
        Assert.That(buildConfigVar, Is.Not.Null);
        Assert.That(buildConfigVar!.DefaultValue, Is.EqualTo("Release"));

        // Verify debugSymbols variable (boolean)
        var debugSymbolsVar = pipeline.Variables.FirstOrDefault(v => v.Name == "debugSymbols");
        Assert.That(debugSymbolsVar, Is.Not.Null);
        Assert.That(debugSymbolsVar!.DefaultValue, Is.EqualTo("true"));

        // Verify dotnetVersion variable
        var dotnetVersionVar = pipeline.Variables.FirstOrDefault(v => v.Name == "dotnetVersion");
        Assert.That(dotnetVersionVar, Is.Not.Null);
        Assert.That(dotnetVersionVar!.DefaultValue, Is.EqualTo("8.0.x"));

        // Verify triggers
        Assert.That(pipeline.Triggers, Is.Not.Null);
        Assert.That(pipeline.Triggers.IncludedBranches, Has.Count.EqualTo(1));
        Assert.That(pipeline.Triggers.IncludedBranches, Does.Contain("main"));

        // Verify pool
        Assert.That(pipeline.AgentPool, Is.Not.Null);
        Assert.That(pipeline.AgentPool.VmImage, Is.EqualTo("ubuntu-latest"));

        // Verify steps
        Assert.That(pipeline.Stages, Has.Count.EqualTo(1));
        Assert.That(pipeline.Stages[0].Jobs, Has.Count.EqualTo(1));
        var steps = pipeline.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Has.Count.EqualTo(2));
    }

    [Test]
    public void PipelineWithEmptyScriptNode_ThrowsInvalidPipelineException()
    {
        var ex = Assert.Throws<InvalidPipelineException>(
            () => PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_empty_script.yaml")
        );

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex?.Message, Does.Contain("script node has no content"));
    }

    [Test]
    public void PipelineWithUnterminatedStringInTemplateExpression_ThrowsInvalidPipelineException()
    {
        var ex = Assert.Throws<InvalidPipelineException>(() =>
            PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_unterminated_string.yaml")
        );

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex?.Message, Does.Contain("Unterminated string"));
    }

    [Test]
    public void PipelineWithConditionalStepInsertion1()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/simple_conditional_insertion.yaml");

        Assert.That(pipeline, Is.Not.Null);

        Assert.That(pipeline.Stages, Has.Count.EqualTo(1));
        Assert.That(pipeline.Stages[0].Jobs, Has.Count.EqualTo(1));
        
        var steps = pipeline.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Has.Count.EqualTo(2));
        
        Assert.That(steps[0], Is.InstanceOf<ConditionalStepExpression>());
        Assert.That(steps[1], Is.InstanceOf<ConditionalStepExpression>());
    }

    [Test]
    public void PipelineWithConditionalStepInsertion2()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/ifelse_conditional_step_insertion.yaml");

        Assert.That(pipeline, Is.Not.Null);

        Assert.That(pipeline.Stages, Has.Count.EqualTo(1));
        Assert.That(pipeline.Stages[0].Jobs, Has.Count.EqualTo(1));
        
        var steps = pipeline.Stages[0].Jobs[0].Steps;
        
        // Now we should have only 1 top-level conditional (the if-elseif-else chain)
        Assert.That(steps, Has.Count.EqualTo(1));
        Assert.That(steps[0], Is.InstanceOf<ConditionalStepExpression>());
        
        var ifStatement = steps[0] as ConditionalStepExpression;
        Assert.That(ifStatement, Is.Not.Null);
        
        // Verify the "if" branch
        Assert.That(ifStatement!.ThenSteps, Has.Count.EqualTo(1));
        Assert.That(ifStatement.ThenSteps[0], Is.InstanceOf<TaskStepElement>());
        
        // Verify the "else if" branch (nested in ElseBranch)
        Assert.That(ifStatement.ElseBranch, Is.InstanceOf<ConditionalStepExpression>());
        var elseIfStatement = ifStatement.ElseBranch as ConditionalStepExpression;
        Assert.That(elseIfStatement!.ThenSteps, Has.Count.EqualTo(3));
        
        // Verify the "else" branch (nested in the else-if's ElseBranch)
        Assert.That(elseIfStatement.ElseBranch, Is.InstanceOf<ConditionalStepExpression>());
        var elseStatement = elseIfStatement.ElseBranch as ConditionalStepExpression;
        Assert.That(elseStatement!.ThenSteps, Has.Count.EqualTo(1));
        Assert.That(elseStatement.ThenSteps[0], Is.InstanceOf<ScriptStepElement>());
    }
}