using AdoPipelineTest.Model;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsers;

internal static class TriggerParser
{
    internal static PipelineTriggers? ParseTriggers(YamlMappingNode rootNode)
    {
        if (!rootNode.Children.TryGetValue("trigger", out var triggerNode))
        {
            return null;
        }
        
        if (triggerNode is YamlSequenceNode sequenceNode)
        {
            return new PipelineTriggers
                { IncludedBranches = sequenceNode.Children.Select(node => (node as YamlScalarNode)?.Value).ToList() };
        }

        return null;
    }
}