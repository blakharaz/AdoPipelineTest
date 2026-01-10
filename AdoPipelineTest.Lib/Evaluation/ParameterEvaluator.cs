using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing.RawModel;

namespace AdoPipelineTest.Evaluation;

internal static class ParameterEvaluator
{
    internal static IList<PipelineParameter> EvaluateParameters(IList<RawPipelineParameter> rawModel, IDictionary<string, object> parameterValues)
    {
        return rawModel.Select(param => EvaluateParameter(param, parameterValues)).ToList();
    }

    private static PipelineParameter EvaluateParameter(RawPipelineParameter rawModel, IDictionary<string, object> parameterValues)
    {
        var result = new PipelineParameter
        {
            Name = rawModel.Name,
            DefaultValue = rawModel.DefaultValue,
            DisplayName = rawModel.DisplayName,
            AllowedValues = rawModel.AllowedValues,
            Value = parameterValues.TryGetValue(rawModel.Name, out var value) ? value : rawModel.DefaultValue,
        };
        
        return result;
    }
}