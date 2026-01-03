using AdoPipelineTest.Parsing.RawModel;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsing;

internal static class StepsParser
{
    internal static IList<RawPipelineStage> ParseStages(YamlSequenceNode stagesNode, string pipelinePath)
    {
        return stagesNode.Select(stageNode => ParseStage(stageNode, pipelinePath)).ToList();
    }

    private static RawPipelineStage ParseStage(YamlNode stageNode, string pipelinePath)
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
                        Steps = ParseSteps(stepsSequence, pipelinePath)
                    }
                ]
            };
        }

        if (stageMappingNode.Children.TryGetValue("jobs", out var jobsNode) && jobsNode is YamlSequenceNode jobsSequence)
        {
            return new RawPipelineStage
            {
                DisplayName = displayName,
                Jobs = ParseJobs(jobsSequence, pipelinePath)
            };
        }

        throw new FormatException("Neither steps nor jobs sequence node found");
    }

    internal static IList<RawPipelineJob> ParseJobs(YamlSequenceNode jobsNode, string pipelinePath)
    {
        return jobsNode.Select(jobNode => ParseJob(jobNode, pipelinePath)).ToList();
    }

    private static RawPipelineJob ParseJob(YamlNode jobNode, string pipelinePath)
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
            Steps = ParseSteps(stepsNode, pipelinePath)
        };
    }

    internal static IList<RawPipelineStep> ParseSteps(YamlSequenceNode stepsNode, string pipelinePath)
    {
        return stepsNode.Select(stepNode => ParseStep(stepNode, pipelinePath)).ToList();
    }

    private static RawPipelineStep ParseStep(YamlNode stepNode, string pipelinePath)
    {
        if (stepNode is not YamlMappingNode stepMappingNode)
        {
            throw new FormatException("Step node is not a mapping node");
        }

        string? displayName = null;
        if (stepMappingNode.Children.TryGetValue("displayName", out var displayNameNode))
        {
            displayName = (displayNameNode as YamlScalarNode)?.Value;
        }

        string? continueOnError = null;
        if (stepMappingNode.Children.TryGetValue("continueOnError", out var continueOnErrorNode))
        {
            continueOnError = (continueOnErrorNode as YamlScalarNode)?.Value;
        }

        if (stepMappingNode.Children.TryGetValue("task", out var taskNode) && taskNode is YamlScalarNode taskScalar)
        {
            return ParseTaskStep(displayName, continueOnError, taskScalar, stepMappingNode);
        }

        if (stepMappingNode.Children.TryGetValue("script", out var scriptNode) && scriptNode is YamlScalarNode scriptScalar)
        {
            return ParseScriptStep(displayName, continueOnError, scriptScalar, stepMappingNode);
        }

        if (stepMappingNode.Children.TryGetValue("template", out var templateNode) && templateNode is YamlScalarNode templateScalar)
        {
            return ParseTemplateStep(templateScalar, stepMappingNode, pipelinePath);
        }
            
        throw new InvalidDataException("unknown step type"); 
    }

    private static RawPipelineStep ParseTemplateStep(YamlScalarNode templateNode, YamlMappingNode stepMappingNode, string pipelinePath)
    {
        if (templateNode.Value == null)
        {
            throw new FormatException("script node has no value");
        }
        
        return new RawTemplateStep { Template = templateNode.Value, ReferencedBy = pipelinePath };
    }

    private static RawScriptStep ParseScriptStep(string? displayName, string? continueOnError,
        YamlScalarNode scriptNode, YamlMappingNode stepNode)
    {
        if (scriptNode.Value == null)
        {
            throw new FormatException("script node has no value");
        }
        
        return new RawScriptStep { DisplayName = displayName, ContinueOnError = continueOnError, Script = scriptNode.Value};
    }

    private static RawTaskStep ParseTaskStep(string? displayName, string? continueOnError, YamlScalarNode taskNode,
        YamlMappingNode stepNode)
    {
        if (taskNode.Value == null)
        {
            throw new FormatException("Task node has no value");
        }
        
        return new RawTaskStep { DisplayName = displayName, ContinueOnError = continueOnError, TaskName = taskNode.Value};
    }
}