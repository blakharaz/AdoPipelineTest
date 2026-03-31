using AdoPipelineTest.Parsing;

namespace AdoPipelineTest.UnitTests.Parsing;

[TestFixture]
public class VariablesParserTest
{
    [Test]
    public void ParseVariablesWithStringDefaults()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_variables.yaml");

        Assert.That(pipeline.Variables, Has.Count.EqualTo(3));

        // Verify each variable
        var buildConfigVar = pipeline.Variables.FirstOrDefault(v => v.Name == "buildConfiguration");
        Assert.That(buildConfigVar, Is.Not.Null);
        Assert.That(buildConfigVar!.DefaultValue, Is.EqualTo("Release"));

        var debugSymbolsVar = pipeline.Variables.FirstOrDefault(v => v.Name == "debugSymbols");
        Assert.That(debugSymbolsVar, Is.Not.Null);
        Assert.That(debugSymbolsVar!.DefaultValue, Is.EqualTo("true"));

        var dotnetVersionVar = pipeline.Variables.FirstOrDefault(v => v.Name == "dotnetVersion");
        Assert.That(dotnetVersionVar, Is.Not.Null);
        Assert.That(dotnetVersionVar!.DefaultValue, Is.EqualTo("8.0.x"));
    }

    [Test]
    public void PipelineWithNoVariables()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/simple_pipeline_just_steps.yaml");

        Assert.That(pipeline.Variables, Is.Empty);
    }

    [Test]
    public void ParseVariablesWithComplexDefaults()
    {
        var pipeline = PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_complex_variables.yaml");

        Assert.That(pipeline.Variables, Has.Count.EqualTo(4));

        // Verify simple string
        var simpleStringVar = pipeline.Variables.FirstOrDefault(v => v.Name == "simpleString");
        Assert.That(simpleStringVar, Is.Not.Null);
        Assert.That(simpleStringVar!.DefaultValue, Is.EqualTo("hello"));

        // Verify simple number (as string in YAML)
        var simpleNumberVar = pipeline.Variables.FirstOrDefault(v => v.Name == "simpleNumber");
        Assert.That(simpleNumberVar, Is.Not.Null);
        Assert.That(simpleNumberVar!.DefaultValue, Is.EqualTo("42"));

        // Verify mapping/object default
        var buildConfigVar = pipeline.Variables.FirstOrDefault(v => v.Name == "buildConfig");
        Assert.That(buildConfigVar, Is.Not.Null);
        Assert.That(buildConfigVar!.DefaultValue, Is.TypeOf<Dictionary<string, object?>>());
        var buildConfigDict = (Dictionary<string, object?>)buildConfigVar!.DefaultValue!;
        Assert.That(buildConfigDict, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(buildConfigDict["debug"], Is.EqualTo("true"));
            Assert.That(buildConfigDict["release"], Is.EqualTo("false"));
        }

        // Verify sequence default
        var frameworksVar = pipeline.Variables.FirstOrDefault(v => v.Name == "frameworks");
        Assert.That(frameworksVar, Is.Not.Null);
        Assert.That(frameworksVar!.DefaultValue, Is.TypeOf<List<object?>>());
        var frameworksList = (List<object?>)frameworksVar!.DefaultValue!;
        Assert.That(frameworksList, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(frameworksList[0], Is.EqualTo("net6.0"));
            Assert.That(frameworksList[1], Is.EqualTo("net8.0"));
        }
    }
}
