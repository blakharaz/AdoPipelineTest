# AdoPipelineTest Architecture

## Overview

AdoPipelineTest is built on a clean separation of concerns using a **two-phase processing model**: parsing YAML into an Abstract Syntax Tree (AST), then evaluating the AST into fully-resolved domain models.

## Two-Phase Processing Model

### Phase 1: Parsing (YAML → AST)

The **Parsing Phase** converts raw YAML files into an Abstract Syntax Tree without evaluating expressions or resolving parameters.

**Key Components:**
- `PipelineParser` - Main orchestrator that coordinates specialized parsers
- `*Parser.cs` classes - Dedicated parsers for each YAML element (TriggerParser, VariablesParser, StepsParser, etc.)
- `*Element.cs` classes - AST node classes that preserve the raw pipeline structure

**Flow:**
```
azure-pipelines.yaml
        ↓
   [PipelineParser]
   ├─ TriggerParser
   ├─ VariablesParser
   ├─ ParametersParser
   ├─ PoolParser
   └─ StepsParser
        ↓
   AST (with *Element nodes)
```

**What's Preserved:**
- Raw YAML structure
- Unresolved template expressions (`${{ }}` syntax)
- Conditional blocks (`${{ if }}...` `${{ else }}...`)
- Variable references
- Parameter placeholders

### Phase 2: Evaluation (AST → Model)

The **Evaluation Phase** transforms the AST into fully-resolved domain models by:
- Resolving template expressions (`${{ }}`)
- Substituting parameters with provided values
- Injecting variables
- Evaluating conditionals
- Creating concrete domain objects

**Key Components:**
- `PipelineEvaluator` - Main orchestrator that transforms AST to models
- `ExpressionEvaluator` - Parses and evaluates `${{ }}` expressions
- `ParameterEvaluator` - Resolves parameter references
- `TemplateResolver` - Resolves referenced template files
- `Pipeline*` classes - Domain models (fully evaluated)

**Flow:**
```
AST (with *Element nodes)
        ↓
  [PipelineEvaluator]
  ├─ ExpressionEvaluator
  ├─ ParameterEvaluator
  ├─ TemplateResolver
  └─ (Conditional evaluation)
        ↓
PipelineTestResult
├─ Pipeline
├─ Stages[]
├─ Jobs[]
└─ Steps[]
```

**Result:**
A fully instantiated `PipelineTestResult` ready for unit test assertions, with all expressions evaluated and parameters/variables substituted.

## Project Structure

### Core Library: `AdoPipelineTest.Lib/`

```
AdoPipelineTest.Lib/
├── Parsing/                      # Phase 1: YAML → AST
│   ├── PipelineParser.cs         # Main orchestrator
│   ├── TriggerParser.cs          # Trigger element parser
│   ├── VariablesParser.cs        # Variables element parser
│   ├── ParametersParser.cs       # Parameters element parser
│   ├── PoolParser.cs             # Pool element parser
│   ├── StepsParser.cs            # Steps element parser
│   ├── ExpressionParser.cs       # Template expression syntax parser
│   ├── TemplateExpressionParser.cs
│   ├── Ast/                      # AST node definitions
│   │   ├── StepsElement.cs
│   │   ├── JobElement.cs
│   │   ├── StageElement.cs
│   │   ├── VariablesElement.cs
│   │   ├── ParametersElement.cs
│   │   └── ...
│   └── Utils/
│       └── YamlExtensions.cs     # YamlDotNet helpers
│
├── Evaluation/                   # Phase 2: AST → Model
│   ├── PipelineEvaluator.cs      # Main orchestrator
│   ├── ExpressionEvaluator.cs    # Evaluates ${{ }} expressions
│   ├── ParameterEvaluator.cs     # Resolves parameters
│   ├── TemplateResolver.cs       # Resolves template files
│   └── ...
│
├── Model/                        # Domain models (Phase 2 output)
│   ├── Pipeline.cs               # Top-level model
│   ├── PipelineStage.cs          # Stage model
│   ├── PipelineJob.cs            # Job model
│   ├── PipelineStep.cs           # Base step model
│   ├── PipelineVariable.cs       # Variable model
│   ├── PipelineParameter.cs      # Parameter model
│   ├── PipelineTriggers.cs       # Triggers model
│   ├── PipelineAgentPool.cs      # Pool configuration model
│   ├── Steps/                    # Step type hierarchy
│   │   ├── TaskStep.cs           # task: X@Y steps
│   │   ├── ScriptStep.cs         # script: steps
│   │   ├── BashStep.cs           # bash: steps
│   │   └── ...
│   └── InvalidPipelineException.cs
│
├── PipelineTester.cs             # Main public API (fluent builder)
└── PipelineTestResult.cs         # Result container
```

