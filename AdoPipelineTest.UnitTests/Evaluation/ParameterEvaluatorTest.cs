using Xunit;
using AdoPipelineTest.Evaluation;
using AdoPipelineTest.Parsing.Ast;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Evaluation;

public class ParameterEvaluatorTest
{
    [Fact]
    public void EvaluateParameters_WithDefaultValues_UsesDefaults()
    {
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "projectName",
                Type = "string",
                DisplayName = "Project Name",
                DefaultValue = "MySampleProject"
            },
            new()
            {
                Name = "enableTests",
                Type = "boolean",
                DisplayName = "Enable Unit Tests",
                DefaultValue = true
            }
        };
        var parameterValues = new Dictionary<string, object>();

        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        Assert.Equal(2, result.Count);
        var projectName = result.First(p => p.Name == "projectName");
        Assert.Equal("MySampleProject", projectName.Value);
        Assert.Equal("Project Name", projectName.DisplayName);
        Assert.Equal("MySampleProject", projectName.DefaultValue);

        var enableTests = result.First(p => p.Name == "enableTests");
        Assert.Equal(true, enableTests.Value);
        Assert.Equal(true, enableTests.DefaultValue);
    }

    [Fact]
    public void EvaluateParameters_WithSuppliedValues_OverridesDefaults()
    {
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "projectName",
                Type = "string",
                DefaultValue = "DefaultProject"
            },
            new()
            {
                Name = "buildConfiguration",
                Type = "string",
                DefaultValue = "Release"
            }
        };
        var parameterValues = new Dictionary<string, object>
        {
            { "projectName", "CustomProject" },
            { "buildConfiguration", "Debug" }
        };

        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        Assert.Equal(2, result.Count);
        var projectName = result.First(p => p.Name == "projectName");
        Assert.Equal("CustomProject", projectName.Value);
        Assert.Equal("DefaultProject", projectName.DefaultValue);

        var buildConfig = result.First(p => p.Name == "buildConfiguration");
        Assert.Equal("Debug", buildConfig.Value);
        Assert.Equal("Release", buildConfig.DefaultValue);
    }

    [Fact]
    public void EvaluateParameters_WithNumericDefaults_PreservesType()
    {
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "timeoutMinutes",
                Type = "number",
                DefaultValue = 30
            },
            new()
            {
                Name = "retryCount",
                Type = "number",
                DefaultValue = 3
            }
        };
        var parameterValues = new Dictionary<string, object>();

        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        var timeout = result.First(p => p.Name == "timeoutMinutes");
        Assert.Equal(30, timeout.Value);
        Assert.Equal(30, timeout.DefaultValue);
    }

    [Fact]
    public void EvaluateParameters_WithAllowedValues_IncludesConstraints()
    {
        var allowedConfigs = new List<object> { "Debug", "Release", "CI" };
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "buildConfiguration",
                Type = "string",
                DefaultValue = "Release",
                AllowedValues = allowedConfigs
            }
        };
        var parameterValues = new Dictionary<string, object>();

        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        var buildConfig = result.First();
        Assert.NotNull(buildConfig.AllowedValues);
        Assert.Equal(3, buildConfig.AllowedValues.Count);
        Assert.Contains("Debug", buildConfig.AllowedValues);
        Assert.Contains("Release", buildConfig.AllowedValues);
        Assert.Contains("CI", buildConfig.AllowedValues);
    }

    [Fact]
    public void EvaluateParameters_WithObjectDefaults_PreservesObjectType()
    {
        var defaultSettings = new Dictionary<object, object>();
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "buildSettings",
                Type = "object",
                DefaultValue = defaultSettings
            }
        };
        var parameterValues = new Dictionary<string, object>();

        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        var buildSettings = result.First();
        Assert.IsType<Dictionary<object, object>>(buildSettings.Value);
        Assert.IsType<Dictionary<object, object>>(buildSettings.DefaultValue);
    }

    [Fact]
    public void EvaluateParameters_WithEnvironmentVariableDefault_PreservesAsString()
    {
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "outputDirectory",
                Type = "string",
                DefaultValue = "$(Build.ArtifactStagingDirectory)"
            }
        };
        var parameterValues = new Dictionary<string, object>();

        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        var outputDir = result.First();
        Assert.Equal("$(Build.ArtifactStagingDirectory)", outputDir.Value);
    }

    [Fact]
    public void EvaluateParameters_WithMixedProvidedAndDefault_UsesCorrectValues()
    {
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "projectName",
                Type = "string",
                DefaultValue = "DefaultProject"
            },
            new()
            {
                Name = "buildConfiguration",
                Type = "string",
                DefaultValue = "Release"
            },
            new()
            {
                Name = "timeoutMinutes",
                Type = "number",
                DefaultValue = 30
            }
        };
        var parameterValues = new Dictionary<string, object>
        {
            { "projectName", "CustomProject" }
        };

        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        Assert.Equal(3, result.Count);
        
        var projectName = result.First(p => p.Name == "projectName");
        Assert.Equal("CustomProject", projectName.Value);
        
        var buildConfig = result.First(p => p.Name == "buildConfiguration");
        Assert.Equal("Release", buildConfig.Value);
        
        var timeout = result.First(p => p.Name == "timeoutMinutes");
        Assert.Equal(30, timeout.Value);
    }

    [Fact]
    public void EvaluateParameters_WithNullDefault_ValueIsNull()
    {
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "optionalField",
                Type = "string",
                DefaultValue = null
            }
        };
        var parameterValues = new Dictionary<string, object>();

        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        var optionalField = result.First();
        Assert.Null(optionalField.Value);
        Assert.Null(optionalField.DefaultValue);
    }

    [Fact]
    public void EvaluateParameters_WithEmptyParameterList_ReturnsEmpty()
    {
        var rawParameters = new List<PipelineParameterElement>();
        var parameterValues = new Dictionary<string, object>();

        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        Assert.Empty(result);
    }

    [Fact]
    public void EvaluateParameters_PreservesDisplayName()
    {
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "projectName",
                Type = "string",
                DisplayName = "Project Name",
                DefaultValue = "MySampleProject"
            }
        };
        var parameterValues = new Dictionary<string, object>();

        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        var projectName = result.First();
        Assert.Equal("Project Name", projectName.DisplayName);
    }
}