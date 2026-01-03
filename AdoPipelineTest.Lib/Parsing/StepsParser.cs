using AdoPipelineTest.Parsing.RawModel;
using AdoPipelineTest.Utils;
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

        var displayName = stageMappingNode.GetChildIfExists<YamlScalarNode>("displayName")?.Value;
        
        if (stageMappingNode.TryGetChild<YamlSequenceNode>("steps", out var stepsNode))
        {
            return new RawPipelineStage
            {
                DisplayName = displayName,
                Jobs =
                [
                    new RawPipelineJob
                    {
                        Steps = ParseSteps(stepsNode, pipelinePath)
                    }
                ]
            };
        }

        if (stageMappingNode.TryGetChild<YamlSequenceNode>("jobs", out var jobsNode))
        {
            return new RawPipelineStage
            {
                DisplayName = displayName,
                Jobs = ParseJobs(jobsNode, pipelinePath)
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

        var displayName = jobMappingNode.GetChildIfExists<YamlScalarNode>("displayName")?.Value;;

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

        var displayName = stepMappingNode.GetChildIfExists<YamlScalarNode>("displayName")?.Value;;
        var continueOnError = stepMappingNode.GetChildIfExists<YamlScalarNode>("continueOnError")?.Value;

        if (stepMappingNode.TryGetChild<YamlScalarNode>("task", out var taskNode))
        {
            return ParseTaskStep(displayName, continueOnError, taskNode, stepMappingNode);
        }

        if (stepMappingNode.TryGetChild<YamlScalarNode>("script", out var scriptNode))
        {
            return ParseScriptStep(displayName, continueOnError, scriptNode, stepMappingNode);
        }

        if (stepMappingNode.TryGetChild<YamlScalarNode>("template", out var templateNode))
        {
            return ParseTemplateStep(templateNode, stepMappingNode, pipelinePath);
        }
            
        throw new InvalidDataException("unknown step type"); 
    }

    private static RawPipelineStep ParseTemplateStep(YamlScalarNode templateNode, YamlMappingNode stepMappingNode, string pipelinePath)
    {
        return new RawTemplateStep
        {
            Template = templateNode.Value ?? throw new InvalidDataException("template node has no value"), 
            ReferencedBy = pipelinePath
        };
    }

    private static RawScriptStep ParseScriptStep(string? displayName, string? continueOnError,
        YamlScalarNode scriptNode, YamlMappingNode stepNode)
    {
        return new RawScriptStep { DisplayName = displayName, ContinueOnError = continueOnError, Script = scriptNode.Value};
    }

    private static RawTaskStep ParseTaskStep(string? displayName, string? continueOnError, YamlScalarNode taskNode,
        YamlMappingNode stepNode)
    {
        return new RawTaskStep { DisplayName = displayName, ContinueOnError = continueOnError, TaskName = taskNode.Value};
    }
}