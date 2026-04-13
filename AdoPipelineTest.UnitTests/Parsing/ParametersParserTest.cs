using Xunit;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.UnitTests.Utils;
using YamlDotNet.RepresentationModel;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Parsing;

public class ParametersParserTest
{
    private const string PipelineNoParametersPath = "test_data/pipeline_parser/simple_pipeline_just_steps.yaml";
    private const string PipelineWithParametersPath = "test_data/pipeline_parser/pipeline_with_parameters.yaml";
    
    [Fact]
    public void PipelineWithNoVariables()
    {
        var parameters = ParametersParser.ParseParameters(LoadPipeline(PipelineNoParametersPath));

        Assert.Empty(parameters);
    }

    [Fact]
    public void ParseParameters()
    {
        var parameters = ParametersParser.ParseParameters(LoadPipeline(PipelineWithParametersPath));

        Assert.Equal(6, parameters.Count);

        var projectNameParam = parameters.First(p => p.Name == "projectName");
        Assert.Equal("string", projectNameParam.Type);
        Assert.Equal("Project Name", projectNameParam.DisplayName);
        Assert.Equal("MySampleProject", projectNameParam.DefaultValue);
        Assert.Null(projectNameParam.AllowedValues);

        var enableTestsParam = parameters.First(p => p.Name == "enableTests");
        Assert.Equal("boolean", enableTestsParam.Type);
        Assert.Equal("Enable Unit Tests", enableTestsParam.DisplayName);
        var defaultValue = (bool?)enableTestsParam.DefaultValue;
        Assert.Equal(true, defaultValue);
        Assert.Null(enableTestsParam.AllowedValues);

        var timeoutParam = parameters.First(p => p.Name == "timeoutMinutes");
        Assert.Equal("number", timeoutParam.Type);
        Assert.Equal("Timeout in Minutes", timeoutParam.DisplayName);
        Assert.Equal(30, timeoutParam.DefaultValue);
        Assert.Null(timeoutParam.AllowedValues);

        var buildConfigParam = parameters.First(p => p.Name == "buildConfiguration");
        Assert.Equal("string", buildConfigParam.Type);
        Assert.Equal("Build Configuration", buildConfigParam.DisplayName);
        Assert.Equal("Release", buildConfigParam.DefaultValue);
        Assert.NotNull(buildConfigParam.AllowedValues);
        Assert.Equal(3, buildConfigParam.AllowedValues.Count);
        Assert.Contains("Debug", buildConfigParam.AllowedValues);
        Assert.Contains("Release", buildConfigParam.AllowedValues);
        Assert.Contains("CI", buildConfigParam.AllowedValues);

        var outputDirParam = parameters.First(p => p.Name == "outputDirectory");
        Assert.Equal("string", outputDirParam.Type);
        Assert.Equal("Output Directory", outputDirParam.DisplayName);
        Assert.Equal("$(Build.ArtifactStagingDirectory)", outputDirParam.DefaultValue);
        Assert.Null(outputDirParam.AllowedValues);

        var buildSettingsParam = parameters.First(p => p.Name == "buildSettings");
        Assert.Equal("object", buildSettingsParam.Type);
        Assert.Equal("Build Settings", buildSettingsParam.DisplayName);
        Assert.NotNull(buildSettingsParam.DefaultValue);
        Assert.IsType<Dictionary<object, object>>(buildSettingsParam.DefaultValue);
        var settingsDict = (Dictionary<object, object>)buildSettingsParam.DefaultValue!;
        Assert.Empty(settingsDict);
        Assert.Null(buildSettingsParam.AllowedValues);
    }

    private static YamlMappingNode LoadPipeline(string path) => YamlUtils.LoadPipelineFile(path);
}