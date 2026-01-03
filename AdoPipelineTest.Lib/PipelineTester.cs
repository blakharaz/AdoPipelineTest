using AdoPipelineTest.Evaluation;
using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.RawModel;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest;

public class PipelineTester
{
    private Dictionary<string, object> _parameters = [];
    private Dictionary<string, object> _variables = [];
    private string? _pipelinePath;

    public PipelineTester WithPipeline(string pipelinePath)
    {
        _pipelinePath = pipelinePath;
        return this;
    }

    public PipelineTester WithParameters(Dictionary<string, object> parameters)
    {
        _parameters = new Dictionary<string, object>(parameters);
        return this;
    }

    public PipelineTester WithVariables(Dictionary<string, object> variables)
    {
        _variables = new Dictionary<string, object>(variables);
        return this;
    }

    public PipelineTestResult Run()
    {
        if (string.IsNullOrWhiteSpace(_pipelinePath))
        {
            throw new InvalidOperationException("Pipeline path not set. Use WithPipeline() to set the pipeline path.");
        }

        var parseResult = PipelineParser.Parse(_pipelinePath);

        var stagesWithResolvedTemplates = parseResult.Stages.Select(ResolveTemplates);
        var evaluatedStages = stagesWithResolvedTemplates.Select(stage => Evaluate(stage, _parameters, _variables)).ToList();
        
        return new PipelineTestResult
        {
            Triggers = parseResult.Triggers,
            AgentPool = parseResult.AgentPool,
            Stages = evaluatedStages 
        };
    }

    private RawPipelineStage ResolveTemplates(RawPipelineStage stageWithTemplates)
    {
        return TemplateResolver.ResolveStage(stageWithTemplates);
    }

    private PipelineStage Evaluate(RawPipelineStage stage, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        return new PipelineStage
        {
            Jobs = stage.Jobs.Select(job => Evaluate(job, parameters, variables)).ToList()
        };
    }

    private PipelineJob Evaluate(RawPipelineJob job, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        return new PipelineJob
        {
            DisplayName = job.DisplayName,
            Steps = job.Steps.Select(step => Evaluate(step, parameters, variables)).ToList()
        };
    }
    
    private PipelineStep Evaluate(RawPipelineStep step, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        if (step is RawTaskStep taskStep)
        {
            return EvaluateStep(taskStep, parameters, variables);
        }

        if (step is RawScriptStep scriptStep)
        {
            return EvaluateStep(scriptStep, parameters, variables);
        }
        
        throw new ArgumentException($"Unknown step type: {step.GetType().Name}");
    }

    private TaskStep EvaluateStep(RawTaskStep taskStep, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        return new TaskStep
        {
            DisplayName = taskStep.DisplayName,
            ContinueOnError = ExpressionEvaluator.EvaluateBool(taskStep.ContinueOnError, false),
            TaskName = taskStep.TaskName
        };
    }

    private ScriptStep EvaluateStep(RawScriptStep scriptStep, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        return new ScriptStep
        {
            DisplayName = scriptStep.DisplayName,
            ContinueOnError = ExpressionEvaluator.EvaluateBool(scriptStep.ContinueOnError, false),
            Script = scriptStep.Script
        };
    }
}
