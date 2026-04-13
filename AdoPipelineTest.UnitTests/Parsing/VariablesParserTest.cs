using AdoPipelineTest.Parsing;
using Xunit;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Parsing;

public class VariablesParserTest
{
    [Fact]
    public void ParseVariablesWithStringDefaults()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_variables.yaml");

        Assert.Equal(3, pipeline.Variables.Count);

        var buildConfigVar = pipeline.Variables.FirstOrDefault(v => v.Name == "buildConfiguration");
        Assert.NotNull(buildConfigVar);
        Assert.Equal("Release", buildConfigVar!.DefaultValue);

        var debugSymbolsVar = pipeline.Variables.FirstOrDefault(v => v.Name == "debugSymbols");
        Assert.NotNull(debugSymbolsVar);
        Assert.Equal("true", debugSymbolsVar!.DefaultValue);

        var dotnetVersionVar = pipeline.Variables.FirstOrDefault(v => v.Name == "dotnetVersion");
        Assert.NotNull(dotnetVersionVar);
        Assert.Equal("8.0.x", dotnetVersionVar!.DefaultValue);
    }

    [Fact]
    public void PipelineWithNoVariables()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/simple_pipeline_just_steps.yaml");

        Assert.Empty(pipeline.Variables);
    }

    [Fact]
    public void ParseVariablesWithComplexDefaults()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_complex_variables.yaml");

        Assert.Equal(4, pipeline.Variables.Count);

        var simpleStringVar = pipeline.Variables.FirstOrDefault(v => v.Name == "simpleString");
        Assert.NotNull(simpleStringVar);
        Assert.Equal("hello", simpleStringVar!.DefaultValue);

        var simpleNumberVar = pipeline.Variables.FirstOrDefault(v => v.Name == "simpleNumber");
        Assert.NotNull(simpleNumberVar);
        Assert.Equal("42", simpleNumberVar!.DefaultValue);

        var buildConfigVar = pipeline.Variables.FirstOrDefault(v => v.Name == "buildConfig");
        Assert.NotNull(buildConfigVar);
        Assert.IsType<Dictionary<string, object?>>(buildConfigVar!.DefaultValue);
        var buildConfigDict = (Dictionary<string, object?>)buildConfigVar!.DefaultValue!;
        Assert.Equal(2, buildConfigDict.Count);
        Assert.Equal("true", buildConfigDict["debug"]);
        Assert.Equal("false", buildConfigDict["release"]);

        var frameworksVar = pipeline.Variables.FirstOrDefault(v => v.Name == "frameworks");
        Assert.NotNull(frameworksVar);
        Assert.IsType<List<object?>>(frameworksVar!.DefaultValue);
        var frameworksList = (List<object?>)frameworksVar!.DefaultValue!;
        Assert.Equal(2, frameworksList.Count);
        Assert.Equal("net6.0", frameworksList[0]);
        Assert.Equal("net8.0", frameworksList[1]);
    }
}