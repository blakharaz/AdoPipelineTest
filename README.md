# AdoPipelineTest

A .NET testing library for Azure DevOps YAML pipelines. Unit test your ADO pipeline configurations using a simple, fluent API.

## Features

- **Parse & Evaluate** - Converts YAML pipelines into testable domain models
- **Test Parameters** - Verify pipeline behavior with different parameter values  
- **Test Expressions** - Validate template expressions and conditional logic
- **Template Support** - Automatically resolves referenced template files
- **NUnit Integration** - Custom constraints for fluent assertions
- **MSTest Integration** - Extension methods for fluent assertions

## Quick Start

### Installation

```bash
dotnet add package AdoPipelineTest.Lib
dotnet add package AdoPipelineTest.Nunit
```

### Simple Example

```csharp
using AdoPipelineTest.Lib;
using NUnit.Framework;

[Test]
public void MyPipeline_HasCorrectStructure()
{
    var result = new PipelineTester()
        .WithPipeline("azure-pipelines.yaml")
        .WithParameter("environment", "production")
        .Run();

    Assert.That(result.Stages, Has.Length.GreaterThan(0));
    Assert.That(result.Stages[0].Name, Is.EqualTo("Deploy"));
}
```
### NUnit Constraints

The `AdoPipelineTest.Nunit` package provides custom NUnit constraints for fluent pipeline assertions:

```csharp
using AdoPipelineTest.Nunit;

[Test]
public void Pipeline_HasCorrectStructure()
{
    var result = new PipelineTester()
        .WithPipeline("azure-pipelines.yaml")
        .Run();

    // Stage assertions (match by Name or DisplayName)
    Assert.That(result, Is.HasStage("Build"));
    Assert.That(result, Is.HasStage("Build Stage"));
    
    // Job assertions (match by Name or DisplayName)
    Assert.That(result.Stages[0], Is.HasJob("Compile"));
    
    // Step assertions (match by DisplayName)
    Assert.That(result.Stages[0].Jobs[0], Is.HasStep("Build Task"));
    
    // Task assertions (match by TaskName)
    Assert.That(result.Stages[0].Jobs[0], Is.HasTask("DotNetCoreCLI@2"));
    
    // Dependency assertions
    Assert.That(result.Stages[0], Is.DependsOn("Prep"));
    Assert.That(result.Stages[0].Jobs[0], Is.DependsOn("Setup"));
    
    // Variable assertions
    Assert.That(result, Is.HasVariable("buildConfiguration"));
    
    // Parameter assertions
    Assert.That(result, Is.HasParameter("environment"));
    
    // Trigger assertions
    Assert.That(result.Triggers, Is.HasTrigger());
    
    // Resource assertions
    Assert.That(result, Is.HasResource("repositories"));
}
```

| Constraint | Target Type | Description |
|------------|-------------|-------------|
| `HasStage(name)` | `PipelineTestResult` | Asserts pipeline has a stage with given name or display name |
| `HasJob(name)` | `PipelineStage` | Asserts stage has a job with given name or display name |
| `HasStep(displayName)` | `PipelineJob` | Asserts job has a step with given display name |
| `HasTask(taskName)` | `PipelineJob` | Asserts job has a task with given task name |
| `HasVariable(name)` | `PipelineTestResult` | Asserts pipeline has a variable with given name |
| `HasParameter(name)` | `PipelineTestResult` | Asserts pipeline has a parameter with given name |
| `HasTrigger()` | `PipelineTriggers` | Asserts pipeline has triggers configured |
| `DependsOn(name)` | `PipelineStage`, `PipelineJob` | Asserts stage/job depends on another |
| `HasResource(type)` | `PipelineTestResult` | Asserts pipeline has a resource of given type |

### MSTest Extension Methods

The `AdoPipelineTest.Mstest` package provides extension methods on `PipelineTestResult` and `PipelineStage` for fluent assertions:

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdoPipelineTest.Mstest;

