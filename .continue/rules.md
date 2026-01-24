# Continue AI Rules - AdoPipelineTest

## Project Overview

AdoPipelineTest is a .NET 10 testing library for Azure DevOps YAML pipelines. It parses ADO pipeline YAML, resolves templates, evaluates expressions, and enables unit testing of pipeline configurations using a clean, fluent API.

## How It Works

The library follows a **two-phase processing approach**:

### Phase 1: Parsing (YAML → AST)
Raw YAML files are parsed into an Abstract Syntax Tree with `*Element` nodes. This preserves the raw structure without evaluation.

### Phase 2: Evaluation (AST → Model)
The AST is evaluated to produce domain models:
- Resolves template expressions (`${{ }}` syntax)
- Substitutes parameters and variables
- Evaluates conditionals (`if`/`else`)
- Returns fully instantiated `Pipeline*` objects ready for assertions

This separation enables testing of conditional logic and parameter behavior.

## Main Entry Point

```csharp
var result = new PipelineTester()
    .WithPipeline("path/to/pipeline.yaml")
    .WithParameter("environment", "production")
    .WithVariables(new Dictionary<string, object> { ["buildConfig"] = "Release" })
    .WithTemplateRoot("templates/")
    .Run();
```

The `.Run()` method returns `PipelineTestResult` containing fully evaluated stages, jobs, and steps.

## Technology Stack

| Technology | Purpose | Version |
|-----------|---------|---------|
| .NET | Runtime | 10.0 |
| C# | Language | 14 with nullable enabled |
| YamlDotNet | YAML parsing | 16.x |
| NUnit | Testing framework | 4.x |

## Code Style & Conventions

- **Nullable**: Always enabled; use `?` for nullable types
- **Namespaces**: File-scoped (`namespace AdoPipelineTest.Lib;`)
- **Implicit usings**: Enabled in all projects
- **Properties**: Use `required` and `init` for immutable DTOs
- **Collections**: Use collection expressions (`[]`) for empty collections
- **LINQ**: Prefer method chains with expressions

### Naming Patterns

| Type | Pattern | Example |
|------|---------|---------|
| AST Nodes | `*Element` | `StepsElement`, `JobElement` |
| Parsers | `*Parser` | `TriggerParser`, `StepsParser` |
| Domain Models | `Pipeline*` | `PipelineStep`, `PipelineJob` |
| Test Files | `*Test` | `PipelineParserTest` |
| Constraints | `*Constraint` | `VmImageConstraint` |

## Architecture Layers

### Parsing Layer (`Parsing/`)
- **PipelineParser.cs** - Orchestrator that coordinates all parsers
- **`*Parser.cs`** - Specialized parsers for YAML elements
- **`Ast/*Element.cs`** - AST node definitions
- **YamlExtensions.cs** - Helpers for safe YAML navigation

### Evaluation Layer (`Evaluation/`)
- **PipelineEvaluator.cs** - AST → Model transformation
- **ExpressionEvaluator.cs** - Evaluates `${{ }}` expressions
- **ParameterEvaluator.cs** - Resolves parameter references
- **TemplateResolver.cs** - Resolves template file references

