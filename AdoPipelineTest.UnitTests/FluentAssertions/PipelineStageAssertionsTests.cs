using AdoPipelineTest.Model;
using AdoPipelineTest.PipelineAssertions;
using FluentAssertions;
using Xunit;

namespace AdoPipelineTest.UnitTests.FluentAssertions;

public class PipelineStageAssertionsTests
{
    [Fact]
    public void HaveJob_WithExistingJobByName_ShouldNotThrow()
    {
        var stage = new PipelineStage
        {
            Name = "BuildStage",
            Jobs = new List<PipelineJob> { new() { Name = "BuildJob" } }
        };

        var act = () => stage.Should().HaveJob("BuildJob");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveJob_WithExistingJobByDisplayName_ShouldNotThrow()
    {
        var stage = new PipelineStage
        {
            DisplayName = "Build Stage",
            Jobs = new List<PipelineJob> { new() { DisplayName = "Build Job" } }
        };

        var act = () => stage.Should().HaveJob("Build Job");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveJob_WithNonExistingJob_ShouldThrow()
    {
        var stage = new PipelineStage
        {
            Jobs = new List<PipelineJob> { new() { Name = "ExistingJob" } }
        };

        var act = () => stage.Should().HaveJob("NonExistent");
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void HaveJobCount_WithCorrectCount_ShouldNotThrow()
    {
        var stage = new PipelineStage
        {
            Jobs = new List<PipelineJob> { new(), new() }
        };

        var act = () => stage.Should().HaveJobCount(2);
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveJobCount_WithWrongCount_ShouldThrow()
    {
        var stage = new PipelineStage
        {
            Jobs = new List<PipelineJob> { new() }
        };

        var act = () => stage.Should().HaveJobCount(2);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void HaveJobCount_WithZeroJobs_ShouldNotThrow()
    {
        var stage = new PipelineStage
        {
            Jobs = new List<PipelineJob>()
        };

        var act = () => stage.Should().HaveJobCount(0);
        act.Should().NotThrow();
    }
}