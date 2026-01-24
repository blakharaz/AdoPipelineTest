using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest.Evaluation;

internal static class PipelineEvaluator
{
    internal static PipelineStage EvaluateStage(PipelineStageElement stage, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        return new PipelineStage
        {
            Jobs = stage.Jobs.Select(job => EvaluateJob(job, parameters, variables)).ToList()
        };
    }

    internal static PipelineJob EvaluateJob(PipelineJobElement job, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        return new PipelineJob
        {
            DisplayName = job.DisplayName,
            Steps = job.Steps.SelectMany(step => EvaluateSteps(step, parameters, variables)).ToList()
        };
    }
    
    /// <summary>
    /// Evaluates a step element, which may be a conditional expression that expands to multiple steps,
    /// or a regular step that evaluates to a single step.
    /// </summary>
    internal static IEnumerable<PipelineStep> EvaluateSteps(PipelineStepElement step, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        if (step is ConditionalStepExpression conditionalStep)
        {
            return EvaluateConditionalStep(conditionalStep, parameters, variables);
        }

        return [EvaluateStep(step, parameters, variables)];
    }

    /// <summary>
    /// Evaluates a conditional step expression and returns the appropriate steps based on the condition.
    /// </summary>
    private static IEnumerable<PipelineStep> EvaluateConditionalStep(ConditionalStepExpression conditionalStep, 
        Dictionary<string, object> parameters, 
        Dictionary<string, object> variables)
    {
        var conditionResult = ExpressionEvaluator.EvaluateCondition(conditionalStep.Condition, parameters, variables);
        
        if (conditionResult)
        {
            // Condition is true, evaluate the then steps
            return conditionalStep.ThenSteps.SelectMany(step => EvaluateSteps(step, parameters, variables));
        }
        
        if (conditionalStep.ElseBranch != null)
        {
            // Condition is false and we have an else branch
            return EvaluateSteps(conditionalStep.ElseBranch, parameters, variables);
        }
        
        // Condition is false and no else branch, return empty
        return [];
    }
    
    internal static PipelineStep EvaluateStep(PipelineStepElement step, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        if (step is TaskStepElement taskStep)
        {
            return EvaluateStep(taskStep, parameters, variables);
        }

        if (step is ScriptStepElement scriptStep)
        {
            return EvaluateStep(scriptStep, parameters, variables);
        }
        
        throw new ArgumentException($"Unknown step type: {step.GetType().Name}");
    }

    internal static TaskStep EvaluateStep(TaskStepElement taskStep, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        var nullableParams = parameters.Cast<KeyValuePair<string, object?>>().ToDictionary(x => x.Key, x => x.Value);
        return new TaskStep
        {
            DisplayName = taskStep.DisplayName is null ? null : ExpressionEvaluator.EvaluateString(taskStep.DisplayName, parameters, variables),
            ContinueOnError = ExpressionEvaluator.EvaluateBool(taskStep.ContinueOnError, false),
            TaskName = taskStep.TaskName,
            Inputs = ExpressionEvaluator.EvaluateDictionaryValues(taskStep.Inputs, nullableParams, variables)
        };
    }

    internal static ScriptStep EvaluateStep(ScriptStepElement scriptStep, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        return new ScriptStep
        {
            DisplayName = scriptStep.DisplayName is null ? null : ExpressionEvaluator.EvaluateString(scriptStep.DisplayName, parameters, variables),
            ContinueOnError = ExpressionEvaluator.EvaluateBool(scriptStep.ContinueOnError, false),
            Script = ExpressionEvaluator.EvaluateString(scriptStep.Script, parameters, variables),
            Variables = variables
        };
    }
}