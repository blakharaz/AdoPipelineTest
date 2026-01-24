using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing.Ast;
using AdoPipelineTest.Utils;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsing;

internal static class StepsParser
{
    internal static IList<PipelineStageElement> ParseStages(YamlSequenceNode stagesNode, string pipelinePath)
    {
        return stagesNode.Select(stageNode => ParseStage(stageNode, pipelinePath)).ToList();
    }

    private static PipelineStageElement ParseStage(YamlNode stageNode, string pipelinePath)
    {
        if (stageNode is not YamlMappingNode stageMappingNode)
        {
            throw new FormatException("Stage node is not a mapping node");
        }

        var displayName = stageMappingNode.GetChildIfExists<YamlScalarNode>("displayName")?.Value;
        
        if (stageMappingNode.TryGetChild<YamlSequenceNode>("steps", out var stepsNode))
        {
            return new PipelineStageElement
            {
                DisplayName = displayName,
                Jobs =
                [
                    new PipelineJobElement
                    {
                        Steps = ParseSteps(stepsNode, pipelinePath)
                    }
                ]
            };
        }

        if (stageMappingNode.TryGetChild<YamlSequenceNode>("jobs", out var jobsNode))
        {
            return new PipelineStageElement
            {
                DisplayName = displayName,
                Jobs = ParseJobs(jobsNode, pipelinePath)
            };
        }

        throw new FormatException("Neither steps nor jobs sequence node found");
    }

    internal static IList<PipelineJobElement> ParseJobs(YamlSequenceNode jobsNode, string pipelinePath)
    {
        return jobsNode.Select(jobNode => ParseJob(jobNode, pipelinePath)).ToList();
    }

    private static PipelineJobElement ParseJob(YamlNode jobNode, string pipelinePath)
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

        return new PipelineJobElement
        {
            DisplayName = displayName,
            Steps = ParseSteps(stepsNode, pipelinePath)
        };
    }

    internal static IList<PipelineStepElement> ParseSteps(YamlSequenceNode stepsNode, string pipelinePath)
    {
        return stepsNode.Select(stepNode => ParseStep(stepNode, pipelinePath)).ToList();
    }

    private static PipelineStepElement ParseStep(YamlNode stepNode, string pipelinePath)
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

    private static PipelineStepElement ParseTemplateStep(YamlScalarNode templateNode, YamlMappingNode stepMappingNode, string pipelinePath)
    {
        return new TemplateStepElement
        {
            Template = templateNode.Value ?? throw new InvalidDataException("template node has no value"), 
            ReferencedBy = pipelinePath
        };
    }

    private static ScriptStepElement ParseScriptStep(string? displayName, string? continueOnError,
        YamlScalarNode scriptNode, YamlMappingNode stepNode)
    {
        var script = scriptNode.Value;

        if (string.IsNullOrEmpty(script))
        {
            throw new InvalidPipelineException("script node has no content", "", scriptNode);
        }
        
        return new ScriptStepElement { DisplayName = displayName, ContinueOnError = continueOnError, Script = scriptNode.Value};
    }

    private static TaskStepElement ParseTaskStep(string? displayName, string? continueOnError, YamlScalarNode taskNode,
        YamlMappingNode stepNode)
    {
        return new TaskStepElement
        {
            DisplayName = displayName, 
            ContinueOnError = continueOnError, 
            TaskName = taskNode.Value ?? throw new InvalidPipelineException("task node must have value"),
            Inputs = stepNode.GetChildIfExists<YamlMappingNode>("inputs")?.ToDictionary()
        };
    }
}