[TestClass]
public class PipelineTests
{
    [TestMethod]
    public void Pipeline_HasCorrectStructure()
    {
        var result = new PipelineTester()
            .WithPipeline("azure-pipelines.yaml")
            .Run();

        result.HasTrigger();
        result.HasVmImage("ubuntu-latest");
        result.HasStageCount(1);
        result.HasStage("Build");
        result.HasJob("Build", "Compile");
        result.HasTask("DotNetCoreCLI@2");
        result.HasStep("Build Task");
        result.HasVariable("buildConfiguration");
        result.HasVariable("buildConfiguration", "Release");
        result.HasParameter("environment");
        result.HasScriptStep("dotnet build");
        result.TriggersIncludeBranch("main");
    }

    [TestMethod]
    public void Pipeline_JobAssertions()
    {
        var result = new PipelineTester()
            .WithPipeline("azure-pipelines.yaml")
            .Run();

        var stage = result.Stages[0];
        stage.HasJob("Compile");
    }
}
```

| Method | Target | Description |
|--------|--------|-------------|
| `HasStage(name)` | `PipelineTestResult` | Asserts pipeline has a stage with given display name |
| `HasStageCount(count)` | `PipelineTestResult` | Asserts exact stage count |
| `HasJob(stageName, jobName)` | `PipelineTestResult` | Asserts a job exists in a named stage |
| `HasJob(jobName)` | `PipelineStage` | Asserts a job exists in a stage |
| `HasStep(displayName)` | `PipelineTestResult` | Asserts a step with given display name exists |
| `HasStep(predicate)` | `PipelineTestResult` | Asserts a step matching a predicate exists |
| `HasTask(taskName)` | `PipelineTestResult` | Asserts a TaskStep with given task name exists |
| `HasVariable(name)` | `PipelineTestResult` | Asserts a variable exists |
| `HasVariable(name, value)` | `PipelineTestResult` | Asserts a variable exists with a specific value |
| `HasParameter(name)` | `PipelineTestResult` | Asserts a parameter exists |
| `HasTrigger()` | `PipelineTestResult` | Asserts triggers are configured |
| `TriggersIncludeBranch(name)` | `PipelineTestResult` | Asserts a specific branch is in triggers |
| `HasVmImage(image)` | `PipelineTestResult` | Asserts the VM image |
| `HasScriptStep(pattern)` | `PipelineTestResult` | Asserts a ScriptStep containing a pattern exists |

All methods accept an optional `because` parameter for custom failure messages.

## How It Works

AdoPipelineTest uses a **two-phase approach**:

1. **Parsing** - YAML files are converted into an Abstract Syntax Tree (AST)
2. **Evaluation** - The AST is evaluated with your parameters/variables to produce testable domain models

See [ARCHITECTURE.md](ARCHITECTURE.md) for detailed information on how the library works internally.

## Documentation

- **[EXAMPLES.md](EXAMPLES.md)** - Comprehensive examples for all testing scenarios
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Technical deep-dive into parsing and evaluation phases
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - Guide for contributors

## API Quick Reference

```csharp
var result = new PipelineTester()
    .WithPipeline("path/to/pipeline.yaml")
    .WithParameter("name", value)
    .WithVariables(new Dictionary<string, object> { ... })
    .WithTemplateRoot("templates/")
    .Run();

// Access evaluated pipeline
result.Stages              // PipelineStage[]
result.Jobs                // PipelineJob[] (steps-only)
result.Steps               // PipelineStep[] (steps-only)
result.Triggers            // PipelineTriggers
result.IsValid             // bool
result.ValidationErrors    // string[]
```

## Contributing

Interested in contributing to AdoPipelineTest? We'd love your help! Whether you want to:
- Add support for new YAML pipeline elements
- Improve expression evaluation
- Add new NUnit constraints
- Fix bugs or improve documentation

Please see [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Development setup and build instructions
- Code style guidelines and naming conventions
- Project organization and file structure
- Detailed workflows for common tasks (new YAML elements, constraints, tests)
- Pull request process and code review checklist

## Technologies

- **.NET 10.0** / **C# 14**
- **YamlDotNet 16.x** - YAML parsing
- **Sprache 2.3.1** - Expression parsing
- **NUnit 4.x** - Testing framework

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Resources

- [Azure DevOps Pipelines Documentation](https://docs.microsoft.com/en-us/azure/devops/pipelines/)
- [NUnit Documentation](https://docs.nunit.org/)
- [YamlDotNet Documentation](https://github.com/aaubry/YamlDotNet)
- [Sprache Documentation](https://github.com/sprache/Sprache)

