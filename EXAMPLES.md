# Examples - AdoPipelineTest

Comprehensive examples for common testing scenarios.

## Table of Contents

- [Basic Usage](#basic-usage)
- [Testing Parameters](#testing-parameters)
- [Testing Variables](#testing-variables)
- [Testing Conditional Steps](#testing-conditional-steps)
- [Testing Multi-Stage Pipelines](#testing-multi-stage-pipelines)
- [Testing Step Types](#testing-step-types)
- [Testing Triggers](#testing-triggers)
- [Testing NUnit Constraints](#testing-nunit-constraints)
- [Testing Templates](#testing-templates)
- [Advanced Scenarios](#advanced-scenarios)

## Basic Usage

### Simple Pipeline Test

```csharp
[TestFixture]
public class SimplePipelineTest
{
    [Test]
    public void Pipeline_Loads_Successfully()
    {
        var result = new PipelineTester()
            .WithPipeline("azure-pipelines.yaml")
            .Run();

        Assert.That(result, Is.Not.Null);
    }
}
```

### Test Pipeline Has Expected Structure

```csharp
[Test]
public void Pipeline_HasCorrectPoolConfiguration()
{
    var result = new PipelineTester()
        .WithPipeline("azure-pipelines.yaml")
        .Run();

    Assert.That(result.Stages, Has.Length.GreaterThan(0));
    Assert.That(result.Stages[0].Name, Is.EqualTo("Build"));
}
```

## Testing Parameters

### Test with Different Parameter Values

```csharp
[TestFixture]
public class ParameterTestingTest
{
    [Test]
    public void Pipeline_WhenEnvironmentIsProduction_UsesProductionPool()
    {
        var result = new PipelineTester()
            .WithPipeline("parameterized-pipeline.yaml")
            .WithParameter("environment", "production")
            .Run();

        var pool = result.Stages[0].Pool;
        Assert.That(pool.VmImage, Is.EqualTo("windows-latest"));
    }

    [Test]
    public void Pipeline_WhenEnvironmentIsDevelopment_UsesDevelopmentPool()
    {
        var result = new PipelineTester()
            .WithPipeline("parameterized-pipeline.yaml")
            .WithParameter("environment", "development")
            .Run();

        var pool = result.Stages[0].Pool;
        Assert.That(pool.VmImage, Is.EqualTo("ubuntu-latest"));
    }
}
```

### Test Parameter Validation

```csharp
[Test]
public void Pipeline_WithMissingRequiredParameter_FailsValidation()
{
    var result = new PipelineTester()
        .WithPipeline("strict-parameters.yaml")
        // Missing required parameter 'environment'
        .Run();

    Assert.That(result.IsValid, Is.False);
    Assert.That(result.ValidationErrors, Is.Not.Empty);
}
```

### Parameterized Tests with Multiple Values

```csharp
[TestFixture]
public class MultiParameterTest
{
    [TestCase("dev", "ubuntu-latest")]
    [TestCase("staging", "ubuntu-latest")]
    [TestCase("prod", "windows-latest")]
    public void Pipeline_SelectsCorrectPoolForEnvironment(string env, string expectedImage)
    {
        var result = new PipelineTester()
            .WithPipeline("multi-env-pipeline.yaml")
            .WithParameter("environment", env)
            .Run();

        var pool = result.Stages[0].Pool;
        Assert.That(pool.VmImage, Is.EqualTo(expectedImage));
    }
}
```

## Testing Variables

### Test Variable Substitution

```csharp
[TestFixture]
public class VariablesTest
{
    [Test]
    public void Pipeline_SubstitutesVariableInStepName()
    {
        var result = new PipelineTester()
            .WithPipeline("variables-pipeline.yaml")
            .WithVariables(new Dictionary<string, object>
            {
                ["buildConfiguration"] = "Release",
                ["buildPlatform"] = "x64"
            })
            .Run();

        var step = result.Stages[0].Jobs[0].Steps[0];
        Assert.That(step.DisplayName, Does.Contain("Release"));
        Assert.That(step.DisplayName, Does.Contain("x64"));
    }

    [Test]
    public void Pipeline_UsesDefaultVariableValues()
    {
        var result = new PipelineTester()
            .WithPipeline("variables-pipeline.yaml")
            .Run();

        var step = result.Stages[0].Jobs[0].Steps[0];
        Assert.That(step.DisplayName, Does.Contain("Debug")); // default value
    }
}
```

### Complex Variable Objects

```csharp
[Test]
public void Pipeline_WithComplexVariables_SubstitutesCorrectly()
{
    var result = new PipelineTester()
        .WithPipeline("complex-variables.yaml")
        .WithVariables(new Dictionary<string, object>
        {
            ["buildSettings"] = new Dictionary<string, object>
            {
                ["config"] = "Release",
                ["platform"] = "x64"
            }
        })
        .Run();

    var step = result.Stages[0].Jobs[0].Steps[0];
    Assert.That(step.DisplayName, Does.Contain("Release"));
}
```

## Testing Conditional Steps

### Test Conditional Step Inclusion

```csharp
[TestFixture]
public class ConditionalStepsTest
{
    [Test]
    public void Pipeline_WhenRunSecurityTests_IncludesSecurityStage()
    {
        var result = new PipelineTester()
            .WithPipeline("conditional-steps.yaml")
            .WithParameter("runSecurityTests", true)
            .Run();

        var securitySteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .Where(s => s.DisplayName.Contains("Security"))
            .ToList();

        Assert.That(securitySteps, Is.Not.Empty);
    }

    [Test]
    public void Pipeline_WhenSkipSecurityTests_ExcludesSecurityStage()
    {
        var result = new PipelineTester()
            .WithPipeline("conditional-steps.yaml")
            .WithParameter("runSecurityTests", false)
            .Run();

        var securitySteps = result.Stages
            .SelectMany(s => s.Jobs)
            .SelectMany(j => j.Steps)
            .Where(s => s.DisplayName.Contains("Security"))
            .ToList();

        Assert.That(securitySteps, Is.Empty);
    }
}
```

### Test If-Else Conditionals

```csharp
[Test]
public void Pipeline_WithIfElse_ExecutesCorrectBranch()
{
    var resultDev = new PipelineTester()
        .WithPipeline("if-else-pipeline.yaml")
        .WithParameter("environment", "development")
        .Run();

    var resultProd = new PipelineTester()
        .WithPipeline("if-else-pipeline.yaml")
        .WithParameter("environment", "production")
        .Run();

    var devSteps = resultDev.Stages[0].Jobs[0].Steps;
    var prodSteps = resultProd.Stages[0].Jobs[0].Steps;

    // Dev should have debugging step
    Assert.That(devSteps, Has.Some.Property("DisplayName").Contain("Debug"));
    
    // Prod should NOT have debugging step but should have security scan
    Assert.That(prodSteps, Has.None.Property("DisplayName").Contain("Debug"));
    Assert.That(prodSteps, Has.Some.Property("DisplayName").Contain("Security"));
}
```

## Testing Multi-Stage Pipelines

### Test Stage Structure

```csharp
[TestFixture]
public class MultiStageTest
{
    [Test]
    public void Pipeline_HasBuildTestDeployStages()
    {
        var result = new PipelineTester()
            .WithPipeline("multi-stage-pipeline.yaml")
            .Run();

        Assert.That(result.Stages, Has.Length.EqualTo(3));
        Assert.That(result.Stages[0].Name, Is.EqualTo("Build"));
        Assert.That(result.Stages[1].Name, Is.EqualTo("Test"));
        Assert.That(result.Stages[2].Name, Is.EqualTo("Deploy"));
    }

    [Test]
    public void Pipeline_BuildStagePrecedesDeploy()
    {
        var result = new PipelineTester()
            .WithPipeline("multi-stage-pipeline.yaml")
            .Run();

        var buildStageIndex = Array.FindIndex(result.Stages, s => s.Name == "Build");
        var deployStageIndex = Array.FindIndex(result.Stages, s => s.Name == "Deploy");

        Assert.That(buildStageIndex, Is.LessThan(deployStageIndex));
    }
}
```

### Test Stage Dependencies

```csharp
[Test]
public void Pipeline_DeploymentRequiresBuildCompletion()
{
    var result = new PipelineTester()
        .WithPipeline("dependent-stages.yaml")
        .Run();

    var deployStage = result.Stages.First(s => s.Name == "Deploy");
    
    // Verify deployment stage has dependency on build
    Assert.That(deployStage, Has.Property("DependsOn").EqualTo("Build"));
}
```

## Testing Step Types

### Test Task Steps

```csharp
[TestFixture]
public class TaskStepTest
{
    [Test]
    public void Pipeline_ContainsVSBuildTask()
    {
        var result = new PipelineTester()
            .WithPipeline("task-steps.yaml")
            .Run();

        var taskSteps = result.Stages[0].Jobs[0].Steps
            .OfType<TaskStep>()
            .ToList();

        Assert.That(taskSteps, Is.Not.Empty);
        Assert.That(taskSteps, Has.Some.Property("TaskName").EqualTo("VSBuild"));
    }

    [Test]
    public void Pipeline_TaskHasCorrectInputs()
    {
        var result = new PipelineTester()
            .WithPipeline("task-steps.yaml")
            .Run();

        var buildTask = result.Stages[0].Jobs[0].Steps
            .OfType<TaskStep>()
            .First(t => t.TaskName == "DotNetCoreCLI");

        Assert.That(buildTask.Inputs, Contains.Key("command"));
        Assert.That(buildTask.Inputs["command"], Is.EqualTo("build"));
    }
}
```

### Test Script Steps

```csharp
[Test]
public void Pipeline_ContainsBothTaskAndScriptSteps()
{
    var result = new PipelineTester()
        .WithPipeline("mixed-steps.yaml")
        .Run();

    var steps = result.Stages[0].Jobs[0].Steps;
    var taskSteps = steps.OfType<TaskStep>();
    var scriptSteps = steps.OfType<ScriptStep>();

    Assert.That(taskSteps, Has.Length.GreaterThan(0));
    Assert.That(scriptSteps, Has.Length.GreaterThan(0));
}
```

## Testing Triggers

### Test Branch Triggers

```csharp
[TestFixture]
public class TriggersTest
{
    [Test]
    public void Pipeline_TriggersOnMainBranch()
    {
        var result = new PipelineTester()
            .WithPipeline("trigger-pipeline.yaml")
            .Run();

        var triggers = result.Triggers;
        Assert.That(triggers.Branches, Has.Some.EqualTo("main"));
    }

    [Test]
    public void Pipeline_TriggersOnMultipleBranches()
    {
        var result = new PipelineTester()
            .WithPipeline("trigger-pipeline.yaml")
            .Run();

        var triggers = result.Triggers;
        Assert.That(triggers.Branches, Has.Count.GreaterThan(1));
        Assert.That(triggers.Branches, Has.Some.EqualTo("main"));
        Assert.That(triggers.Branches, Has.Some.EqualTo("develop"));
    }
}
```

### Test Path Triggers

```csharp
[Test]
public void Pipeline_TriggersOnlyForPathChanges()
{
    var result = new PipelineTester()
        .WithPipeline("path-trigger-pipeline.yaml")
        .Run();

    var triggers = result.Triggers;
    Assert.That(triggers.Paths, Is.Not.Empty);
    Assert.That(triggers.Paths, Has.Some.EqualTo("src/**"));
}
```

## Testing NUnit Constraints

The `AdoPipelineTest.Nunit` package provides fluent assertions for pipeline testing.

### Basic Constraint Usage

```csharp
using AdoPipelineTest.Nunit;

[TestFixture]
public class PipelineConstraintsTest
{
    private PipelineTestResult _result;

    [SetUp]
    public void Setup()
    {
        _result = new PipelineTester()
            .WithPipeline("azure-pipelines.yaml")
            .Run();
    }

    [Test]
    public void Pipeline_HasExpectedStages()
    {
        Assert.That(_result, Is.HasStage("Build"));
        Assert.That(_result, Is.HasStage("Test"));
        Assert.That(_result, Is.HasStage("Deploy"));
    }

    [Test]
    public void Pipeline_StagesHaveCorrectJobs()
    {
        Assert.That(_result.Stages[0], Is.HasJob("Compile"));
        Assert.That(_result.Stages[0], Is.HasJob("Package"));
    }

    [Test]
    public void Pipeline_JobsHaveCorrectTasks()
    {
        var buildJob = _result.Stages[0].Jobs[0];
        
        Assert.That(buildJob, Is.HasTask("DotNetCoreCLI@2"));
        Assert.That(buildJob, Is.HasStep("Restore packages"));
    }
}
```

### Testing Dependencies

```csharp
[Test]
public void Pipeline_StagesHaveCorrectDependencies()
{
    var result = new PipelineTester()
        .WithPipeline("dependent-stages.yaml")
        .Run();

    var buildStage = result.Stages.First(s => s.Name == "Build");
    var deployStage = result.Stages.First(s => s.Name == "Deploy");

    Assert.That(buildStage, Is.DependsOn("Prep"));
    Assert.That(deployStage, Is.DependsOn("Build"));
    Assert.That(deployStage, Is.DependsOn("Test"));
}
```

### Testing Variables and Parameters

```csharp
[Test]
public void Pipeline_HasExpectedVariablesAndParameters()
{
    var result = new PipelineTester()
        .WithPipeline("configured-pipeline.yaml")
        .WithParameter("environment", "production")
        .Run();

    Assert.That(result, Is.HasVariable("buildConfiguration"));
    Assert.That(result, Is.HasParameter("environment"));
}
```

### Testing Triggers and Resources

```csharp
[Test]
public void Pipeline_HasTriggersAndResources()
{
    var result = new PipelineTester()
        .WithPipeline("full-pipeline.yaml")
        .Run();

    Assert.That(result.Triggers, Is.HasTrigger());
    Assert.That(result, Is.HasResource("repositories"));
    Assert.That(result, Is.HasResource("pipelines"));
}
```

### Combining Constraints

```csharp
[Test]
public void Pipeline_FullStructureValidation()
{
    var result = new PipelineTester()
        .WithPipeline("ci-pipeline.yaml")
        .Run();

    // Stages
    Assert.That(result, Is.HasStage("Build"));
    Assert.That(result, Is.HasStage("Deploy"));
    
    // Jobs within stages
    Assert.That(result.Stages[0], Is.HasJob("Compile"));
    Assert.That(result.Stages[0], Is.HasJob("Test"));
    
    // Steps within jobs
    var buildJob = result.Stages[0].Jobs[0];
    Assert.That(buildJob, Is.HasStep("Build solution"));
    Assert.That(buildJob, Is.HasTask("DotNetCoreCLI@2"));
    
    // Dependencies
    Assert.That(result.Stages[1], Is.DependsOn("Build"));
    
    // Configuration
    Assert.That(result, Is.HasVariable("buildConfiguration"));
    Assert.That(result, Is.HasParameter("environment"));
    Assert.That(result.Triggers, Is.HasTrigger());
}
```

## Testing Templates

### Test Template Resolution

```csharp
[TestFixture]
public class TemplateTest
{
    [Test]
    public void Pipeline_ResolvesExternalTemplate()
    {
        var result = new PipelineTester()
            .WithPipeline("uses-template.yaml")
            .WithTemplateRoot("templates/")
            .Run();

        // Verify steps from template are included
        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Is.Not.Empty);
        
        // Verify expected steps from template
        Assert.That(steps, Has.Some.Property("DisplayName").Contain("Build"));
    }
}
```

### Test Template with Parameters

```csharp
[Test]
public void Pipeline_PassesParametersToTemplate()
{
    var result = new PipelineTester()
        .WithPipeline("template-with-params.yaml")
        .WithParameter("buildConfiguration", "Release")
        .WithTemplateRoot("templates/")
        .Run();

    var step = result.Stages[0].Jobs[0].Steps[0];
    Assert.That(step.DisplayName, Does.Contain("Release"));
}
```

## Advanced Scenarios

### Test Complex Conditional Logic

```csharp
[TestFixture]
public class ComplexConditionalTest
{
    [Test]
    public void Pipeline_WithMultipleConditions_EvaluatesCorrectly()
    {
        var result = new PipelineTester()
            .WithPipeline("complex-conditions.yaml")
            .WithParameter("environment", "production")
            .WithParameter("runTests", true)
            .WithVariables(new Dictionary<string, object>
            {
                ["isMainBranch"] = true
            })
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        // Verify correct steps are included based on conditions
        Assert.That(steps, Has.Some.Property("DisplayName").Contain("Prod Deployment"));
        Assert.That(steps, Has.Some.Property("DisplayName").Contain("Tests"));
    }
}
```

### Test Job Dependencies

```csharp
[Test]
public void Pipeline_JobsHaveDependencies()
{
    var result = new PipelineTester()
        .WithPipeline("dependent-jobs.yaml")
        .Run();

    var jobs = result.Stages[0].Jobs;
    var testJob = jobs.First(j => j.Name == "Test");

    Assert.That(testJob, Has.Property("DependsOn").EqualTo("Build"));
}
```

### Test Large Pipeline with Many Stages

```csharp
[Test]
public void Pipeline_LargePipelineLoads_Successfully()
{
    var result = new PipelineTester()
        .WithPipeline("large-complex-pipeline.yaml")
        .WithParameter("deployRegion", "us-east")
        .WithVariables(new Dictionary<string, object>
        {
            ["buildNumber"] = "123",
            ["releaseVersion"] = "2.0.0"
        })
        .Run();

    Assert.That(result.Stages, Has.Length.GreaterThan(5));
    Assert.That(result.IsValid, Is.True);
}
```

### Test Error Handling

```csharp
[Test]
public void Pipeline_WithInvalidYaml_ReportsError()
{
    var result = new PipelineTester()
        .WithPipeline("invalid-pipeline.yaml")
        .Run();

    Assert.That(result.IsValid, Is.False);
    Assert.That(result.ValidationErrors, Is.Not.Empty);
}
```

### Snapshot Testing Pattern

```csharp
[Test]
public void Pipeline_StructureMatchesSnapshot()
{
    var result = new PipelineTester()
        .WithPipeline("snapshot-pipeline.yaml")
        .Run();

    var snapshot = new
    {
        StageCount = result.Stages.Length,
        StageNames = result.Stages.Select(s => s.Name).ToArray(),
        FirstStageJobCount = result.Stages[0].Jobs.Count(),
    };

    // Use Verify library or similar for snapshot testing
    Verify(snapshot);
}
```

## Tips & Best Practices

### Use Descriptive Test Names
```csharp
// Good
[Test]
public void Pipeline_WhenEnvironmentIsProduction_IncludesSecurityScanStage()

// Avoid
[Test]
public void Test1()
```

### Test One Concern Per Test
```csharp
// Good - tests one specific behavior
[Test]
public void Pipeline_WithSecurityEnabled_IncludesScanStep()

// Avoid - tests multiple concerns
[Test]
public void Pipeline_WithSecurityAndNotifications_WorksCorrectly()
```

### Use TestCase for Parameterized Tests
```csharp
// Good - tests multiple scenarios clearly
[TestCase("dev", "ubuntu-latest")]
[TestCase("prod", "windows-latest")]
public void Pipeline_SelectsPoolByEnvironment(string env, string expected)

// Avoid - multiple separate test methods
[Test]
public void Pipeline_DevEnvironmentUsesUbuntu()
[Test]
public void Pipeline_ProdEnvironmentUsesWindows()
```

### Group Related Tests
```csharp
// Organize tests by concern
[TestFixture]
public class ParameterTests { }

[TestFixture]
public class ConditionalTests { }

[TestFixture]
public class TemplateTests { }
```

