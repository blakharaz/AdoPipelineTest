using AdoPipelineTest.Parsing.Ast;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsing;

internal static class ResourcesParser
{
    internal static IList<PipelineResourceElement> ParseResources(YamlMappingNode rootNode)
    {
        var resources = new List<PipelineResourceElement>();

        if (!rootNode.Children.TryGetValue("resources", out var resourcesNode))
        {
            return resources;
        }

        foreach (var kvp in ((YamlMappingNode)resourcesNode).Children)
        {
            var name = GetScalarValue(kvp.Key as YamlScalarNode);
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }
            
            resources.Add(ParseSingleResource(name, kvp.Value as YamlMappingNode));
        }

        return resources;
    }

    private static PipelineResourceElement ParseSingleResource(
        string name,
        YamlMappingNode? resourceNode)
    {
        var element = new PipelineResourceElement
        {
            Name = name,
            Type = GetScalarValue(resourceNode, "type"),
            Source = GetScalarValue(resourceNode, "source"),
            Version = GetScalarValue(resourceNode, "version"),
            Trigger = ParseTriggerList(resourceNode),
            Endpoints = ParseEndpoints(resourceNode)
        };

        return element;
    }

    private static IList<string>? ParseTriggerList(YamlMappingNode? resourceNode)
    {
        if (resourceNode == null)
        {
            return null;
        }

        if (!resourceNode.Children.TryGetValue("trigger", out var triggerNode) ||
            triggerNode is not YamlSequenceNode triggerSeq)
        {
            return null;
        }

        var triggers = new List<string>();
        foreach (var node in triggerSeq.Children)
        {
            var scalar = node as YamlScalarNode;
            if (!string.IsNullOrEmpty(scalar?.Value))
            {
                triggers.Add(scalar.Value);
            }
        }

        return triggers.Count > 0 ? triggers : null;
    }

    private static IList<PipelineResourceEndpoint>? ParseEndpoints(YamlMappingNode? resourceNode)
    {
        if (resourceNode == null)
        {
            return null;
        }

        if (!resourceNode.Children.TryGetValue("endpoints", out var endpointsNode) ||
            endpointsNode is not YamlSequenceNode endpointsSeq)
        {
            return null;
        }

        var endpoints = new List<PipelineResourceEndpoint>();
        foreach (var endpointNode in endpointsSeq.Children.OfType<YamlMappingNode>())
        {
            var name = GetScalarValue(endpointNode, "name");
            var value = GetScalarValue(endpointNode, "value");
            var auth = new Dictionary<string, object?>();

            // Parse any additional properties as auth
            foreach (var kvp in endpointNode.Children)
            {
                var key = kvp.Key as YamlScalarNode;
                if (key != null && (key.Value == "name" || key.Value == "value"))
                {
                    continue; // Skip name and value as they're handled separately
                }

                auth[key.Value] = ExtractValue(kvp.Value);
            }

            var endpoint = new PipelineResourceEndpoint
            {
                Name = name,
                Value = value,
                Auth = auth
            };

            endpoints.Add(endpoint);
        }

        return endpoints.Count > 0 ? endpoints : null;
    }

    private static string? GetScalarValue(YamlScalarNode? scalarNode)
    {
        return scalarNode?.Value;
    }

    private static string? GetScalarValue(YamlMappingNode? node, string key)
    {
        if (node == null || !node.Children.TryGetValue(key, out var valueNode))
        {
            return null;
        }

        var scalar = valueNode as YamlScalarNode;
        
        if (scalar == null)
        {
            return null;
        }
        
        return scalar.Value;
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

    private static Dictionary<string, object?>? ExtractMappingValue(YamlMappingNode node)
    {
        var result = new Dictionary<string, object?>();

        foreach (var kvp in node.Children)
        {
            var key = (kvp.Key as YamlScalarNode)?.Value;
            if (!string.IsNullOrEmpty(key))
            {
                result[key] = ExtractValue(kvp.Value);
            }
        }

        return result.Count > 0 ? result : null;
    }

    private static List<object?>? ExtractSequenceValue(YamlSequenceNode sequenceNode)
    {
        var result = new List<object?>();

        foreach (var item in sequenceNode.Children)
        {
            result.Add(ExtractValue(item));
        }

        return result.Count > 0 ? result : null;
    }
}