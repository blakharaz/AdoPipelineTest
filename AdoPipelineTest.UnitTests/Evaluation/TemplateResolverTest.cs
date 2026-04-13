using Xunit;
using AdoPipelineTest.Evaluation;
using AdoPipelineTest.Parsing.Ast;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Evaluation;

public class TemplateResolverTest
{
    [Fact]
    public void ResolveSteps_LoadsTemplateFileAndReturnsSteps()
    {
        var stepTemplate = new TemplateStepElement
        {
            Template = "test_data/template_resolver_step_templates/two_steps_template.yaml",
            ReferencedBy = "pipeline.yml"
        };

        var result = TemplateResolver.ResolveStepTemplate(stepTemplate);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.NotNull(result[0]);
        Assert.Null(result[0].DisplayName);
        Assert.IsType<TaskStepElement>(result[0]);

        Assert.NotNull(result[1]);
        Assert.Equal("Publish Build Output", result[1].DisplayName);
        Assert.IsType<TaskStepElement>(result[1]);
    }
}