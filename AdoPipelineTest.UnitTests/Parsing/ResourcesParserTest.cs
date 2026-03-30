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

    [Test]
    public void ParseResourcesWithRepositoryGroup()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_repository_group.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(2));

        var repo1 = pipeline.Resources.FirstOrDefault(r => r.Name == "myGitRepo");
        Assert.That(repo1, Is.Not.Null);
        Assert.That(repo1!.Type, Is.EqualTo("git"));

        var repo2 = pipeline.Resources.FirstOrDefault(r => r.Name == "anotherRepo");
        Assert.That(repo2, Is.Not.Null);
        Assert.That(repo2!.Type, Is.EqualTo("git"));
    }

    [Test]
    public void ParseResourcesWithPipelineGroup()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_pipeline_group.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(2));

        var buildPipeline = pipeline.Resources.FirstOrDefault(r => r.Name == "myPipeline");
        Assert.That(buildPipeline, Is.Not.Null);
        Assert.That(buildPipeline!.Trigger, Is.Not.Null);
        Assert.That(buildPipeline!.Trigger, Contains.Item("main"));

        var releasePipeline = pipeline.Resources.FirstOrDefault(r => r.Name == "releasePipeline");
        Assert.That(releasePipeline, Is.Not.Null);
    }

    [Test]
    public void ParseResourcesWithContainerGroup()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_container_group.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(1));

        var container = pipeline.Resources[0];
        Assert.That(container.Name, Is.EqualTo("myContainer"));
        Assert.That(container.Source, Is.Null);
    }

    [Test]
    public void ParseResourcesWithPackageGroup()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_package_group.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(1));

        var package = pipeline.Resources[0];
        Assert.That(package.Name, Is.EqualTo("myPackage"));
        Assert.That(package.Source, Is.Null);
    }

    [Test]
    public void ParseResourceWithEndpointAuth()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_endpoint_auth.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(1));

        var resource = pipeline.Resources[0];
        Assert.That(resource.Endpoints, Is.Not.Null);
        Assert.That(resource.Endpoints!.Count, Is.EqualTo(1));

        var endpoint = resource.Endpoints[0];
        Assert.That(endpoint.Name, Is.EqualTo("docker-auth"));
        Assert.That(endpoint.Value, Is.EqualTo("secret-token"));
        Assert.That(endpoint.Auth, Is.Not.Null);
        Assert.That(endpoint.Auth!.ContainsKey("username"), Is.True);
        Assert.That(endpoint.Auth["username"], Is.EqualTo("dockeruser"));
        Assert.That(endpoint.Auth["password"], Is.EqualTo("dockerpassword"));
    }

    [Test]
    public void ParseResourceWithEndpointNoValue()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_endpoint_no_value.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(1));

        var resource = pipeline.Resources[0];
        Assert.That(resource.Endpoints, Is.Not.Null);
        Assert.That(resource.Endpoints!.Count, Is.EqualTo(1));

        var endpoint = resource.Endpoints[0];
        Assert.That(endpoint.Name, Is.EqualTo("docker-auth"));
        Assert.That(endpoint.Value, Is.Null);
        Assert.That(endpoint.Auth, Is.Not.Null);
    }

    [Test]
    public void ParseResourceWithNestedAuth()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_nested_auth.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(1));

        var resource = pipeline.Resources[0];
        var endpoint = resource.Endpoints![0];
        Assert.That(endpoint.Auth, Is.Not.Null);
        Assert.That(endpoint.Auth!.ContainsKey("config"), Is.True);

        var config = endpoint.Auth["config"] as Dictionary<string, object?>;
        Assert.That(config, Is.Not.Null);
        Assert.That(config!.ContainsKey("username"), Is.True);
        Assert.That(config["username"], Is.EqualTo("user1"));

        var extras = config["extras"] as List<object?>;
        Assert.That(extras, Is.Not.Null);
        Assert.That(extras!.Count, Is.EqualTo(2));
    }

    [Test]
    public void ParseResourceWithNullTriggerItems()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_null_trigger.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(1));

        var resource = pipeline.Resources[0];
        Assert.That(resource.Trigger, Is.Not.Null);
        Assert.That(resource.Trigger!.Count, Is.EqualTo(2));
        Assert.That(resource.Trigger, Contains.Item("main"));
        Assert.That(resource.Trigger, Contains.Item("develop"));
    }

    [Test]
    public void ParseResourceWithNullTriggerScalar()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_null_trigger_scalar.yaml");

        Assert.That(pipeline.Resources, Has.Count.EqualTo(1));

        var resource = pipeline.Resources[0];
        Assert.That(resource.Trigger, Is.Null);
    }
}