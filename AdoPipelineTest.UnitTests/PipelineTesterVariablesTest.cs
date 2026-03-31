using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.UnitTests;

[TestFixture]
public class PipelineTesterVariablesTest
{
    [Test]
    public void Run_UsesVariableDefaults_InTaskInputs()
    {
        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_variables.yaml")
            .Run();

        // Verify variables are captured in result
        Assert.That(result.Variables, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Variables.Select(v => v.Name), Does.Contain("buildConfiguration"));

            // Verify stages are evaluated
            Assert.That(result.Stages, Has.Count.EqualTo(1));
        }
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Has.Count.EqualTo(2));

        // Verify the second step (DotNetCoreCLI) has the variable replaced in inputs
        var buildTask = steps[1] as TaskStep;
        Assert.That(buildTask, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(buildTask!.Inputs, Does.ContainKey("arguments"));

            // The variable $(buildConfiguration) should be replaced with 'Release'
            Assert.That(buildTask.Inputs["arguments"], Does.Contain("Release"));
        }
    }

    [Test]
    public void Run_OverridesVariableDefaults_WithUserProvidedVariables()
    {
        var customVariables = new Dictionary<string, object>
        {
            ["buildConfiguration"] = "Debug"
        };

        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_variables.yaml")
            .WithVariables(customVariables)
            .Run();

        // Verify stages are evaluated
        Assert.That(result.Stages, Has.Count.EqualTo(1));
        var steps = result.Stages[0].Jobs[0].Steps;

        // Verify the second step has the overridden variable value
        var buildTask = steps[1] as TaskStep;
        Assert.That(buildTask, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(buildTask!.Inputs, Does.ContainKey("arguments"));

            // The variable should be replaced with 'Debug' (user-provided), not 'Release' (default)
            Assert.That(buildTask.Inputs["arguments"], Does.Contain("Debug"));
        }
        Assert.That(buildTask.Inputs["arguments"], Does.Not.Contain("Release"));
    }

    [Test]
    public void Run_MergesMultipleVariables()
    {
        var customVariables = new Dictionary<string, object>
        {
            ["customVar"] = "CustomValue"
        };

        var result = new PipelineTester()
            .WithPipeline("test_data/pipeline_parser/pipeline_with_variables.yaml")
            .WithVariables(customVariables)
            .Run();

        // Verify both default and custom variables are in the result
        Assert.That(result.Variables, Has.Count.EqualTo(3));
        var varNames = result.Variables.Select(v => v.Name);
        Assert.That(varNames, Does.Contain("buildConfiguration"));
    }
}
