# GitHub Copilot Instructions for AdoPipelineTest

## Project Overview
This is a .NET 10 testing library for Azure DevOps YAML pipelines. It parses ADO pipeline YAML, resolves templates, evaluates expressions, and enables unit testing of pipeline configurations.

## How It Works
The library uses a **two-step approach**:

1. **Parsing Phase** (YAML → AST): `PipelineParser` reads YAML files and creates an Abstract Syntax Tree (AST) with `*Element` nodes (e.g., `StepsElement`, `JobElement`). This preserves the raw structure without evaluation.

2. **Evaluation Phase** (AST → Model): `PipelineEvaluator` traverses the AST, evaluates template expressions (`${{ }}` syntax), resolves parameters and variables, and produces domain model objects (e.g., `PipelineStep`, `PipelineJob`). The result is a fully evaluated `Pipeline` ready for unit testing assertions.

## Architecture
- **AdoPipelineTest.Lib**: Core library with parsing (YamlDotNet), evaluation, and model classes
- **AdoPipelineTest.Nunit**: NUnit constraints for pipeline assertions
- **AdoPipelineTest.Samples**: Example tests demonstrating library usage
- **AdoPipelineTest.UnitTests**: Library unit tests

## Code Style & Conventions
- Use C# 14 with `nullable` enabled and `ImplicitUsings`
- Prefer `required` properties over constructor injection for DTOs
- Use collection expressions (`[]`) for empty collections
- Use file-scoped namespaces
- Test classes follow `*Test.cs` naming pattern
- YAML test data files go in `test_data/` directories

## Key Patterns
- **Fluent Builder**: `PipelineTester` uses fluent API (`.WithPipeline().WithParameter().Run()`)
- **Parser Pattern**: Each YAML element has dedicated parser (e.g., `TriggerParser`, `StepsParser`)
- **AST Model**: Parsing creates `*Element` AST nodes, evaluation produces `Pipeline*` domain models
- **NUnit Constraints**: Custom constraints inherit from NUnit's `Constraint` base class

## Azure DevOps Pipeline Knowledge
- Pipelines can have: triggers, pool, parameters, variables, stages/jobs/steps
- Template expressions use `${{ }}` syntax with `if/else` conditionals
- Parameters support types: string, boolean, number, object
- Variables can be simple key-value or complex with `name`/`value` properties

## Testing Conventions
- Use NUnit 4.x attributes and assertions
- Copy YAML test files to output with `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>`
- Sample tests demonstrate real-world usage scenarios

## For Contributors
See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidance on:
- Development setup and build process
- Code style guidelines and naming conventions
- Workflows for adding new features
- Pull request process and code review guidelines

