using AdoPipelineTest.Parsing.RawModel;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsing;

internal class PipelineParser
{
    internal static PipelineParseResult Parse(string yamlPath)
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

        return new PipelineParseResult
        {
            Triggers = triggers,
            AgentPool = agentPool,
            Stages = stages
        };
    }
    
    
    private static IList<RawPipelineStage> ParseStages(YamlMappingNode rootNode)
    {
        if (rootNode.Children.TryGetValue("steps", out var stepsInRoot) && stepsInRoot is YamlSequenceNode stepsInRootSequence)
        {
            var steps = StepsParser.ParseSteps(stepsInRootSequence);
            return [new RawPipelineStage { Jobs = [new RawPipelineJob { Steps = steps }] }];
        }

        if (rootNode.Children.TryGetValue("jobs", out var jobsInRoot) && jobsInRoot is YamlSequenceNode jobsInRootSequence)
        {
            var jobs = StepsParser.ParseJobs(jobsInRootSequence);
            return [new RawPipelineStage { Jobs = jobs }];
        }

        if (rootNode.Children.TryGetValue("stages", out var stagesInRoot) && stagesInRoot is YamlSequenceNode stagesInRootSequence)
        {
            var stages = StepsParser.ParseStages(stagesInRootSequence);
            return stages;
        }

        throw new InvalidDataException("neither stages, jobs nor steps defined");
    }
}