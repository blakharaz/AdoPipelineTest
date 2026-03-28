using AdoPipelineTest.Parsing;

namespace AdoPipelineTest.UnitTests.Parsing;

[TestFixture]
public class ResourcesParserTest
{
    [Test]
    public void ParseResourcesWithSimpleData()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_simple_resources.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(2));

        // Verify first resource - docker registry
        var dockerRegistry = pipeline.Resources.FirstOrDefault(r => r.Name == "docker-registry");
        Assert.That(dockerRegistry, Is.Not.Null);
        Assert.That(dockerRegistry!.Name, Is.EqualTo("docker-registry"));
        Assert.That(dockerRegistry!.Type, Is.EqualTo("docker-registry"));
        Assert.That(dockerRegistry!.Source, Is.EqualTo("https://index.docker.io/v1/"));

        // Verify second resource - nuget feed
        var nugetFeed = pipeline.Resources.FirstOrDefault(r => r.Name == "nuget-feed");
        Assert.That(nugetFeed, Is.Not.Null);
        Assert.That(nugetFeed!.Name, Is.EqualTo("nuget-feed"));
        Assert.That(nugetFeed!.Type, Is.EqualTo("nuget-feed"));
        Assert.That(nugetFeed!.Source, Is.EqualTo("https://api.nuget.org/v3/index.json"));
    }

    [Test]
    public void PipelineWithNoResources()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_without_resources.yaml");

        Assert.That(pipeline.Resources, Is.Empty);
    }

    [Test]
    public void ParseResourceWithVersionAndTrigger()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_versioned_resource.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(1));

        var resource = pipeline.Resources[0];
        Assert.That(resource.Name, Is.EqualTo("docker-registry"));
        Assert.That(resource.Version, Is.EqualTo("v2"));
        Assert.That(resource.Trigger, Is.Not.Null);
        Assert.That(resource.Trigger!.Count, Is.EqualTo(3));
        Assert.That(resource.Trigger!.Contains("main"), Is.True);
        Assert.That(resource.Trigger!.Contains("develop"), Is.True);
        Assert.That(resource.Trigger!.Contains("release/*"), Is.True);
    }

    [Test]
    public void ParseResourceWithEndpoints()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_endpoints.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(1));

        var resource = pipeline.Resources[0];
        Assert.That(resource.Name, Is.EqualTo("docker-registry"));
        
        // Verify endpoints exist
        Assert.That(resource.Endpoints, Is.Not.Null);
        Assert.That(resource.Endpoints!.Count, Is.GreaterThan(0));
    }

    [Test]
    public void ParseResourceWithMissingFields()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_partial_resource.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(1));

        var resource = pipeline.Resources[0];
        // Name should be present
        Assert.That(resource.Name, Is.Not.Null.And.Not.Empty);
    }
}