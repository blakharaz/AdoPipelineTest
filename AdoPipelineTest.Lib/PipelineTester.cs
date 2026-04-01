using AdoPipelineTest.Evaluation;
using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest;

public class PipelineTester
{
    private readonly Dictionary<string, object> _parameters = [];
    private Dictionary<string, object> _variables = [];
    private string? _pipelinePath;

    public PipelineTester WithPipeline(string pipelinePath)
    {
        _pipelinePath = pipelinePath;
        return this;
    }

    public PipelineTester WithParameter(string name, object value)
    {
        _parameters[name] = value;
        return this;
    }

    public PipelineTester WithParameters(Dictionary<string, object> parameters)
    {
        foreach (var kvp in parameters)
        {
            _parameters[kvp.Key] = kvp.Value;
        }
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
        
        var parameters = ParameterEvaluator.EvaluateParameters(parseResult.Parameters, _parameters);

        var undefinedParameters = parameters.Where(p => p.Value == null && p.DefaultValue == null).Select(p => p.Name).ToList();
        if (undefinedParameters.Count > 0)
        {
            throw new InvalidOperationException($"Parameter(s) {string.Join(',', undefinedParameters)} were not provided a value and have no default.");
        }

        // Merge parsed variable defaults with user-provided variables
        // User-provided variables take precedence over defaults
        var mergedVariables = MergeVariables(parseResult.Variables, _variables);

        var stagesWithResolvedTemplates = parseResult.Stages.Select(TemplateResolver.ResolveStage);
        var evaluatedStages = stagesWithResolvedTemplates.Select(stage => PipelineEvaluator.EvaluateStage(stage, parameters.ToDictionary(item => item.Name, item => item.Value!), mergedVariables)).ToList();
        
        return new PipelineTestResult
        {
            Triggers = parseResult.Triggers,
            AgentPool = parseResult.AgentPool,
            Parameters = parameters.ToDictionary(item => item.Name),
            Variables = ConvertVariables(parseResult.Variables),
            Stages = evaluatedStages,
            Resources = ConvertResources(parseResult.Resources)
        };
    }

    private static List<PipelineVariable> ConvertVariables(IList<PipelineVariableElement> rawVariables)
    {
        return rawVariables.Select(rawVar => new PipelineVariable
        {
            Name = rawVar.Name,
            DefaultValue = rawVar.DefaultValue
        }).ToList();
    }

    private static List<PipelineResource> ConvertResources(IList<PipelineResourceElement> rawResources)
    {
        return rawResources.Select(rawResource => new PipelineResource
        {
            Type = rawResource.Type,
            Name = rawResource.Name
        }).ToList();
    }

    private static Dictionary<string, object> MergeVariables(IList<PipelineVariableElement> defaultVariables, Dictionary<string, object> userVariables)
    {
        var merged = new Dictionary<string, object>();

        // First, add all default values from the pipeline
        foreach (var variable in defaultVariables)
        {
            if (variable.DefaultValue != null)
            {
                merged[variable.Name] = variable.DefaultValue;
            }
        }

        // Then, override with user-provided variables
        foreach (var kvp in userVariables)
        {
            merged[kvp.Key] = kvp.Value;
        }

        return merged;
    }
}
