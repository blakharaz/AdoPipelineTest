using AdoPipelineTest.Parsing.Ast;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsing;

internal static class VariablesParser
{
    internal static List<PipelineVariableElement> ParseVariables(YamlMappingNode rootNode)
    {
        var variables = new List<PipelineVariableElement>();

        if (!rootNode.Children.TryGetValue("variables", out var variablesNode))
        {
            return variables;
        }

        if (variablesNode is not YamlMappingNode variablesMappingNode)
        {
            return variables;
        }

        foreach (var kvp in variablesMappingNode.Children)
        {
            var name = (kvp.Key as YamlScalarNode)?.Value;
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            var defaultValue = ExtractValue(kvp.Value);

            variables.Add(new PipelineVariableElement
            {
                Name = name,
                DefaultValue = defaultValue
            });
        }

        return variables;
    }

    private static object? ExtractValue(YamlNode valueNode)
    {
        return valueNode switch
        {
            YamlScalarNode scalarNode => scalarNode.Value,
            YamlMappingNode mappingNode => ExtractMappingValue(mappingNode),
            YamlSequenceNode sequenceNode => ExtractSequenceValue(sequenceNode),
            _ => null
        };
    }

    private static Dictionary<string, object?> ExtractMappingValue(YamlMappingNode mappingNode)
    {
        var result = new Dictionary<string, object?>();

        foreach (var kvp in mappingNode.Children)
        {
            var key = (kvp.Key as YamlScalarNode)?.Value;
            if (!string.IsNullOrEmpty(key))
            {
                result[key] = ExtractValue(kvp.Value);
            }
        }

        return result;
    }

    private static List<object?> ExtractSequenceValue(YamlSequenceNode sequenceNode)
    {
        var result = new List<object?>();

        foreach (var item in sequenceNode.Children)
        {
            result.Add(ExtractValue(item));
        }

        return result;
    }
}
