using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.UnitTests.Utils;

internal static class YamlUtils
{
    internal static YamlMappingNode LoadPipelineFile(string path)
    {
        using TextReader input = File.OpenText(path);

        var yaml = new YamlStream();
        yaml.Load(input);
    
        var document = yaml.Documents[0];
        var rootNode = document.RootNode as YamlMappingNode;

        return rootNode!;
    }
}