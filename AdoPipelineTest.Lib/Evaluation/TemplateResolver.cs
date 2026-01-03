using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.RawModel;
using AdoPipelineTest.Utils;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Evaluation;

internal static class TemplateResolver
{
    internal static IList<RawPipelineStep> ResolveStepTemplate(RawTemplateStep stepTemplate)
    {
        var templatePath = Path.Combine(Path.GetDirectoryName(stepTemplate.ReferencedBy) ?? string.Empty, stepTemplate.Template);
        
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}");
        }

        var fileContent = File.ReadAllText(templatePath);
        
        var yamlStream = new YamlDotNet.RepresentationModel.YamlStream();
        using (var reader = new StringReader(fileContent))
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

    internal static RawPipelineStage ResolveStage(RawPipelineStage stageWithTemplates)
    {
        return new RawPipelineStage(stageWithTemplates)
        {
            Jobs = stageWithTemplates.Jobs.Select(ResolveJob).ToList()
        };
    }

    private static RawPipelineJob ResolveJob(RawPipelineJob jobWithTemplates)
    {
        return new RawPipelineJob(jobWithTemplates)
        {
            Steps = jobWithTemplates.Steps.SelectMany(ResolveStep).ToList()
        };
    }

    private static IList<RawPipelineStep> ResolveStep(RawPipelineStep step)
    {
        if (step is RawTemplateStep stepTemplate)
        {
            return ResolveStepTemplate(stepTemplate);
        }

        return [step];
    }
}