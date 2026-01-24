# AdoPipelineTest

A .NET testing library for Azure DevOps YAML pipelines. Unit test your ADO pipeline configurations using a simple, fluent API.

## Features

- **Parse & Evaluate** - Converts YAML pipelines into testable domain models
- **Test Parameters** - Verify pipeline behavior with different parameter values  
- **Test Expressions** - Validate template expressions and conditional logic
- **Template Support** - Automatically resolves referenced template files
- **NUnit Integration** - Custom constraints for fluent assertions

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
- **NUnit 4.x** - Testing framework

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Resources

- [Azure DevOps Pipelines Documentation](https://docs.microsoft.com/en-us/azure/devops/pipelines/)
- [NUnit Documentation](https://docs.nunit.org/)
- [YamlDotNet Documentation](https://github.com/aaubry/YamlDotNet)