### Model Layer (`Model/`)
- **Pipeline*** classes - Fully evaluated domain objects
- **Steps/** - Step type hierarchy (TaskStep, ScriptStep, etc.)
- Domain-specific models (PipelineStage, PipelineJob, etc.)

### Public API
- **PipelineTester.cs** - Fluent builder (user-facing API)
- **PipelineTestResult.cs** - Result container with evaluated pipeline

## Key Patterns to Follow

### Pattern: Parser Implementation
```csharp
internal class MyParser
{
    public MyElement Parse(YamlMappingNode node)
    {
        // Use TryGetValue for safe access
        if (node.Children.TryGetValue("property", out var value))
        {
            // Parse child nodes recursively
        }
        return new MyElement { /* ... */ };
    }
}
```

### Pattern: Domain Model
```csharp
public class PipelineMyModel
{
    public required string Name { get; init; }
    public required List<string> Values { get; init; }
}
```

### Pattern: Evaluation
```csharp
private PipelineMyModel EvaluateMyElement(MyElement element)
{
    // Transform AST to domain model
    // Evaluate expressions and resolve references
    return new PipelineMyModel 
    { 
        Name = EvaluateExpression(element.Name),
        Values = element.Values ?? []
    };
}
```

## Do's ✅

- Generate **NUnit 4.x** compatible test code
- Use `Assert.That()` with constraints (fluent style)
- Create **immutable models** with `required` and `init`
- Use **file-scoped namespaces**
- Include `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` for test YAML files
- Follow **existing parser patterns** when adding support for new YAML elements
- Use `YamlMappingNode.Children.TryGetValue()` for **safe YAML access**
- Implement parsing and evaluation as **separate concerns**
- Write **unit tests** in `AdoPipelineTest.UnitTests/` with matching folder structure
- Add **test data files** in `test_data/` subdirectories

## Don'ts ❌

- Don't use **xUnit** or **MSTest** syntax
- Don't create **mutable models** with setters
- Don't hardcode **file paths** (use relative paths from test output)
- Don't mix **parsing and evaluation** logic
- Don't skip **nullable analysis** (consider `?` for optional properties)
- Don't forget to **register new parsers** in `PipelineParser`
- Don't make **parser classes public** (keep internal)
- Don't omit **test data files** from version control

## Common Development Tasks

### Adding Support for New YAML Element

1. **Create AST node** in `Parsing/Ast/MyElement.cs`
2. **Create parser** in `Parsing/MyParser.cs`
3. **Register in PipelineParser** (in appropriate orchestration method)
4. **Create domain model** in `Model/PipelineMyElement.cs`
5. **Add evaluation logic** in `PipelineEvaluator.cs`
6. **Add unit tests** in `AdoPipelineTest.UnitTests/Parsing/MyParserTest.cs`
7. **Add test data** in `AdoPipelineTest.UnitTests/test_data/my-element/`

### Adding New NUnit Constraint

1. Create constraint in `AdoPipelineTest.Nunit/Constraints/MyConstraint.cs`
2. Inherit from `Constraint` and implement `ApplyTo<T>()`
3. Add factory method in `Is.cs`
4. Create sample test in `AdoPipelineTest.Samples/`

### Testing Conditional Logic

Use `${{ if }}` syntax in test YAML files:

```yaml
steps:
  - ${{ if eq(parameters.environment, 'prod') }}:
      - task: SecurityScan@1
  - ${{ else }}:
      - script: echo "Dev mode"
```

Test both branches:
```csharp
[TestCase("prod", true)]
[TestCase("dev", false)]
public void Pipeline_IncludesSecurityScanForProd(string env, bool shouldInclude)
{
    var result = new PipelineTester()
        .WithPipeline("conditional.yaml")
        .WithParameter("environment", env)
        .Run();
    
    var hasScan = result.Stages[0].Jobs[0].Steps
        .Any(s => s.DisplayName.Contains("Security"));
    
    Assert.That(hasScan, Is.EqualTo(shouldInclude));
}
```

## Project Organization Quick Reference

```
AdoPipelineTest.Lib/
  Parsing/              ← Phase 1: YAML parsing
    Ast/               ← AST node definitions
    *Parser.cs         ← Specialized parsers
    PipelineParser.cs  ← Orchestrator
  
  Evaluation/           ← Phase 2: AST evaluation
    PipelineEvaluator.cs    ← Main evaluator
    ExpressionEvaluator.cs  ← Expression resolution
  
  Model/               ← Domain models
    Pipeline*.cs       ← Model classes
    Steps/            ← Step types

AdoPipelineTest.Nunit/
  Constraints/         ← Custom NUnit constraints
  Is.cs               ← Constraint factory

AdoPipelineTest.UnitTests/
  Parsing/            ← Parser tests
  Evaluation/         ← Evaluator tests
  test_data/          ← YAML fixtures
```

## Testing Best Practices

- **One concern per test** - Test one behavior at a time
- **Descriptive names** - Use pattern: `Method_Condition_ExpectedResult`
- **Use TestCase** - For parameterized tests with multiple scenarios
- **Group by TestFixture** - Organize related tests together
- **Use YAML fixtures** - Store complex test data in YAML files with `<CopyToOutputDirectory>` set

## Debugging Tips

- Check that YAML files are copied to output directory (`<CopyToOutputDirectory>`)
- Verify template paths are relative to pipeline location
- Use `.WithTemplateRoot()` to specify template base directory
- Check expression syntax: `${{ expression }}` for compile-time, `$[ expression ]` for runtime
- Ensure parameter types match (string, boolean, number, object)

## References

- **Architecture Details**: See `ARCHITECTURE.md`
- **Code Examples**: See `EXAMPLES.md`
- **Contributing Guide**: See `CONTRIBUTING.md` for development setup, code guidelines, and contribution workflows
- **Azure DevOps Pipelines**: https://docs.microsoft.com/en-us/azure/devops/pipelines/
- **YamlDotNet**: https://github.com/aaubry/YamlDotNet

