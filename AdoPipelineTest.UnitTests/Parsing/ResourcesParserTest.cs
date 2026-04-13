using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;
using Xunit;

namespace AdoPipelineTest.UnitTests.Parsing;

public class ResourcesParserTest
{
    [Fact]
    public void ParseResourcesWithSimpleData()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_simple_resources.yaml");

        Assert.Equal(2, pipeline.Resources.Count);

        var dockerRegistry = pipeline.Resources.FirstOrDefault(r => r.Name == "docker-registry");
        Assert.NotNull(dockerRegistry);
        Assert.Equal("docker-registry", dockerRegistry.Name);
        Assert.Equal("docker-registry", dockerRegistry.Type);
        Assert.Equal("https://index.docker.io/v1/", dockerRegistry.Source);

        var nugetFeed = pipeline.Resources.FirstOrDefault(r => r.Name == "nuget-feed");
        Assert.NotNull(nugetFeed);
        Assert.Equal("nuget-feed", nugetFeed.Name);
        Assert.Equal("nuget-feed", nugetFeed.Type);
        Assert.Equal("https://api.nuget.org/v3/index.json", nugetFeed.Source);
    }

    [Fact]
    public void PipelineWithNoResources()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_without_resources.yaml");

        Assert.Empty(pipeline.Resources);
    }

    [Fact]
    public void ParseResourceWithVersionAndTrigger()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_versioned_resource.yaml");

        Assert.Single(pipeline.Resources);

        var resource = pipeline.Resources[0];
        Assert.NotNull(resource);
        
        Assert.Equal("docker-registry", resource.Name);
        Assert.Equal("v2", resource.Version);

        Assert.NotNull(resource.Trigger);

        Assert.Equal(3, resource.Trigger.Count);
        Assert.True(resource.Trigger.Contains("main"));
        Assert.True(resource.Trigger.Contains("develop"));
        Assert.True(resource.Trigger.Contains("release/*"));
    }

    [Fact]
    public void ParseResourceWithEndpoints()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_endpoints.yaml");

        Assert.Single(pipeline.Resources);

        var resource = pipeline.Resources[0];
        Assert.Equal("docker-registry", resource.Name);

        Assert.NotNull(resource.Endpoints);
        Assert.NotEmpty(resource.Endpoints);
    }

    [Fact]
    public void ParseResourceWithMissingFields()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_partial_resource.yaml");

        Assert.Single(pipeline.Resources);

        var resource = pipeline.Resources[0];
        Assert.NotNull(resource.Name);
        Assert.NotEmpty(resource.Name);
    }

    [Fact]
    public void ParseResourcesWithRepositoryGroup()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_repository_group.yaml");

        Assert.Equal(2, pipeline.Resources.Count);

        var repo1 = pipeline.Resources.FirstOrDefault(r => r.Name == "myGitRepo");
        Assert.NotNull(repo1);
        Assert.Equal("git", repo1!.Type);

        var repo2 = pipeline.Resources.FirstOrDefault(r => r.Name == "anotherRepo");
        Assert.NotNull(repo2);
        Assert.Equal("git", repo2!.Type);
    }

    [Fact]
    public void ParseResourcesWithPipelineGroup()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_pipeline_group.yaml");

        Assert.Equal(2, pipeline.Resources.Count);

        var buildPipeline = pipeline.Resources.FirstOrDefault(r => r.Name == "myPipeline");
        Assert.NotNull(buildPipeline);

        Assert.NotNull(buildPipeline.Trigger);
        Assert.Contains("main", buildPipeline.Trigger);

        var releasePipeline = pipeline.Resources.FirstOrDefault(r => r.Name == "releasePipeline");
        Assert.NotNull(releasePipeline);
    }

    [Fact]
    public void ParseResourcesWithContainerGroup()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_container_group.yaml");

        Assert.Single(pipeline.Resources);

        var container = pipeline.Resources[0];
        Assert.Equal("myContainer", container.Name);
        Assert.Null(container.Source);
    }

    [Fact]
    public void ParseResourcesWithPackageGroup()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_package_group.yaml");

        Assert.Single(pipeline.Resources);

        var package = pipeline.Resources[0];
        Assert.Equal("myPackage", package.Name);
        Assert.Null(package.Source);
    }

    [Fact]
    public void ParseResourceWithEndpointAuth()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_endpoint_auth.yaml");

        Assert.Single(pipeline.Resources);

        var resource = pipeline.Resources[0];
        Assert.NotNull(resource.Endpoints);
        Assert.Single(resource.Endpoints);

        var endpoint = resource.Endpoints[0];
        Assert.Equal("docker-auth", endpoint.Name);
        Assert.Equal("secret-token", endpoint.Value);
        Assert.NotNull(endpoint.Auth);
        Assert.True(endpoint.Auth!.ContainsKey("username"));
        Assert.Equal("dockeruser", endpoint.Auth["username"]);
        Assert.Equal("dockerpassword", endpoint.Auth["password"]);
    }

    [Fact]
    public void ParseResourceWithEndpointNoValue()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_endpoint_no_value.yaml");

        Assert.Single(pipeline.Resources);

        var resource = pipeline.Resources[0];
        Assert.NotNull(resource.Endpoints);
        Assert.Single(resource.Endpoints!);

        var endpoint = resource.Endpoints[0];
        Assert.Equal("docker-auth", endpoint.Name);
        Assert.Null(endpoint.Value);
        Assert.NotNull(endpoint.Auth);
    }

    [Fact]
    public void ParseResourceWithNestedAuth()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_nested_auth.yaml");

        Assert.Single(pipeline.Resources);

        var resource = pipeline.Resources[0];
        var endpoint = resource.Endpoints![0];
        Assert.NotNull(endpoint.Auth);
        Assert.True(endpoint.Auth!.ContainsKey("config"));

        var config = endpoint.Auth["config"] as Dictionary<string, object?>;
        Assert.NotNull(config);
        Assert.True(config!.ContainsKey("username"));
        Assert.Equal("user1", config["username"]);

        var extras = config["extras"] as List<object?>;
        Assert.NotNull(extras);
        Assert.Equal(2, extras!.Count);
    }

    [Fact]
    public void ParseResourceWithNullTriggerItems()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_null_trigger.yaml");

        Assert.Single(pipeline.Resources);

        var resource = pipeline.Resources[0];
        Assert.NotNull(resource.Trigger);
        Assert.Equal(2, resource.Trigger!.Count);
        Assert.Contains("main", resource.Trigger);
        Assert.Contains("develop", resource.Trigger);
    }

    [Fact]
    public void ParseResourceWithNullTriggerScalar()
    {
        var pipeline = PipelineParser.Parse("test_data/resources/pipeline_with_null_trigger_scalar.yaml");

        Assert.Single(pipeline.Resources);

        var resource = pipeline.Resources[0];
        Assert.Null(resource.Trigger);
    }
}
