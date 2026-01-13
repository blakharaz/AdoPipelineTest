using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.Ast;
using AdoPipelineTest.Utils;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Evaluation;

internal static class TemplateResolver
{
    internal static IList<PipelineStepElement> ResolveStepTemplate(TemplateStepElement stepTemplate)
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
            return ResolveStepTemplate(stepTemplate);
        }

        return [step];
    }
}