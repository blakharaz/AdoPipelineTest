# JetBrains AI Assistant Rules for AdoPipelineTest

## Project Context
A .NET testing library for Azure DevOps YAML pipelines with NUnit integration.

**Target Framework**: .NET 10.0, C# 14, Nullable enabled

## Processing Model
The library processes pipelines in **two distinct phases**:

1. **Parsing** (YAML → AST): `PipelineParser` orchestrates specialized parsers (`TriggerParser`, `VariablesParser`, `StepsParser`, etc.) to convert YAML input into an Abstract Syntax Tree. AST nodes are `*Element` classes that represent the raw, unevaluated pipeline structure.

2. **Evaluation** (AST → Model): `PipelineEvaluator` traverses the AST and transforms it into domain models (`Pipeline`, `PipelineStage`, `PipelineJob`, `PipelineStep`, etc.). During evaluation:
   - Template expressions (`${{ }}`) are parsed and evaluated
   - Parameters are substituted with provided values
   - Variables are resolved and injected
   - Conditional blocks are evaluated for inclusion/exclusion
   - The result is a `PipelineTestResult` ready for unit test assertions

This separation of concerns allows for robust testing of ADO pipeline behavior under different parameter and variable conditions.

## Solution Structure
| Project | Purpose |
|---------|---------|
| AdoPipelineTest.Lib | Core parsing & evaluation logic |
| AdoPipelineTest.Nunit | NUnit constraint extensions |
| AdoPipelineTest.Samples | Usage examples & integration tests |
| AdoPipelineTest.UnitTests | Unit tests for the library |

## Coding Guidelines
- Use `init` accessors and `required` modifier for model properties
- Prefer LINQ expressions for collection transformations
- Use pattern matching with `is` and `switch` expressions
- Internal classes for parsers, public for models and API
- Use `InternalsVisibleTo` for test access to internal members

## Key Dependencies
- **YamlDotNet 16.x**: YAML parsing via `YamlStream`, `YamlMappingNode`, `YamlSequenceNode`
- **NUnit 4.x**: Test framework with custom constraints

## ADO Pipeline Specifics
When generating or modifying code related to ADO pipelines:
- Support all three pipeline structures: steps-only, jobs with steps, stages with jobs
- Handle template expressions: `${{ if }}`, `${{ else }}`, `${{ parameters.x }}`
- Recognize task syntax: `task: TaskName@Version` with `inputs:` block
- Support both simple triggers (`trigger: [main]`) and complex trigger objects

## File Organization
- Parsers: `AdoPipelineTest.Lib/Parsing/*.cs`
- AST nodes: `AdoPipelineTest.Lib/Parsing/Ast/*.cs`  
- Domain models: `AdoPipelineTest.Lib/Model/*.cs`
- Step types: `AdoPipelineTest.Lib/Model/Steps/*.cs`
- Evaluators: `AdoPipelineTest.Lib/Evaluation/*.cs`

## For Contributors
When assisting with contributions, reference [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Detailed development setup instructions
- Complete code style guidelines
- Step-by-step workflows for adding features
- Pull request and code review process

