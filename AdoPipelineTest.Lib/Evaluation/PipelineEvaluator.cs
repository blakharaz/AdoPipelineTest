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
            Steps = job.Steps.Select(step => EvaluateStep(step, parameters, variables)).ToList()
        };
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
        return new TaskStep
        {
            DisplayName = taskStep.DisplayName is null ? null : ExpressionEvaluator.EvaluateString(taskStep.DisplayName, parameters, variables),
            ContinueOnError = ExpressionEvaluator.EvaluateBool(taskStep.ContinueOnError, false),
            TaskName = taskStep.TaskName,
            Inputs = ExpressionEvaluator.EvaluateDictionaryValues(taskStep.Inputs, parameters, variables)
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