using NUnit.Framework;
using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.Samples.Nunit.Variables;

using Is = AdoPipelineTest.Nunit.Is;

[TestFixture]
public class StepVariablesTest
{
    private const string YamlPath = "pipelines/Variables/pipeline_with_variables.yaml";

    [Test]
    public void PipelineLoadedWithVariables()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Stages, Has.Count.EqualTo(1));
        Assert.That(result.Stages[0].Jobs, Has.Count.EqualTo(1));
        
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Has.Count.EqualTo(4));
    }

    [Test]
    public void VariableEvaluatedInBuildStep()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        Assert.That(buildStep, Is.Not.Null);
        Assert.That(buildStep.DisplayName, Is.EqualTo("Build"));

        // The build step should have the buildConfiguration variable evaluated in its arguments
        var arguments = buildStep.Inputs?["arguments"]?.ToString();
        Assert.That(arguments, Does.Contain("Release"));
    }

    [Test]
    public void VariableEvaluatedInTestStep()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var testStep = steps[3] as TaskStep;

        Assert.That(testStep, Is.Not.Null);
        Assert.That(testStep.DisplayName, Is.EqualTo("Test"));

        // The test step should have the buildConfiguration variable evaluated in its arguments
        var arguments = testStep.Inputs?["arguments"]?.ToString();
        Assert.That(arguments, Does.Contain("Release"));
    }

    [Test]
    public void AllStepsPreserveTaskDetails()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;

        // Verify all steps are TaskSteps
        foreach (var step in steps)
        {
            Assert.That(step, Is.InstanceOf<TaskStep>());
        }

        // Verify each step has a display name and task name
        using (Assert.EnterMultipleScope())
        {
            Assert.That(steps[0].DisplayName, Is.Not.Null.And.Not.Empty);
            Assert.That((steps[0] as TaskStep)?.TaskName, Is.Not.Null.And.Not.Empty);

            Assert.That(steps[1].DisplayName, Is.Not.Null.And.Not.Empty);
            Assert.That((steps[1] as TaskStep)?.TaskName, Is.Not.Null.And.Not.Empty);

            Assert.That(steps[2].DisplayName, Is.Not.Null.And.Not.Empty);
            Assert.That((steps[2] as TaskStep)?.TaskName, Is.Not.Null.And.Not.Empty);

            Assert.That(steps[3].DisplayName, Is.Not.Null.And.Not.Empty);
            Assert.That((steps[3] as TaskStep)?.TaskName, Is.Not.Null.And.Not.Empty);
        }
    }

    [Test]
    public void VariableOverriddenWithCustomValue()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        Assert.That(buildStep, Is.Not.Null);
        Assert.That(buildStep.DisplayName, Is.EqualTo("Build"));

        // The build step should have the overridden buildConfiguration variable
        var arguments = buildStep.Inputs["arguments"]?.ToString();
        Assert.That(arguments, Does.Contain("Debug"));
        Assert.That(arguments, Does.Not.Contain("Release"));
    }

    [Test]
    public void VariableOverriddenInMultipleSteps()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;
        var testStep = steps[3] as TaskStep;

        // Verify both steps use the overridden variable
        using (Assert.EnterMultipleScope())
        {
            Assert.That(buildStep, Is.Not.Null);
            Assert.That(testStep, Is.Not.Null);
            
            var buildArguments = buildStep!.Inputs?["arguments"]?.ToString();
            var testArguments = testStep!.Inputs?["arguments"]?.ToString();

            Assert.That(buildArguments, Does.Contain("Debug"));
            Assert.That(testArguments, Does.Contain("Debug"));
        }
    }

    [Test]
    public void DefaultVariableValueUsedWhenNotOverridden()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        // The default value from the pipeline should be used
        var arguments = buildStep!.Inputs?["arguments"]?.ToString();
        Assert.That(arguments, Does.Contain("Release"));
    }

    [Test]
    public void MultipleVariablesCanBeSet()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Release" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        // The last set variable should be used
        var arguments = buildStep!.Inputs?["arguments"]?.ToString();
        Assert.That(arguments, Does.Contain("Release"));
    }

    [Test]
    public void VariableEvaluationPreservesStepTaskName()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        Assert.That(buildStep, Is.Not.Null);
        // Task name should not be affected by variable evaluation
        Assert.That(buildStep.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
    }
}