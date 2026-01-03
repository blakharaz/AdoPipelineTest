using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Utils;

internal static class YamlExtensions
{
    internal static YamlNode? GetChildIfExists(this YamlMappingNode parentNode, string key)
    {
        return parentNode.Children.TryGetValue(key, out var value) ? value : null;
    }

    internal static TNodeType? GetChildIfExists<TNodeType>(this YamlMappingNode parentNode, string key)
        where TNodeType : YamlNode
    {
        return parentNode.GetChildIfExists(key) as TNodeType;
    }
    
    internal static bool TryGetChild<TNodeType>(this YamlMappingNode parentNode, string key, out TNodeType child)
        where TNodeType : YamlNode
    {
        if (parentNode.Children.TryGetValue(key, out var value) && value is TNodeType typedValue)
        {
            child = typedValue;
            return true;
        }

        child = null!;
        return false;
    }
}