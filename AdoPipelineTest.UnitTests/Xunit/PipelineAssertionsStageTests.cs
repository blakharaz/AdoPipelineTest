using AdoPipelineTest.Model;
using NUnit.Framework;
using NUnitAssert = NUnit.Framework.Assert;
using Assert = AdoPipelineTest.Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Xunit;

public class PipelineAssertionsStageTests
{
    [Test]
    public void HasStage_WhenStageExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() { DisplayName = "Build" } }
        };

        NUnitAssert.DoesNotThrow(() => Assert.HasStage(result, "Build"));
    }

    [Test]
    public void HasStage_WhenStageDoesNotExist_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() { DisplayName = "Build" } }
        };

        NUnitAssert.That(() => Assert.HasStage(result, "Deploy"), Throws.Exception);
    }

    [Test]
    public void StageCount_WhenCountMatches_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new(), new() }
        };

        NUnitAssert.DoesNotThrow(() => Assert.StageCount(result, 2));
    }

    [Test]
    public void StageCount_WhenCountDiffers_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() }
        };

        NUnitAssert.That(() => Assert.StageCount(result, 2), Throws.Exception);
    }
}
