using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest.Evaluation;

internal static class ParameterEvaluator
{
    internal static IList<PipelineParameter> EvaluateParameters(IList<PipelineParameterElement> astParameterElements, IDictionary<string, object> parameterValues)
    {
        return astParameterElements.Select(param => EvaluateParameter(param, parameterValues)).ToList();
    }

    private static PipelineParameter EvaluateParameter(PipelineParameterElement model, IDictionary<string, object> parameterValues)
    {
        var result = new PipelineParameter
        {
            Name = model.Name,
            DefaultValue = model.DefaultValue,
            DisplayName = model.DisplayName,
            AllowedValues = model.AllowedValues,
            Value = parameterValues.TryGetValue(model.Name, out var value) ? value : model.DefaultValue,
        };
        
        return result;
    }
}