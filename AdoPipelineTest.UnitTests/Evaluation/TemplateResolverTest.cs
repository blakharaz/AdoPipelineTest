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

    [Fact]
    public void ResolveSteps_WithParameters_SubstitutesParameterValues()
    {
        var stepTemplate = new TemplateStepElement
        {
            Template = "test_data/template_resolver_step_templates/two_steps_with_parameters_template.yaml",
            ReferencedBy = "pipeline.yml",
            Parameters = new Dictionary<string, string>
            {
                ["configuration"] = "Release"
            }
        };

        var result = TemplateResolver.ResolveStepTemplate(stepTemplate);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var buildStep = result[0] as TaskStepElement;
        Assert.NotNull(buildStep);
        Assert.Equal("Build .NET Project", buildStep.DisplayName);
        Assert.Contains("arguments", buildStep.Inputs!.Keys);
        Assert.Equal("--configuration Release", buildStep.Inputs["arguments"] ?? "");

        var publishStep = result[1] as TaskStepElement;
        Assert.NotNull(publishStep);
        Assert.Equal("Publish Build Output", publishStep.DisplayName);
        Assert.Contains("arguments", publishStep.Inputs!.Keys);
        Assert.Contains("--configuration Release", publishStep.Inputs["arguments"] ?? "");
    }

    [Fact]
    public void ResolveSteps_WithParameters_PreservesParametersNotProvided()
    {
        var stepTemplate = new TemplateStepElement
        {
            Template = "test_data/template_resolver_step_templates/two_steps_with_parameters_template.yaml",
            ReferencedBy = "pipeline.yml",
            Parameters = new Dictionary<string, string>
            {
                ["unknownParameter"] = "someValue"
            }
        };

        var result = TemplateResolver.ResolveStepTemplate(stepTemplate);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var buildStep = result[0] as TaskStepElement;
        Assert.NotNull(buildStep);
        Assert.Equal("--configuration ${{ parameters.configuration }}", buildStep.Inputs!["arguments"] ?? "");
    }

    [Fact]
    public void ResolveSteps_WithEmptyParameters_DoesNotModifyTemplate()
    {
        var stepTemplate = new TemplateStepElement
        {
            Template = "test_data/template_resolver_step_templates/two_steps_with_parameters_template.yaml",
            ReferencedBy = "pipeline.yml",
            Parameters = new Dictionary<string, string>()
        };

        var result = TemplateResolver.ResolveStepTemplate(stepTemplate);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var buildStep = result[0] as TaskStepElement;
        Assert.NotNull(buildStep);
        Assert.Equal("--configuration ${{ parameters.configuration }}", buildStep.Inputs!["arguments"] ?? "");
    }

    [Fact]
    public void ResolveSteps_WithParametersAndVariableReference_SubstitutesOnlyParameters()
    {
        var stepTemplate = new TemplateStepElement
        {
            Template = "test_data/template_resolver_step_templates/two_steps_with_parameters_template.yaml",
            ReferencedBy = "pipeline.yml",
            Parameters = new Dictionary<string, string>
            {
                ["configuration"] = "Debug"
            }
        };

        var result = TemplateResolver.ResolveStepTemplate(stepTemplate);

        Assert.NotNull(result);

        var publishStep = result[1] as TaskStepElement;
        Assert.NotNull(publishStep);
        Assert.Contains("$(Build.ArtifactStagingDirectory)", publishStep.Inputs!["arguments"] ?? "");
        Assert.Contains("--configuration Debug", publishStep.Inputs!["arguments"] ?? "");
    }
}