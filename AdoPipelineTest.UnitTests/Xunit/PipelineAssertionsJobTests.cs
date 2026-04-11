using AdoPipelineTest.Model;
using NUnit.Framework;
using NUnitAssert = NUnit.Framework.Assert;
using Assert = AdoPipelineTest.Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Xunit;

public class PipelineAssertionsJobTests
{
    [Test]
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

        NUnitAssert.DoesNotThrow(() => Assert.HasJob(result, "Build", "BuildJob"));
    }

    [Test]
    public void HasJob_WithStageAndJob_WhenStageMissing_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() { DisplayName = "Build" } }
        };

        NUnitAssert.That(() => Assert.HasJob(result, "Deploy", "Job"), Throws.Exception);
    }

    [Test]
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

        NUnitAssert.That(() => Assert.HasJob(result, "Build", "TestJob"), Throws.Exception);
    }

    [Test]
    public void HasJob_OnStage_WhenJobExists_DoesNotThrow()
    {
        var stage = new PipelineStage
        {
            DisplayName = "Build",
            Jobs = new List<PipelineJob> { new() { DisplayName = "BuildJob" } }
        };

        NUnitAssert.DoesNotThrow(() => Assert.HasJob(stage, "BuildJob"));
    }

    [Test]
    public void HasJob_OnStage_WhenJobMissing_Throws()
    {
        var stage = new PipelineStage
        {
            DisplayName = "Build",
            Jobs = new List<PipelineJob> { new() { DisplayName = "BuildJob" } }
        };

        NUnitAssert.That(() => Assert.HasJob(stage, "TestJob"), Throws.Exception);
    }

    [Test]
    public void JobCount_WhenCountMatches_DoesNotThrow()
    {
        var stage = new PipelineStage
        {
            Jobs = new List<PipelineJob> { new(), new(), new() }
        };

        NUnitAssert.DoesNotThrow(() => Assert.JobCount(stage, 3));
    }

    [Test]
    public void JobCount_WhenCountDiffers_Throws()
    {
        var stage = new PipelineStage
        {
            Jobs = new List<PipelineJob> { new() }
        };

        NUnitAssert.That(() => Assert.JobCount(stage, 2), Throws.Exception);
    }
}
