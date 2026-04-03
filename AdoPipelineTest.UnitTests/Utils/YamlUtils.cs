using NUnit.Framework;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.UnitTests.Utils;

internal static class YamlUtils
{
    internal static YamlMappingNode LoadPipelineFile(string path)
    {
        // Set up the input
        using TextReader input = File.OpenText(path);

        // Load the stream
        var yaml = new YamlStream();
        yaml.Load(input);
    
        var document = yaml.Documents[0];
        var rootNode = document.RootNode as YamlMappingNode;

        return rootNode!;
    }
}