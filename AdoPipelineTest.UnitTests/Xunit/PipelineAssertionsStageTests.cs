using AdoPipelineTest.Model;
using Xunit;
using PipelineAssert = AdoPipelineTest.Xunit.Assert;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Xunit;

public class PipelineAssertionsStageTests
{
    [Fact]
    public void HasStage_WhenStageExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() { DisplayName = "Build" } }
        };

        var ex = Record.Exception(() => PipelineAssert.HasStage(result, "Build"));
        Assert.Null(ex);
    }

    [Fact]
    public void HasStage_WhenStageDoesNotExist_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() { DisplayName = "Build" } }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasStage(result, "Deploy"));
    }

    [Fact]
    public void StageCount_WhenCountMatches_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new(), new() }
        };

        var ex = Record.Exception(() => PipelineAssert.StageCount(result, 2));
        Assert.Null(ex);
    }

    [Fact]
    public void StageCount_WhenCountDiffers_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.StageCount(result, 2));
    }
}