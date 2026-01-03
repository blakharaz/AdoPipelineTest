using AdoPipelineTest.Parsing.RawModel;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsing;

internal static class StepsParser
{
    public static IList<RawPipelineStage> ParseStages(YamlSequenceNode stagesNode)
    {
        return stagesNode.Select(ParseStage).ToList();
    }

    private static RawPipelineStage ParseStage(YamlNode stageNode)
    {
        if (stageNode is not YamlMappingNode stageMappingNode)
        {
            throw new FormatException("Stage node is not a mapping node");
        }

        var displayName = (stageMappingNode.Children["displayName"] as YamlScalarNode)?.Value;
        
        if (stageMappingNode.Children.TryGetValue("steps", out var stepsNode) && stepsNode is YamlSequenceNode stepsSequence)
        {
            return new RawPipelineStage
            {
                DisplayName = displayName,
                Jobs =
                [
                    new RawPipelineJob
                    {
                        Steps = ParseSteps(stepsSequence)
                    }
                ]
            };
        }

        if (stageMappingNode.Children.TryGetValue("jobs", out var jobsNode) && jobsNode is YamlSequenceNode jobsSequence)
        {
            return new RawPipelineStage
            {
                DisplayName = displayName,
                Jobs = ParseJobs(jobsSequence)
            };
        }

        throw new FormatException("Neither steps nor jobs sequence node found");
    }

    internal static IList<RawPipelineJob> ParseJobs(YamlSequenceNode jobsNode)
    {
        return jobsNode.Select(ParseJob).ToList();
    }

    private static RawPipelineJob ParseJob(YamlNode jobNode)
    {
        if (jobNode is not YamlMappingNode jobMappingNode)
        {
            throw new FormatException("Job node is not a mapping node");
        }
        
        if (jobMappingNode.Children["steps"] is not YamlSequenceNode stepsNode)
        {
            throw new FormatException("Steps node is not a sequence node");
        }

        var displayName = (jobMappingNode.Children["displayName"] as YamlScalarNode)?.Value;

        return new RawPipelineJob
        {
            DisplayName = displayName,
            Steps = ParseSteps(stepsNode)
        };
    }

    internal static IList<RawPipelineStep> ParseSteps(YamlSequenceNode stepsNode)
    {
        return stepsNode.Select(ParseStep).ToList();
    }

    private static RawPipelineStep ParseStep(YamlNode stepNode)
    {
        if (stepNode is not YamlMappingNode stepMappingNode)
        {
            throw new FormatException("Step node is not a mapping node");
        }
        
        var displayName = (stepMappingNode.Children["displayName"] as YamlScalarNode)?.Value;
        string? continueOnError = null;
        if (stepMappingNode.Children.TryGetValue("continueOnError", out var continueOnErrorNode))
        {
            continueOnError = (continueOnErrorNode as YamlScalarNode)?.Value;
        }

        if (stepMappingNode.Children.TryGetValue("task", out var taskNode))
        {
            return ParseTaskStep(displayName, continueOnError, taskNode as YamlScalarNode, stepMappingNode);
        }

        if (stepMappingNode.Children.TryGetValue("script", out var scriptNode))
        {
            return ParseScriptStep(displayName, continueOnError, scriptNode as YamlScalarNode, stepMappingNode);
        }
            
        throw new InvalidDataException("unknown step type"); 
    }

    private static RawScriptStep ParseScriptStep(string? displayName, string? continueOnError,
        YamlScalarNode? scriptNode, YamlMappingNode stepNode)
    {
        if (scriptNode == null)
        {
            throw new FormatException("script node is not a scalar node");
        }

        if (scriptNode.Value == null)
        {
            throw new FormatException("script node has no value");
        }
        
        return new RawScriptStep { DisplayName = displayName, ContinueOnError = continueOnError, Script = scriptNode.Value};
    }

    private static RawTaskStep ParseTaskStep(string? displayName, string? continueOnError, YamlScalarNode? taskNode,
        YamlMappingNode stepNode)
    {
        if (taskNode == null)
        {
            throw new FormatException("Task node is not a scalar node");
        }

        if (taskNode.Value == null)
        {
            throw new FormatException("Task node has no value");
        }
        
        return new RawTaskStep { DisplayName = displayName, ContinueOnError = continueOnError, TaskName = taskNode.Value};
    }

}