### NUnit Integration: `AdoPipelineTest.Nunit/`

```
AdoPipelineTest.Nunit/
├── Constraints/                  # Custom NUnit constraints
│   ├── TriggersIncludeBranchConstraint.cs
│   ├── VmImageConstraint.cs
│   └── ...
└── Is.cs                         # Constraint factory methods
```

### Tests & Samples

```
AdoPipelineTest.UnitTests/        # Library unit tests
├── Parsing/                      # Tests for Phase 1
├── Evaluation/                   # Tests for Phase 2
├── Model/                        # Tests for domain models
└── test_data/                    # YAML test fixtures

AdoPipelineTest.Samples/          # Example integration tests
└── Nunit/
    ├── SimplePipeline/
    ├── Parameters/
    ├── Variables/
    ├── TemplateExpressions/
    ├── NestedTemplates/
    └── SimpleTemplates/
```

## Data Flow: Complete Example

Here's how a real pipeline flows through the system:

**Input YAML** (`azure-pipelines.yaml`):
```yaml
parameters:
  - name: environment
    type: string
    default: dev

variables:
  buildConfig: Debug

trigger:
  - main

stages:
  - stage: Build
    jobs:
      - job: CompileJob
        steps:
          - script: echo "Building for ${{ parameters.environment }}"
          - ${{ if eq(parameters.environment, 'prod') }}:
              - script: echo "Production build"
```

**Phase 1 Output** (AST):
```
StagesElement
└─ StageElement
   ├─ name: "Build"
   └─ JobElement
      ├─ name: "CompileJob"
      └─ StepsElement
         ├─ ScriptElement (raw: "echo 'Building for ${{ parameters.environment }}'")
         └─ ConditionalElement
            ├─ condition: "eq(parameters.environment, 'prod')"
            └─ ScriptElement (raw: "echo 'Production build'")
```

**Phase 2 Output** (Domain Model):
```
PipelineTestResult
└─ Stages[0]: PipelineStage
   ├─ Name: "Build"
   └─ Jobs[0]: PipelineJob
      ├─ Name: "CompileJob"
      └─ Steps[0]: ScriptStep
         ├─ DisplayName: "echo 'Building for production'"  ← evaluated
         └─ (if environment=="dev": second script excluded)
```

## Key Design Principles

### 1. Separation of Concerns
- **Parsing** focuses only on YAML structure
- **Evaluation** focuses on expression resolution and substitution
- **Models** are immutable domain objects with no behavior

### 2. Immutable Models
All domain models (`Pipeline*` classes) use:
- `required` properties for essential data
- `init` accessors to prevent mutation
- No methods that modify state

```csharp
public class PipelineStep
{
    public required string DisplayName { get; init; }
    public required string Type { get; init; }
}
```

### 3. Internal vs Public API
- **Parsing and Evaluation**: `internal` classes (implementation details)
- **Public API**: `PipelineTester` (fluent builder) and domain models only
- Test projects use `InternalsVisibleTo` for access

### 4. Type Hierarchy for Steps
Steps follow a hierarchy based on ADO syntax:

```
PipelineStep (abstract base)
├─ TaskStep (task: Name@Version)
├─ ScriptStep (script: ...)
├─ BashStep (bash: ...)
└─ ... other step types
```

## Expression Evaluation

Template expressions (`${{ }}` syntax) are evaluated during Phase 2:

**Supported:**
- Parameter access: `${{ parameters.name }}`
- Variable access: `${{ variables.name }}`
- Conditionals: `${{ if condition }}...` `${{ else }}...`
- Functions: `eq()`, `ne()`, `and()`, `or()`

**Example:**
```yaml
- script: echo "${{ variables.buildConfig }}"
- ${{ if eq(parameters.env, 'prod') }}:
    - script: echo "Production"
```

Becomes (with `env=prod`, `buildConfig=Release`):
```
Step 1: ScriptStep with DisplayName: "echo 'Release'"
Step 2: ScriptStep with DisplayName: "echo 'Production'" ← included
```

## Extensibility

The architecture supports adding new features:

### Adding a New YAML Element
1. Create `*Element` AST node in `Parsing/Ast/`
2. Create `*Parser` in `Parsing/`
3. Register in `PipelineParser`
4. Create domain model in `Model/`
5. Add evaluation logic in `PipelineEvaluator`

### Adding a New Constraint
1. Create constraint in `AdoPipelineTest.Nunit/Constraints/`
2. Add factory method in `Is.cs`
3. Use in tests

## Technologies

- **YamlDotNet 16.x** - YAML parsing via `YamlStream`, `YamlMappingNode`, `YamlSequenceNode`
- **NUnit 4.x** - Testing framework with extensible constraints
- **.NET 10.0 / C# 14** - Language features: records, pattern matching, nullable reference types

