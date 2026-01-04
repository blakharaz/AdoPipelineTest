using AdoPipelineTest.Evaluation;
using AdoPipelineTest.Parsing;

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

        var stagesWithResolvedTemplates = parseResult.Stages.Select(stageWithTemplates => TemplateResolver.ResolveStage(stageWithTemplates));
        var evaluatedStages = stagesWithResolvedTemplates.Select(stage => PipelineEvaluator.EvaluateStage(stage, _parameters, _variables)).ToList();
        
        return new PipelineTestResult
        {
            Triggers = parseResult.Triggers,
            AgentPool = parseResult.AgentPool,
            Stages = evaluatedStages 
        };
    }
}
