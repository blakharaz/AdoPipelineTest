# xUnit Helpers

This document describes the xUnit assertion helpers provided by `AdoPipelineTest.Xunit` for testing Azure DevOps YAML pipelines.

## Getting Started

```csharp
using AdoPipelineTest;
using AdoPipelineTest.Xunit;

var result = new PipelineTester()
    .WithPipeline("azure-pipelines.yaml")
    .Run();

Assert.HasStage(result, "Build");
Assert.StageCount(result, 1);
```

## Available Assertions

### Pipeline Result Assertions

| Method | Description |
|--------|-------------|
| `HasStage(result, stageDisplayName)` | Asserts that a stage with the given display name exists |
| `StageCount(result, count)` | Asserts the number of stages |
| `HasJob(result, stageDisplayName, jobDisplayName)` | Asserts that a job exists in the specified stage |
| `HasStep(result, stepDisplayName)` | Asserts that a step with the given display name exists anywhere |
| `HasStep(result, predicate)` | Asserts that a step matching the predicate exists |
| `HasTask(result, taskName)` | Asserts that a task with the given name exists |
| `TaskHasInput(result, taskName, inputKey, expectedValue?)` | Asserts that a task has a specific input, optionally checking the value |
| `HasVariable(result, variableName)` | Asserts that a variable exists |
| `HasVariable(result, variableName, expectedValue)` | Asserts that a variable exists with the given value |
| `HasParameter(result, parameterName)` | Asserts that a parameter exists |
| `ParameterHasValue(result, parameterName, expectedValue)` | Asserts that a parameter has the given value |
| `HasTrigger(result)` | Asserts that triggers are defined |
| `TriggersIncludeBranch(result, branchName)` | Asserts that triggers include the specified branch |
| `HasVmImage(result, vmImage)` | Asserts the VM image configuration |
| `HasScriptStep(result, scriptContent)` | Asserts that a script step containing the content exists |

### Stage Assertions

| Method | Description |
|--------|-------------|
| `HasJob(stage, jobDisplayName)` | Asserts that a job with the given display name exists |
| `JobCount(stage, expectedCount)` | Asserts the number of jobs in the stage |

### Job Assertions

| Method | Description |
|--------|-------------|
| `StepCount(job, expectedCount)` | Asserts the number of steps in the job |

## Examples

### Testing Stage Structure

```csharp
using Xunit;
using AdoPipelineTest;
using AdoPipelineTest.Xunit;

public class PipelineTests
{
    [Fact]
    public void Pipeline_HasCorrectStages()
    {
        var result = new PipelineTester()
            .WithPipeline("azure-pipelines.yaml")
            .Run();
        
        Assert.HasStage(result, "Build");
        Assert.HasStage(result, "Deploy");
        Assert.StageCount(result, 2);
    }
}
```

### Testing Tasks

```csharp
[Fact]
public void Pipeline_HasCorrectTasks()
{
    var result = new PipelineTester()
        .WithPipeline("azure-pipelines.yaml")
        .Run();
    
    Assert.HasTask(result, "UseDotNet@2");
    Assert.TaskHasInput(result, "UseDotNet@2", "version", "8.0.x");
}
```

### Testing Variables

```csharp
[Fact]
public void Pipeline_HasCorrectVariables()
{
    var result = new PipelineTester()
        .WithPipeline("azure-pipelines.yaml")
        .Run();
    
    Assert.HasVariable(result, "buildConfig");
    Assert.HasVariable(result, "buildConfig", "Release");
}
```

### Testing Parameters

```csharp
[Fact]
public void Pipeline_HasCorrectParameters()
{
    var result = new PipelineTester()
        .WithPipeline("azure-pipelines.yaml")
        .WithParameter("environment", "production")
        .Run();
    
    Assert.HasParameter(result, "environment");
    Assert.ParameterHasValue(result, "environment", "production");
}
```

### Testing Triggers

```csharp
[Fact]
public void Pipeline_HasCorrectTriggers()
{
    var result = new PipelineTester()
        .WithPipeline("azure-pipelines.yaml")
        .Run();
    
    Assert.HasTrigger(result);
    Assert.TriggersIncludeBranch(result, "main");
    Assert.TriggersIncludeBranch(result, "develop");
}
```
