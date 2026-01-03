using AdoPipelineTest.Model;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsing;

internal static class PoolParser
{
    internal static PipelineAgentPool? ParseAgentPool(YamlMappingNode rootNode)
    {
        if (!rootNode.Children.TryGetValue("pool", out var poolNode))
        {
            return null;
        }
        
        if (poolNode is YamlMappingNode mappingNode)
        {
            return new PipelineAgentPool
            {
                VmImage = (mappingNode.Children["vmImage"] as YamlScalarNode)?.Value
            };
        }

        return null;
    }
}