using AdoPipelineTest.Parsing.Ast;
using YamlDotNet.RepresentationModel;
using System.Collections.Generic;

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

        // Handle the resources mapping - it can contain both flat resources and grouped resources
        if (resourcesNode is not YamlMappingNode resourcesMapping)
        {
            return resources;
        }

        foreach (var kvp in resourcesMapping.Children)
        {
            // Skip if key is not a scalar (e.g., null or complex key)
            if (kvp.Key is not YamlScalarNode keyNode || string.IsNullOrEmpty(keyNode.Value))
            {
                continue;
            }

            string groupName = keyNode.Value;
            YamlNode valueNode = kvp.Value;

            // Check if this is a grouped resource (sequence of resources under a group like repositories, pipelines, etc.)
            if (valueNode is YamlSequenceNode resourceSequence)
            {
                // Process each item in the sequence as a resource
                foreach (var itemNode in resourceSequence.Children)
                {
                    if (itemNode is YamlMappingNode resourceMapping)
                    {
                        // Extract the resource name from the mapping (commonly under "repository" or "pipeline" key)
                        string resourceName = ExtractResourceNameFromMapping(resourceMapping, groupName);
                        if (!string.IsNullOrEmpty(resourceName))
                        {
                            resources.Add(ParseSingleResource(resourceName, resourceMapping));
                        }
                    }
                }
            }
            // Check if this is a flat resource (direct mapping under resources)
            else if (valueNode is YamlMappingNode resourceMapping)
            {
                // The key itself is the resource name
                resources.Add(ParseSingleResource(groupName, resourceMapping));
            }
        }

        return resources;
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
            Type = GetScalarValue(resourceNode, "type") ?? "unknown",
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
            // Safely extract name and value
            string? name = null;
            string? value = null;
            var auth = new Dictionary<string, object?>();

            if (endpointNode.Children.TryGetValue(new YamlScalarNode("name"), out var nameNode) &&
                nameNode is YamlScalarNode nameScalar)
            {
                name = nameScalar.Value;
            }

            if (endpointNode.Children.TryGetValue(new YamlScalarNode("value"), out var valueNode) &&
                valueNode is YamlScalarNode valueScalar)
            {
                value = valueScalar.Value;
            }

            // Parse any additional properties as auth
            foreach (var kvp in endpointNode.Children)
            {
                // Skip if key is not a scalar
                if (kvp.Key is not YamlScalarNode keyNode || string.IsNullOrEmpty(keyNode.Value))
                {
                    continue;
                }

                string keyValue = keyNode.Value;
                
                // Skip name and value as they're handled separately
                if (keyValue == "name" || keyValue == "value")
                {
                    continue;
                }

                // Only add to auth if we have extra properties
                if (auth.Count == 0 && !(keyValue == "name" || keyValue == "value"))
                {
                    // We'll only create the auth dict if we actually have auth data
                }

                // Extract the value
                object? authValue = ExtractValue(kvp.Value);
                auth[keyValue] = authValue;
            }

            // Only create endpoint if we have at least a name or value
            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(value))
            {
                var endpoint = new PipelineResourceEndpoint
                {
                    Name = name ?? string.Empty,
                    Value = value,
                    Auth = auth.Count > 0 ? auth : null
                };

                endpoints.Add(endpoint);
            }
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