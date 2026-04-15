using System.Text.RegularExpressions;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.Ast;
using AdoPipelineTest.Utils;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Evaluation;

internal static partial class TemplateResolver
{
    internal static IList<PipelineStepElement> ResolveStepTemplate(TemplateStepElement stepTemplate)
    {
        var templatePath = Path.Combine(Path.GetDirectoryName(stepTemplate.ReferencedBy) ?? string.Empty, stepTemplate.Template);
        
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}");
        }

        var fileContent = File.ReadAllText(templatePath);
        var processedContent = SubstituteTemplateParameters(fileContent, stepTemplate.Parameters);
        
        var yamlStream = new YamlDotNet.RepresentationModel.YamlStream();
        using (var reader = new StringReader(processedContent))
        {
            yamlStream.Load(reader);
        }

        if (yamlStream.Documents.Count == 0 || yamlStream.Documents[0].RootNode is not YamlDotNet.RepresentationModel.YamlMappingNode rootNode)
        {
            throw new FormatException("Template file must be map");
        }

        if (!rootNode.TryGetChild<YamlSequenceNode>("steps", out var stepsNode))
        {
            throw new FormatException("Steps template file must contain steps node at root level");
        }

        return StepsParser.ParseSteps(stepsNode, templatePath);
    }

    private static string SubstituteTemplateParameters(string content, Dictionary<string, string> parameters)
    {
        if (parameters.Count == 0)
        {
            return content;
        }

        var parameterRegex = TemplateParameterRegex();
        
        return parameterRegex.Replace(content, match =>
        {
            var parameterName = match.Groups[1].Value;
            
            if (parameters.TryGetValue(parameterName, out var value))
            {
                return value;
            }
            
            return match.Value;
        });
    }

    [GeneratedRegex(@"\$\{\{\s*parameters\.([a-zA-Z_][a-zA-Z0-9_]*)\s*\}\}")]
    private static partial Regex TemplateParameterRegex();

    internal static PipelineStageElement ResolveStage(PipelineStageElement stageWithTemplates)
    {
        return new PipelineStageElement(stageWithTemplates)
        {
            Jobs = stageWithTemplates.Jobs.Select(ResolveJob).ToList()
        };
    }

    private static PipelineJobElement ResolveJob(PipelineJobElement jobWithTemplates)
    {
        return new PipelineJobElement(jobWithTemplates)
        {
            Steps = jobWithTemplates.Steps.SelectMany(ResolveStep).ToList()
        };
    }

    private static IList<PipelineStepElement> ResolveStep(PipelineStepElement step)
    {
        if (step is TemplateStepElement stepTemplate)
        {
            var resolvedSteps = ResolveStepTemplate(stepTemplate);
            return resolvedSteps.SelectMany(ResolveStep).ToList();
        }

        return [step];
    }
}