using AdoPipelineTest.Model;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Parsers;

internal static class StepsParser
{
    internal static IList<PipelineStep> ParseSteps(YamlSequenceNode stepsNode)
    {
        return stepsNode.Select(stepNode => new PipelineStep()).ToList();
    }
}