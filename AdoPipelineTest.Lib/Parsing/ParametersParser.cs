using AdoPipelineTest.Parsing.Ast;
using AdoPipelineTest.Utils;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsing;

internal static class ParametersParser
{
    internal static List<PipelineParameterElement> ParseParameters(YamlMappingNode pipelineRoot)
    {
        var result = new List<PipelineParameterElement>();
        
        if (!pipelineRoot.TryGetChild("parameters", out YamlSequenceNode parametersSequence))
        {
            return result;
        }

        foreach (var paramNode in parametersSequence.OfType<YamlMappingNode>())
        {
            var parameter = ParseParameter(paramNode);
            if (parameter != null)
            {
                result.Add(parameter);
            }
        }

        return result;
    }

    private static PipelineParameterElement? ParseParameter(YamlMappingNode paramMapping)
    {
        var name = GetScalarValue(paramMapping, "name");
        var type = GetScalarValue(paramMapping, "type");

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
        {
            return null;
        }

        return new PipelineParameterElement
        {
            Name = name,
            Type = type,
            DisplayName = GetScalarValue(paramMapping, "displayName"),
            DefaultValue = GetValue(paramMapping, "default"),
            AllowedValues = GetAllowedValues(paramMapping, "values")
        };
    }

    private static string? GetScalarValue(YamlMappingNode mapping, string key)
    {
        return mapping.GetChildIfExists<YamlScalarNode>(key)?.Value;
    }

    private static object? GetValue(YamlMappingNode node, string key)
    {
        var valueNode = node.GetChildIfExists(key);
        return valueNode switch
        {
            YamlScalarNode scalar => ParseScalarValue(scalar.Value),
            YamlSequenceNode sequence => ConvertSequenceToList(sequence),
            YamlMappingNode _ when node.Children.Count == 0 => new Dictionary<object, object>(),
            YamlMappingNode mapping => ConvertMappingToObject(mapping),
            _ => null
        };
    }

    private static IList<object>? GetAllowedValues(YamlMappingNode mapping, string key)
    {
        var sequenceNode = mapping.GetChildIfExists<YamlSequenceNode>(key);
        return sequenceNode != null ? ConvertSequenceToList(sequenceNode) : null;
    }

    private static object? ParseScalarValue(string? value)
    {
        if (bool.TryParse(value, out var boolValue))
        {
            return boolValue;
        }

        if (int.TryParse(value, out var intValue))
        {
            return intValue;
        }

        if (double.TryParse(value, out var doubleValue))
        {
            return doubleValue;
        }

        return value;
    }

    private static IList<object> ConvertSequenceToList(YamlSequenceNode sequenceNode)
    {
        var list = new List<object>();

        foreach (var item in sequenceNode)
        {
            var value = item switch
            {
                YamlScalarNode scalar => (object?)(scalar.Value ?? string.Empty),
                YamlMappingNode mapping => ConvertMappingToObject(mapping),
                YamlSequenceNode sequence => ConvertSequenceToList(sequence),
                _ => null
            };

            if (value != null)
            {
                list.Add(value);
            }
        }

        return list;
    }

    private static Dictionary<object, object> ConvertMappingToObject(YamlMappingNode mappingNode)
    {
        var dict = new Dictionary<object, object>();

        foreach (var kvp in mappingNode.Children)
        {
            var key = kvp.Key is YamlScalarNode keyNode ? keyNode.Value : kvp.Key.ToString();
            var value = kvp.Value switch
            {
                YamlScalarNode scalar => (object?)(scalar.Value ?? string.Empty),
                YamlMappingNode mapping => ConvertMappingToObject(mapping),
                YamlSequenceNode sequence => ConvertSequenceToList(sequence),
                _ => null
            };

            if (!string.IsNullOrEmpty(key) && value != null)
            {
                dict[key] = value;
            }
        }

        return dict;
    }
}