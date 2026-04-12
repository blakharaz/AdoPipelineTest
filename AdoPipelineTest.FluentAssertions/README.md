# FluentAssertions Extensions

This library provides FluentAssertions extensions for testing Azure DevOps YAML pipelines using `AdoPipelineTest`.

## Installation

```bash
dotnet add package AdoPipelineTest.FluentAssertions
```

## Usage

```csharp
using AdoPipelineTest.PipelineAssertions;
using FluentAssertions;

[TestClass]
public class PipelineTests
{
    [TestMethod]
    public void VerifyPipeline()
    {
        var result = new PipelineTester()
            .WithPipeline("path/to/pipeline.yaml")
            .Run();

        result.Should().HaveStage("Build");
        result.Should().HaveTask("UseDotNet@2");
        result.Should().HaveVariable("buildConfiguration", "Release");
        result.Should().HaveVmImage("ubuntu-latest");
    }
}
```

## Available Assertions

### PipelineTestResultAssertions

- `HaveStage(stageName)` - Assert a stage exists by name or display name
- `HaveStageCount(count)` - Assert the number of stages
- `HaveJob(stageName, jobName)` - Assert a job exists in a stage
- `HaveStep(stepDisplayName)` - Assert a step exists by display name
- `HaveTask(taskName)` - Assert a task exists (e.g., "UseDotNet@2")
- `HaveVariable(variableName)` - Assert a variable exists
- `HaveVariable(variableName, value)` - Assert a variable with specific value
- `HaveParameter(parameterName)` - Assert a parameter exists
- `HaveTrigger()` - Assert triggers are configured
- `HaveTriggers()` - Assert triggers are configured
- `HaveVmImage(vmImage)` - Assert the VM image
- `HaveScriptStepContaining(text)` - Assert a script step containing text

### PipelineStageAssertions

- `HaveJob(jobName)` - Assert a job exists in the stage
- `HaveJobCount(count)` - Assert the number of jobs

### PipelineJobAssertions

- `HaveStepCount(count)` - Assert the number of steps

### PipelineTriggersAssertions

- `IncludeBranch(branchName)` - Assert a branch is included in triggers

### PipelineAgentPoolAssertions

- `HaveVmImage(vmImage)` - Assert the VM image in the agent pool

## Chaining

Assertions return `AndConstraint<T>` allowing method chaining:

```csharp
result.Should()
    .HaveStage("Build")
    .And
    .HaveTask("DotNetCoreCLI@2");
```
