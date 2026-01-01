using AdoPipelineTest.Model;
using AdoPipelineTest.Parsers;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest;

public class PipelineTester
{
    public PipelineTestResult Run(string yamlPath)
    {
        // Set up the input
        using TextReader input = File.OpenText(yamlPath);

        // Load the stream
        var yaml = new YamlStream();
        yaml.Load(input);
        
        var document = yaml.Documents[0];
        var rootNode = document.RootNode as YamlMappingNode;

        if (rootNode == null)
        {
            throw new FormatException("Root node is not a mapping node");
        }
        
        var triggers = TriggerParser.ParseTriggers(rootNode);
        var agentPool = PoolParser.ParseAgentPool(rootNode);
        var stages = ParseStages(rootNode);

        return new PipelineTestResult
        {
            Triggers = triggers,
            AgentPool = agentPool,
            Stages = stages
        };
    }

    private IList<PipelineStage> ParseStages(YamlMappingNode rootNode)
    {
        if (rootNode.Children["steps"] is YamlSequenceNode stepsInRoot)
        {
            var steps = StepsParser.ParseSteps(stepsInRoot);
            return [new PipelineStage { Jobs = [new PipelineJob { Steps = steps }] }];
        }

        return [];
    }
}