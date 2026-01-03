using AdoPipelineTest.Evaluation;
using AdoPipelineTest.Parsing.RawModel;

namespace AdoPipelineTest.UnitTests;

public class TemplateResolverTest
{
    [Test]
    public void ResolveSteps_LoadsTemplateFileAndReturnsSteps()
    {
        var stepTemplate = new RawTemplateStep
        {
            Template = "test_data/template_resolver_step_templates/two_steps_template.yaml",
            ReferencedBy = "pipeline.yml"
        };

        var result = TemplateResolver.ResolveStepTemplate(stepTemplate);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0], Is.Not.Null);
            Assert.That(result[0].DisplayName, Is.Null);
            Assert.That(result[0], Is.InstanceOf<RawTaskStep>());

            Assert.That(result[1], Is.Not.Null);
            Assert.That(result[1].DisplayName, Is.EqualTo("Publish Build Output"));
            Assert.That(result[1], Is.InstanceOf<RawTaskStep>());
        }
    }
}