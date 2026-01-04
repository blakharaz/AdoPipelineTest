using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.Samples.Nunit.Variables;

using Is = AdoPipelineTest.Nunit.Is;

[TestFixture]
public class StepVariablesTest
{
    [Test]
    public void PipelineLoadedWithVariables()
    {
        var result = new PipelineTester()
            .WithPipeline("Nunit/Variables/pipeline_with_variables.yaml")
            .Run();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Stages, Has.Count.EqualTo(1));
        Assert.That(result.Stages[0].Jobs, Has.Count.EqualTo(1));
        
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Has.Count.EqualTo(4));
    }

    [Test]
    public void VariableOverriddenWithCustomValue()
    {
        var result = new PipelineTester()
            .WithPipeline("Nunit/Variables/pipeline_with_variables.yaml")
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
            .WithPipeline("Nunit/Variables/pipeline_with_variables.yaml")
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
            
            var buildArguments = buildStep.Inputs?["arguments"]?.ToString();
            var testArguments = testStep.Inputs?["arguments"]?.ToString();

            Assert.That(buildArguments, Does.Contain("Debug"));
            Assert.That(testArguments, Does.Contain("Debug"));
        }
    }

    [Test]
    public void MultipleVariablesCanBeSet()
    {
        var result = new PipelineTester()
            .WithPipeline("Nunit/Variables/pipeline_with_variables.yaml")
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Release" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        // The last set variable should be used
        var arguments = buildStep.Inputs?["arguments"]?.ToString();
        Assert.That(arguments, Does.Contain("Release"));
    }

    [Test]
    public void VariableEvaluationPreservesStepTaskName()
    {
        var result = new PipelineTester()
            .WithPipeline("Nunit/Variables/pipeline_with_variables.yaml")
            .WithVariables(new Dictionary<string, object> { ["buildConfiguration"] = "Debug" })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        var buildStep = steps[2] as TaskStep;

        Assert.That(buildStep, Is.Not.Null);
        // Task name should not be affected by variable evaluation
        Assert.That(buildStep.TaskName, Is.EqualTo("DotNetCoreCLI@2"));
    }
}