using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest.Evaluation;

internal static class PipelineEvaluator
{
internal static PipelineStage EvaluateStage(PipelineStageElement stage, Dictionary<string, object?> parameters, Dictionary<string, object?> variables, Dictionary<string, string> runtimeVariables)
    {
        return new PipelineStage
        {
            Name = stage.Name,
            DisplayName = stage.DisplayName,
            DependsOn = stage.DependsOn.ToList(),
            Jobs = stage.Jobs.Select(job => EvaluateJob(job, parameters, variables, runtimeVariables)).ToList()
        };
    }

    internal static PipelineJob EvaluateJob(PipelineJobElement job, Dictionary<string, object?> parameters, Dictionary<string, object?> variables, Dictionary<string, string> runtimeVariables)
    {
        return new PipelineJob
        {
            Name = job.Name,
            DisplayName = job.DisplayName,
            DependsOn = job.DependsOn.ToList(),
            Steps = job.Steps.SelectMany(step => EvaluateSteps(step, parameters, variables, runtimeVariables)).ToList()
        };
    }
    
    internal static IEnumerable<PipelineStep> EvaluateSteps(PipelineStepElement step, Dictionary<string, object?> parameters, Dictionary<string, object?> variables, Dictionary<string, string> runtimeVariables)
    {
        if (step is ConditionalStepExpression conditionalStep)
        {
            return EvaluateConditionalStep(conditionalStep, parameters, variables, runtimeVariables);
        }

        return [EvaluateStep(step, parameters, variables, runtimeVariables)];
    }

    private static IEnumerable<PipelineStep> EvaluateConditionalStep(ConditionalStepExpression conditionalStep, 
        Dictionary<string, object?> parameters, 
        Dictionary<string, object?> variables,
        Dictionary<string, string> runtimeVariables)
    {
        var conditionResult = ExpressionEvaluator.EvaluateCondition(conditionalStep.Condition, parameters, variables);
        
        if (conditionResult)
        {
            return conditionalStep.ThenSteps.SelectMany(step => EvaluateSteps(step, parameters, variables, runtimeVariables));
        }
        
        if (conditionalStep.ElseBranch != null)
        {
            return EvaluateSteps(conditionalStep.ElseBranch, parameters, variables, runtimeVariables);
        }
        
        return [];
    }
    
    internal static PipelineStep EvaluateStep(PipelineStepElement step, Dictionary<string, object?> parameters, Dictionary<string, object?> variables, Dictionary<string, string> runtimeVariables)
    {
        if (step is TaskStepElement taskStep)
        {
            return EvaluateStep(taskStep, parameters, variables, runtimeVariables);
        }

        if (step is ScriptStepElement scriptStep)
        {
            return EvaluateStep(scriptStep, parameters, variables, runtimeVariables);
        }
        
        throw new ArgumentException($"Unknown step type: {step.GetType().Name}");
    }

    internal static TaskStep EvaluateStep(TaskStepElement taskStep, Dictionary<string, object?> parameters, Dictionary<string, object?> variables, Dictionary<string, string> runtimeVariables)
    {
        var nullableParams = parameters.Cast<KeyValuePair<string, object?>>().ToDictionary(x => x.Key, x => x.Value);
        return new TaskStep
        {
            DisplayName = taskStep.DisplayName is null ? null : ExpressionEvaluator.EvaluateString(taskStep.DisplayName, parameters, variables, runtimeVariables),
            ContinueOnError = ExpressionEvaluator.EvaluateBool(taskStep.ContinueOnError, false),
            TaskName = taskStep.TaskName,
            Inputs = ExpressionEvaluator.EvaluateDictionaryValues(taskStep.Inputs, nullableParams, variables, runtimeVariables)
        };
    }

    internal static ScriptStep EvaluateStep(ScriptStepElement scriptStep, Dictionary<string, object?> parameters, Dictionary<string, object?> variables, Dictionary<string, string> runtimeVariables)
    {
        return new ScriptStep
        {
            DisplayName = scriptStep.DisplayName is null ? null : ExpressionEvaluator.EvaluateString(scriptStep.DisplayName, parameters, variables, runtimeVariables),
            ContinueOnError = ExpressionEvaluator.EvaluateBool(scriptStep.ContinueOnError, false),
            Script = ExpressionEvaluator.EvaluateString(scriptStep.Script, parameters, variables, runtimeVariables),
            Variables = variables
        };
    }
}