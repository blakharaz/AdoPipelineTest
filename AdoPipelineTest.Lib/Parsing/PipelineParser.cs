using AdoPipelineTest.Parsing.Ast;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsing;

internal class PipelineParser
{
    internal static PipelineSyntaxTree Parse(string yamlPath)
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
        var parameters = ParametersParser.ParseParameters(rootNode);
        var variables = VariablesParser.ParseVariables(rootNode);
        var stages = ParseStages(rootNode, yamlPath);

        return new PipelineSyntaxTree
        {
            Triggers = triggers,
            AgentPool = agentPool,
            Parameters = parameters,
            Variables = variables,
            Stages = stages
        };
    }
    
    
    private static IList<PipelineStageElement> ParseStages(YamlMappingNode rootNode, string pipelinePath)
    {
        if (rootNode.Children.TryGetValue("steps", out var stepsInRoot) && stepsInRoot is YamlSequenceNode stepsInRootSequence)
        {
            var steps = StepsParser.ParseSteps(stepsInRootSequence, pipelinePath);
            return [new PipelineStageElement { Jobs = [new PipelineJobElement { Steps = steps }] }];
        }

        if (rootNode.Children.TryGetValue("jobs", out var jobsInRoot) && jobsInRoot is YamlSequenceNode jobsInRootSequence)
        {
            var jobs = StepsParser.ParseJobs(jobsInRootSequence, pipelinePath);
            return [new PipelineStageElement { Jobs = jobs }];
        }

        if (rootNode.Children.TryGetValue("stages", out var stagesInRoot) && stagesInRoot is YamlSequenceNode stagesInRootSequence)
        {
            var stages = StepsParser.ParseStages(stagesInRootSequence, pipelinePath);
            return stages;
        }

        throw new InvalidDataException("neither stages, jobs nor steps defined");
    }
}