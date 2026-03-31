using AdoPipelineTest.Parsing.Ast;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsing;

internal static class ResourcesParser
{
    internal static List<PipelineResourceElement> ParseResources(YamlMappingNode rootNode)
    {
        var resources = new List<PipelineResourceElement>();

        if (!rootNode.Children.TryGetValue("resources", out var resourcesNode))
        {
            return resources;
        }

        // Handle the resources mapping - it can contain both flat resources and grouped resources
        if (resourcesNode is not YamlMappingNode resourcesMapping)
        {
            return resources;
        }

        foreach (var (key, valueNode) in resourcesMapping.Children)
        {
            // Skip if key is not a scalar (e.g., null or complex key)
            if (key is not YamlScalarNode keyNode || string.IsNullOrEmpty(keyNode.Value))
            {
                continue;
            }

            var groupName = keyNode.Value;

            switch (valueNode)
            {
                // Check if this is a grouped resource (sequence of resources under a group like repositories, pipelines, etc.)
                case YamlSequenceNode resourceSequence:
                {
                    ProcessResourceSequence(resourceSequence, groupName, resources);
                    break;
                }
                // Check if this is a flat resource (direct mapping under resources)
                case YamlMappingNode resourceMapping:
                {
                    // The key itself is the resource name
                    resources.Add(ParseSingleResource(groupName, resourceMapping));
                    break;
                }
            }
        }

        return resources;
    }

    private static void ProcessResourceSequence(YamlSequenceNode resourceSequence, string groupName, List<PipelineResourceElement> resources)
    {
        // Process each item in the sequence as a resource
        foreach (var itemNode in resourceSequence.Children)
        {
            if (itemNode is not YamlMappingNode resourceMapping)
            {
                continue;
            }
            
            // Extract the resource name from the mapping (commonly under "repository" or "pipeline" keys
            var resourceName = ExtractResourceNameFromMapping(resourceMapping, groupName);
            if (!string.IsNullOrEmpty(resourceName))
            {
                resources.Add(ParseSingleResource(resourceName, resourceMapping));
            }
        }
    }

    private static string ExtractResourceNameFromMapping(YamlMappingNode resourceMapping, string groupName)
    {
        // For grouped resources, the name is often in a field like "repository", "pipeline", "container", etc.
        // Default to using the group name if we can't find a specific name field
        string? name = null;

        // Try common name fields based on resource type
        if (resourceMapping.Children.TryGetValue(new YamlScalarNode("repository"), out var repoNode) &&
            repoNode is YamlScalarNode repoScalar)
        {
            name = repoScalar.Value;
        }
        else if (resourceMapping.Children.TryGetValue(new YamlScalarNode("pipeline"), out var pipeNode) &&
                 pipeNode is YamlScalarNode pipeScalar)
        {
            name = pipeScalar.Value;
        }
        else if (resourceMapping.Children.TryGetValue(new YamlScalarNode("container"), out var contNode) &&
                 contNode is YamlScalarNode contScalar)
        {
            name = contScalar.Value;
        }
        else if (resourceMapping.Children.TryGetValue(new YamlScalarNode("package"), out var pkgNode) &&
                 pkgNode is YamlScalarNode pkgScalar)
        {
            name = pkgScalar.Value;
        }
        else if (resourceMapping.Children.TryGetValue(new YamlScalarNode("name"), out var nameNode) &&
                 nameNode is YamlScalarNode nameScalar)
        {
            name = nameScalar.Value;
        }

        // Fallback: if we still don't have a name, use a combination of group and index or just the group
        if (string.IsNullOrEmpty(name))
        {
            // In real ADO pipelines, you might have multiple items in a group without explicit names
            // For simplicity, we'll use the group name, though this could cause collisions
            name = groupName;
        }

        return name;
    }

    private static PipelineResourceElement ParseSingleResource(
        string name,
        YamlMappingNode? resourceNode)
    {
        var element = new PipelineResourceElement
        {
            Name = name,
            Type = ExtractScalarValue(resourceNode, "type") ?? "unknown",
            Source = ExtractScalarValue(resourceNode, "source"),
            Version = ExtractScalarValue(resourceNode, "version"),
            Trigger = ParseTriggerList(resourceNode),
            Endpoints = ParseEndpoints(resourceNode)
        };

        return element;
    }

    private static List<string>? ParseTriggerList(YamlMappingNode? resourceNode)
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

    private static List<PipelineResourceEndpoint>? ParseEndpoints(YamlMappingNode? resourceNode)
    {
        if (resourceNode == null)
        {
            return null;
        }

        var endpointsSeq = GetEndpointsSequence(resourceNode);  

        var endpoints = endpointsSeq?.Children.OfType<YamlMappingNode>()
            .Select(CreateEndpointFromNode)
            .Where(IsValidEndpoint)
            .ToList();

        return endpoints?.Count > 0 ? endpoints : null;
    }

    private static YamlSequenceNode? GetEndpointsSequence(YamlMappingNode resourceNode)
    {
        return resourceNode.Children.TryGetValue("endpoints", out var endpointsNode) &&
               endpointsNode is YamlSequenceNode endpointsSeq
            ? endpointsSeq
            : null;
    }

    private static bool IsValidEndpoint(PipelineResourceEndpoint endpoint)
    {
        return !string.IsNullOrEmpty(endpoint.Name) || !string.IsNullOrEmpty(endpoint.Value);
    }

    private static PipelineResourceEndpoint CreateEndpointFromNode(YamlMappingNode endpointNode)
    {
        var name = ExtractScalarValue(endpointNode, "name");
        var value = ExtractScalarValue(endpointNode, "value");
        var auth = BuildAuthDictionary(endpointNode);

        return new PipelineResourceEndpoint
        {
            Name = name ?? string.Empty,
            Value = value,
            Auth = auth.Count > 0 ? auth : null
        };
    }

    private static Dictionary<string, object?> BuildAuthDictionary(YamlMappingNode endpointNode)
    {
        return endpointNode.Children
            .Where(IsRelevantAuthProperty)
            .Select(kvp => new { Key = ((YamlScalarNode)kvp.Key).Value!, Value = ExtractValue(kvp.Value) })
            .ToDictionary(item => item.Key, item => item.Value);
    }

    private static bool IsRelevantAuthProperty(KeyValuePair<YamlNode, YamlNode> kvp)
    {
        if (kvp.Key is not YamlScalarNode keyNode || string.IsNullOrEmpty(keyNode.Value))
        {
            return false;
        }

        var keyValue = keyNode.Value;
        return keyValue != "name" && keyValue != "value";
    }

    private static string? ExtractScalarValue(YamlMappingNode? node, string key)
    {
        if (node == null || !node.Children.TryGetValue(key, out var valueNode))
        {
            return null;
        }

        return (valueNode as YamlScalarNode)?.Value;
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