# Contributing to AdoPipelineTest

Thank you for your interest in contributing! This document provides guidelines and workflows for developers who want to contribute to the AdoPipelineTest library.

## Development Setup

### Prerequisites
- .NET 10.0 SDK
- Git
- Your preferred C# IDE (Visual Studio, Rider, VS Code)

### Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/AdoPipelineTest.git
   cd AdoPipelineTest
   ```

2. **Build the solution**
   ```bash
   dotnet build
   ```

3. **Run all tests**
   ```bash
   dotnet test
   ```

4. **Run tests for a specific project**
   ```bash
   dotnet test AdoPipelineTest.UnitTests
   dotnet test AdoPipelineTest.Samples
   ```

## Code Style Guidelines

### Language Standards
- **C# 14** with nullable reference types enabled (`<Nullable>enable</Nullable>`)
- **File-scoped namespaces** - Use `namespace AdoPipelineTest.Lib;` not `namespace { }`
- **Implicit usings** - Enabled in all projects via `<ImplicitUsings>enable</ImplicitUsings>`

### Naming Conventions
- **Test files**: `*Test.cs` (e.g., `PipelineParserTest.cs`)
- **AST nodes**: `*Element.cs` (e.g., `StepsElement`, `JobElement`)
- **Domain models**: `Pipeline*` prefix (e.g., `PipelineStep`, `PipelineJob`)
- **Parsers**: `*Parser.cs` (e.g., `TriggerParser`, `VariablesParser`)
- **Evaluators**: `*Evaluator.cs` (e.g., `ExpressionEvaluator`)

### Property and Class Design
- **DTOs and Models**: Use `required` properties with `init` accessors
  ```csharp
  public class PipelineStep
  {
      public required string DisplayName { get; init; }
      public required string Type { get; init; }
  }
  ```
- **Collections**: Use collection expressions for empty collections
  ```csharp
  var emptySteps = PipelineStep[];  // Prefer over `new List<PipelineStep>()`
  ```
- **LINQ**: Prefer expression syntax for transformations
  ```csharp
  var filtered = items
      .Where(x => x.IsValid)
      .Select(x => x.Name)
      .ToList();
  ```

### Visibility
- **Parsers and internal logic**: `internal` by default
- **Public API**: Only expose `PipelineTester`, `PipelineTestResult`, and domain models
- **Use `InternalsVisibleTo`**: For test access to internal members
  ```csharp
  [assembly: InternalsVisibleTo("AdoPipelineTest.UnitTests")]
  ```

## Project Organization

Understanding the project structure is crucial for contributions:

```
AdoPipelineTest.Lib/
├── Parsing/
│   ├── PipelineParser.cs          # Main orchestrator - coordinates all parsers
│   ├── TriggerParser.cs           # Parses trigger configurations
│   ├── VariablesParser.cs         # Parses variables section
│   ├── ParametersParser.cs        # Parses parameters section
│   ├── PoolParser.cs              # Parses pool configuration
│   ├── StepsParser.cs             # Parses steps (all formats)
│   ├── ExpressionParser.cs        # Parses template expression syntax
│   ├── TemplateExpressionParser.cs # Parses ${{ }} expressions
│   ├── Ast/
│   │   ├── StepsElement.cs        # AST node for steps
│   │   ├── JobElement.cs          # AST node for jobs
│   │   ├── StageElement.cs        # AST node for stages
│   │   └── ...other AST nodes
│   └── Utils/
│       └── YamlExtensions.cs      # Helper methods for YamlDotNet
│
├── Evaluation/
│   ├── PipelineEvaluator.cs       # Main evaluator - orchestrates evaluation
│   ├── ExpressionEvaluator.cs     # Evaluates ${{ }} expressions
│   ├── ParameterEvaluator.cs      # Resolves parameters
│   ├── TemplateResolver.cs        # Resolves template files
│   └── ...other evaluators
│
├── Model/
│   ├── Pipeline.cs                # Top-level domain model
│   ├── PipelineStage.cs           # Stage model
│   ├── PipelineJob.cs             # Job model
│   ├── PipelineStep.cs            # Base step model
│   ├── PipelineVariable.cs        # Variable model
│   ├── PipelineParameter.cs       # Parameter model
│   ├── PipelineTriggers.cs        # Triggers model
│   ├── PipelineAgentPool.cs       # Pool configuration model
│   ├── Steps/
│   │   ├── TaskStep.cs            # Task step (task: X@Y)
│   │   ├── ScriptStep.cs          # Script step (script: or bash:)
│   │   └── ...other step types
│   └── InvalidPipelineException.cs # Exception type
│
├── PipelineTester.cs              # Main public API (fluent builder)
└── PipelineTestResult.cs          # Result object returned by .Run()
```

**Key Principle**: Separation of concerns
- **Parsing**: Reads YAML, creates AST with `*Element` nodes
- **Evaluation**: Transforms AST to domain models, resolves expressions
- **Model**: Fully evaluated, immutable domain objects

## Common Contribution Workflows

### Adding Support for a New YAML Element

**Example: Supporting a new "resources" section**

1. **Create an AST node** in `Parsing/Ast/ResourcesElement.cs`:
   ```csharp
   namespace AdoPipelineTest.Lib.Parsing.Ast;

   public class ResourcesElement
   {
       public required Dictionary<string, object> Data { get; init; }
   }
   ```

2. **Create a parser** in `Parsing/ResourcesParser.cs`:
   ```csharp
   namespace AdoPipelineTest.Lib.Parsing;

   internal class ResourcesParser
   {
       public ResourcesElement Parse(YamlMappingNode node)
       {
           var data = new Dictionary<string, object>();
           // Parse node and populate data
           return new ResourcesElement { Data = data };
       }
   }
   ```

3. **Integrate into PipelineParser.cs**:
   ```csharp
   // In PipelineParser.ParsePipeline()
   var resourcesNode = root.Children.FirstOrDefault(n => /* find "resources" */);
   var resources = resourcesNode != null 
       ? new ResourcesParser().Parse(resourcesNode) 
       : null;
   ```

4. **Create domain model** in `Model/PipelineResources.cs`:
   ```csharp
   namespace AdoPipelineTest.Lib.Model;

   public class PipelineResources
   {
       public required Dictionary<string, object> Data { get; init; }
   }
   ```

5. **Add evaluation logic** in `Evaluation/PipelineEvaluator.cs`:
   ```csharp
   private PipelineResources EvaluateResources(ResourcesElement element)
   {
       return new PipelineResources
       {
           Data = element.Data // or apply transformations as needed
       };
   }
   ```

6. **Add unit tests** in `AdoPipelineTest.UnitTests/Parsing/ResourcesParserTest.cs`:
   ```csharp
   namespace AdoPipelineTest.UnitTests.Parsing;

   [TestFixture]
   public class ResourcesParserTest
   {
       [Test]
       public void Parse_WithValidResources_ReturnsElement()
       {
           var yaml = """
               resources:
                 repositories:
                   - repository: MyRepo
           """;
           
           var node = YamlHelper.ParseToMapping(yaml);
           var result = new ResourcesParser().Parse(node["resources"]);
           
           Assert.That(result, Is.Not.Null);
       }
   }
   ```

7. **Add test YAML data** in `AdoPipelineTest.UnitTests/test_data/resources/`:
   ```yaml
   trigger:
     - main
   
   resources:
     repositories:
       - repository: MyRepo
         ref: main
   
   steps:
     - script: echo "test"
   ```

8. **Update your `.csproj`** to copy test data:
   ```xml
   <ItemGroup>
       <None Update="test_data/**/*.yaml">
           <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
       </None>
   </ItemGroup>
   ```

### Adding a Custom NUnit Constraint

**Example: Creating a constraint to assert on step display names**

1. **Create constraint** in `AdoPipelineTest.Nunit/Constraints/StepsContainConstraint.cs`:
   ```csharp
   namespace AdoPipelineTest.Nunit.Constraints;

   public class StepsContainConstraint : Constraint
   {
       private readonly string _displayName;

       public StepsContainConstraint(string displayName)
       {
           _displayName = displayName;
       }

       public override ConstraintResult ApplyTo<TActual>(TActual actual)
       {
           if (actual is not PipelineJob job)
           {
               return new ConstraintResult(this, actual, false);
           }

           var found = job.Steps.Any(s => s.DisplayName == _displayName);
           return new ConstraintResult(this, actual, found);
       }

       public override string Description => $"contains step with display name '{_displayName}'";
   }
   ```

2. **Add factory method** in `AdoPipelineTest.Nunit/Is.cs`:
   ```csharp
   public static StepsContainConstraint ContainsStep(string displayName)
       => new StepsContainConstraint(displayName);
   ```

3. **Use in tests**:
   ```csharp
   Assert.That(job, Is.ContainsStep("Build Solution"));
   ```

## Testing Guidelines

### Writing Unit Tests
- Place tests in `AdoPipelineTest.UnitTests/` with matching folder structure
- Use NUnit 4.x attributes: `[TestFixture]`, `[Test]`, `[TestCase]`
- Name test methods following the pattern: `MethodName_Condition_ExpectedResult`
  ```csharp
  [Test]
  public void Parse_WithValidYaml_ReturnsElement() { }
  
  [Test]
  public void Evaluate_WithMissingParameter_ThrowsException() { }
  ```
- Use `Assert.That()` with constraints (NUnit 4.x style)
  ```csharp
  Assert.That(result, Is.Not.Null);
  Assert.That(result.Count, Is.GreaterThan(0));
  ```

### Test Data Organization
- Store YAML test files in `test_data/` directories
- Organize by component: `test_data/parsing/`, `test_data/evaluation/`
- Use descriptive filenames: `valid-pipeline.yaml`, `conditional-steps.yaml`
- Add `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` to `.csproj`

### Sample Tests
The `AdoPipelineTest.Samples` project contains real-world integration test examples. Review these when:
- Building complex test scenarios
- Testing multiple features together
- Understanding end-to-end workflows

## Pull Request Process

1. **Create a feature branch** from `main`
   ```bash
   git checkout -b feature/add-resources-support
   ```

2. **Make your changes** following the guidelines above

3. **Run tests locally**
   ```bash
   dotnet test
   ```

4. **Commit with clear messages**
   ```bash
   git commit -m "feat: add support for resources section in pipelines"
   ```

5. **Push and open a PR**
   ```bash
   git push origin feature/add-resources-support
   ```

6. **PR should include**:
   - Clear description of changes
   - Why the change is needed
   - Any breaking changes or new dependencies
   - Tests covering the new functionality

## Code Review Checklist

When reviewing PRs, ensure:
- [ ] Code follows style guidelines (C# 14, nullable enabled, file-scoped namespaces)
- [ ] Naming conventions are consistent
- [ ] New public APIs are documented
- [ ] Unit tests are included and passing
- [ ] No hardcoded paths or test-specific logic in library code
- [ ] YAML test files are copied to output directory
- [ ] Changes maintain backward compatibility

## Reporting Issues

When reporting a bug or requesting a feature:

1. **Check existing issues** to avoid duplicates
2. **Provide a minimal example** - YAML snippet and test code if applicable
3. **Include environment info**: .NET version, OS, library version
4. **Describe expected vs actual behavior**

Example issue:
```
Title: Template expression evaluation fails with nested parameters

Description:
When using nested parameter access like `${{ parameters.env.region }}`, 
the evaluator throws InvalidOperationException instead of resolving the value.

Steps to reproduce:
1. Create pipeline with parameter: `env: { region: us-east-1 }`
2. Reference with: `${{ parameters.env.region }}`
3. Run PipelineTester.Run()

Expected: Should resolve to "us-east-1"
Actual: InvalidOperationException: "Parameter 'region' not found"

Environment: .NET 10.0, AdoPipelineTest.Lib 1.0.0, Windows 11
```

## Getting Help

- **Questions about architecture**: Check the Architecture section in README.md
- **Questions about ADO pipelines**: See [Azure DevOps Pipelines Documentation](https://docs.microsoft.com/en-us/azure/devops/pipelines/)
- **Questions about NUnit**: See [NUnit Documentation](https://docs.nunit.org/)
- **Questions about YamlDotNet**: See [YamlDotNet Documentation](https://github.com/aaubry/YamlDotNet)

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.

Thank you for making AdoPipelineTest better! 🎉

