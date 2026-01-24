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
        var steps = new List<PipelineStepElement>();
        int i = 0;
        
        while (i < stepsNode.Count())
        {
            var stepNode = stepsNode.ElementAt(i);
            
            // Check if this is a conditional step
            if (IsConditionalStep(stepNode, out var conditionalType))
            {
                var conditionalStep = ParseConditionalStep(stepsNode, ref i, pipelinePath);
                steps.Add(conditionalStep);
            }
            else
            {
                steps.Add(ParseStep(stepNode, pipelinePath));
                i++;
            }
        }
        
        return steps;
    }

    private static bool IsConditionalStep(YamlNode stepNode, out string? conditionalType)
    {
        conditionalType = null;
        
        if (stepNode is not YamlMappingNode mapping || mapping.Children.Count != 1)
        {
            return false;
        }
        
        var key = mapping.Children.Keys.First() as YamlScalarNode;
        var keyValue = TrimTrailingColon(key?.Value ?? "");
        
        if (keyValue.StartsWith("${{ if ") && keyValue.EndsWith(" }}"))
        {
            conditionalType = "if";
            return true;
        }
        
        if (keyValue.StartsWith("${{ else if ") && keyValue.EndsWith(" }}"))
        {
            conditionalType = "elseif";
            return true;
        }
        
        if (keyValue == "${{ else }}")
        {
            conditionalType = "else";
            return true;
        }
        
        return false;
    }
    
    private static string TrimTrailingColon(string value)
    {
        return value.EndsWith(":") ? value[..^1] : value;
    }

    private static ConditionalStepExpression ParseConditionalStep(
        YamlSequenceNode stepsNode, 
        ref int currentIndex, 
        string pipelinePath)
    {
        var stepNode = stepsNode.ElementAt(currentIndex);
        var mapping = stepNode as YamlMappingNode;
        var key = mapping!.Children.Keys.First() as YamlScalarNode;
        var keyValue = TrimTrailingColon(key!.Value!);
        
        // Parse the condition
        TemplateExpression? condition = null;
        if (keyValue.StartsWith("${{ if ") || keyValue.StartsWith("${{ else if "))
        {
            var conditionText = keyValue
                .Replace("${{ if ", "")
                .Replace("${{ else if ", "")
                .Replace(" }}", "")
                .Trim();
            
            var expr = new TemplateExpressionParser(conditionText).ParseExpression();
            condition = new TemplateExpression { Children = [expr] };
        }
        else // else without condition
        {
            // Create a "true" condition for else
            condition = new TemplateExpression { Children = [new StringLiteral { Value = "true" }] };
        }
        
        // Parse then branch
        var value = mapping.Children.Values.First() as YamlSequenceNode;
        var thenSteps = value?.Select(n => ParseStep(n, pipelinePath)).ToList() ?? [];
        
        currentIndex++;
        
        // Check for else-if or else
        PipelineStepElement? elseBranch = null;
        if (currentIndex < stepsNode.Count())
        {
            var nextNode = stepsNode.ElementAt(currentIndex);
            
            if (IsConditionalStep(nextNode, out var nextType) && 
                (nextType == "elseif" || nextType == "else"))
            {
                // Recursively parse the else-if or else as nested conditional
                elseBranch = ParseConditionalStep(stepsNode, ref currentIndex, pipelinePath);
            }
        }
        
        return new ConditionalStepExpression
        {
            Condition = condition,
            ThenSteps = thenSteps,
            ElseBranch = elseBranch
        };
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
        
        return new ScriptStepElement { DisplayName = displayName, ContinueOnError = continueOnError, Script = script};
    }

    private static TaskStepElement ParseTaskStep(string? displayName, string? continueOnError, YamlScalarNode taskNode,
        YamlMappingNode stepNode)
    {
        var enabled = stepNode.GetChildIfExists<YamlScalarNode>("enabled");
        var inputs = stepNode.GetChildIfExists<YamlMappingNode>("inputs");
        var inputsDict = inputs is not null ? inputs.ToDictionary() : new Dictionary<string, string>();
        
        return new TaskStepElement
        {
            DisplayName = displayName,
            Enabled = enabled?.Value is null ? null : ExpressionParser.ParseStringExpression(enabled.Value),
            ContinueOnError = continueOnError, 
            TaskName = taskNode.Value ?? throw new InvalidPipelineException("task node must have value"),
            Inputs = inputsDict
        };
    }
}