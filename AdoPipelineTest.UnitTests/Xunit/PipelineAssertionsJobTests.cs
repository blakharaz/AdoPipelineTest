using AdoPipelineTest.Model;
using AdoPipelineTest.Xunit;
using NUnitAssert = NUnit.Framework.Assert;
using PipelineAssert = AdoPipelineTest.Xunit.Assert;

namespace AdoPipelineTest.UnitTests.XunitHelpers;

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

        NUnitAssert.DoesNotThrow(() => PipelineAssert.HasJob(result, "Build", "BuildJob"));
    }

    [Test]
    public void HasJob_WithStageAndJob_WhenStageMissing_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() { DisplayName = "Build" } }
        };

        NUnitAssert.That(() => PipelineAssert.HasJob(result, "Deploy", "Job"), Throws.Exception);
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

        NUnitAssert.That(() => PipelineAssert.HasJob(result, "Build", "TestJob"), Throws.Exception);
    }

    [Test]
    public void HasJob_OnStage_WhenJobExists_DoesNotThrow()
    {
        var stage = new PipelineStage
        {
            DisplayName = "Build",
            Jobs = new List<PipelineJob> { new() { DisplayName = "BuildJob" } }
        };

        NUnitAssert.DoesNotThrow(() => PipelineAssert.HasJob(stage, "BuildJob"));
    }

    [Test]
    public void HasJob_OnStage_WhenJobMissing_Throws()
    {
        var stage = new PipelineStage
        {
            DisplayName = "Build",
            Jobs = new List<PipelineJob> { new() { DisplayName = "BuildJob" } }
        };

        NUnitAssert.That(() => PipelineAssert.HasJob(stage, "TestJob"), Throws.Exception);
    }

    [Test]
    public void JobCount_WhenCountMatches_DoesNotThrow()
    {
        var stage = new PipelineStage
        {
            Jobs = new List<PipelineJob> { new(), new(), new() }
        };

        NUnitAssert.DoesNotThrow(() => PipelineAssert.JobCount(stage, 3));
    }

    [Test]
    public void JobCount_WhenCountDiffers_Throws()
    {
        var stage = new PipelineStage
        {
            Jobs = new List<PipelineJob> { new() }
        };

        NUnitAssert.That(() => PipelineAssert.JobCount(stage, 2), Throws.Exception);
    }
}
