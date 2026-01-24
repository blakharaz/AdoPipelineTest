using AdoPipelineTest.Parsing;
using AdoPipelineTest.UnitTests.Utils;
using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.UnitTests.Parsing;

[TestFixture]
public class ParametersParserTest
{
    private const string PipelineNoParametersPath = "test_data/pipeline_parser/simple_pipeline_just_steps.yaml";
    private const string PipelineWithParametersPath = "test_data/pipeline_parser/pipeline_with_parameters.yaml";
    
    [Test]
    public void PipelineWithNoVariables()
    {
        var parameters = ParametersParser.ParseParameters(LoadPipeline(PipelineNoParametersPath));

        Assert.That(parameters, Is.Empty);
    }

    [Test]
    public void ParseParameters()
    {
        var parameters = ParametersParser.ParseParameters(LoadPipeline(PipelineWithParametersPath));

        Assert.That(parameters, Has.Count.EqualTo(6));

        // String parameter: projectName
        var projectNameParam = parameters.First(p => p.Name == "projectName");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(projectNameParam.Type, Is.EqualTo("string"));
            Assert.That(projectNameParam.DisplayName, Is.EqualTo("Project Name"));
            Assert.That(projectNameParam.DefaultValue, Is.EqualTo("MySampleProject"));
            Assert.That(projectNameParam.AllowedValues, Is.Null);
        }

        // Boolean parameter: enableTests
        var enableTestsParam = parameters.First(p => p.Name == "enableTests");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(enableTestsParam.Type, Is.EqualTo("boolean"));
            Assert.That(enableTestsParam.DisplayName, Is.EqualTo("Enable Unit Tests"));
            Assert.That(enableTestsParam.DefaultValue, Is.EqualTo(true));
            Assert.That(enableTestsParam.AllowedValues, Is.Null);
        }

        // Number parameter: timeoutMinutes
        var timeoutParam = parameters.First(p => p.Name == "timeoutMinutes");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(timeoutParam.Type, Is.EqualTo("number"));
            Assert.That(timeoutParam.DisplayName, Is.EqualTo("Timeout in Minutes"));
            Assert.That(timeoutParam.DefaultValue, Is.EqualTo(30));
            Assert.That(timeoutParam.AllowedValues, Is.Null);
        }

        // String with allowed values: buildConfiguration
        var buildConfigParam = parameters.First(p => p.Name == "buildConfiguration");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(buildConfigParam.Type, Is.EqualTo("string"));
            Assert.That(buildConfigParam.DisplayName, Is.EqualTo("Build Configuration"));
            Assert.That(buildConfigParam.DefaultValue, Is.EqualTo("Release"));
            Assert.That(buildConfigParam.AllowedValues, Is.Not.Null);
            Assert.That(buildConfigParam.AllowedValues, Has.Count.EqualTo(3));
            Assert.That(buildConfigParam.AllowedValues, Contains.Item("Debug"));
            Assert.That(buildConfigParam.AllowedValues, Contains.Item("Release"));
            Assert.That(buildConfigParam.AllowedValues, Contains.Item("CI"));
        }

        // String with environment variable: outputDirectory
        var outputDirParam = parameters.First(p => p.Name == "outputDirectory");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(outputDirParam.Type, Is.EqualTo("string"));
            Assert.That(outputDirParam.DisplayName, Is.EqualTo("Output Directory"));
            Assert.That(outputDirParam.DefaultValue, Is.EqualTo("$(Build.ArtifactStagingDirectory)"));
            Assert.That(outputDirParam.AllowedValues, Is.Null);
        }

        // Object parameter: buildSettings
        var buildSettingsParam = parameters.First(p => p.Name == "buildSettings");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(buildSettingsParam.Type, Is.EqualTo("object"));
            Assert.That(buildSettingsParam.DisplayName, Is.EqualTo("Build Settings"));
            Assert.That(buildSettingsParam.DefaultValue, Is.Not.Null);
            Assert.That(buildSettingsParam.DefaultValue, Is.InstanceOf<Dictionary<object, object>>());
            var settingsDict = (Dictionary<object, object>)buildSettingsParam.DefaultValue!;
            Assert.That(settingsDict, Is.Empty);
            Assert.That(buildSettingsParam.AllowedValues, Is.Null);
        }
    }

    private static YamlMappingNode LoadPipeline(string path) => YamlUtils.LoadPipelineFile(path);
}