# AdoPipelineTest - AI Context

## Purpose
Unit testing library for Azure DevOps (ADO) YAML pipelines. Enables developers to:
- Parse and validate ADO pipeline YAML syntax
- Evaluate template expressions with custom parameters
- Assert pipeline configuration using NUnit constraints
- Test conditional step insertion logic

## How It Works
The library follows a **two-step processing model**:

1. **Parse Phase** (YAML → AST): Raw YAML files are parsed into an Abstract Syntax Tree with element nodes (`*Element` classes). This preserves the original structure, including unresolved template expressions and conditional blocks.

2. **Evaluate Phase** (AST → Model): The AST is traversed and evaluated to produce concrete domain models (`Pipeline*` classes). This step:
   - Resolves template expressions (`${{ }}` syntax)
   - Substitutes parameters and variables
   - Evaluates conditionals (`if`/`else`)
   - Produces a fully instantiated `PipelineTestResult` ready for assertions

This separation enables testing of conditional logic and parameter behavior.

## Technology Stack
- .NET 10.0 / C# 14
- YamlDotNet for YAML parsing
- NUnit 4 for testing
- GitHub Actions for CI

## Domain Terminology
| Term | Meaning |
|------|---------|
| Pipeline | Complete ADO YAML file defining CI/CD |
| Stage | Top-level grouping (contains jobs) |
| Job | Execution unit (contains steps) |
| Step | Individual task or script |
| Template Expression | `${{ }}` syntax for compile-time evaluation |
| Runtime Expression | `$[ ]` syntax for runtime evaluation |
| Parameter | Compile-time input with type validation |
| Variable | Runtime configuration value |

## Key Classes
- `PipelineTester`: Main API for loading and evaluating pipelines
- `PipelineTestResult`: Evaluation result with stages, jobs, steps
- `PipelineParser`: Orchestrates YAML parsing
- `TemplateResolver`: Resolves template file references
- `ExpressionEvaluator`: Evaluates `${{ }}` expressions
- `PipelineStep` / `TaskStep` / `ScriptStep`: Step model hierarchy

## Test Data Location
- Unit test YAML: `AdoPipelineTest.UnitTests/test_data/`
- Sample pipelines: `AdoPipelineTest.Samples/Nunit/`

## Contributing
For developers interested in contributing, see [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Development environment setup
- Code style and naming conventions
- Architecture and project organization
- Workflows for common tasks (adding features, constraints, etc.)

