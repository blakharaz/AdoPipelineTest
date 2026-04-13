using AdoPipelineTest.Model;
using Xunit;
using PipelineAssert = AdoPipelineTest.Xunit.Assert;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Xunit;

public class PipelineAssertionsJobTests
{
    [Fact]
    public void HasJob_WithStageAndJob_WhenJobExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    DisplayName = "Build",
                    Jobs = new List<PipelineJob> { new() { DisplayName = "BuildJob" } }
                }
            }
        };

        var ex = Record.Exception(() => PipelineAssert.HasJob(result, "Build", "BuildJob"));
        Assert.Null(ex);
    }

    [Fact]
    public void HasJob_WithStageAndJob_WhenStageMissing_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() { DisplayName = "Build" } }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasJob(result, "Deploy", "Job"));
    }

    [Fact]
    public void HasJob_WithStageAndJob_WhenJobMissing_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage>
            {
                new()
                {
                    DisplayName = "Build",
                    Jobs = new List<PipelineJob> { new() { DisplayName = "BuildJob" } }
                }
            }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasJob(result, "Build", "TestJob"));
    }

    [Fact]
    public void HasJob_OnStage_WhenJobExists_DoesNotThrow()
    {
        var stage = new PipelineStage
        {
            DisplayName = "Build",
            Jobs = new List<PipelineJob> { new() { DisplayName = "BuildJob" } }
        };

        var ex = Record.Exception(() => PipelineAssert.HasJob(stage, "BuildJob"));
        Assert.Null(ex);
    }

    [Fact]
    public void HasJob_OnStage_WhenJobMissing_Throws()
    {
        var stage = new PipelineStage
        {
            DisplayName = "Build",
            Jobs = new List<PipelineJob> { new() { DisplayName = "BuildJob" } }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasJob(stage, "TestJob"));
    }

    [Fact]
    public void JobCount_WhenCountMatches_DoesNotThrow()
    {
        var stage = new PipelineStage
        {
            Jobs = new List<PipelineJob> { new(), new(), new() }
        };

        var ex = Record.Exception(() => PipelineAssert.JobCount(stage, 3));
        Assert.Null(ex);
    }

    [Fact]
    public void JobCount_WhenCountDiffers_Throws()
    {
        var stage = new PipelineStage
        {
            Jobs = new List<PipelineJob> { new() }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.JobCount(stage, 2));
    }
}