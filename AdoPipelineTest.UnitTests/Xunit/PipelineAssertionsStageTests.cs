using AdoPipelineTest.Model;
using AdoPipelineTest.Xunit;
using NUnitAssert = NUnit.Framework.Assert;
using PipelineAssert = AdoPipelineTest.Xunit.Assert;

namespace AdoPipelineTest.UnitTests.XunitHelpers;

public class PipelineAssertionsStageTests
{
    [Test]
    public void HasStage_WhenStageExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() { DisplayName = "Build" } }
        };

        NUnitAssert.DoesNotThrow(() => PipelineAssert.HasStage(result, "Build"));
    }

    [Test]
    public void HasStage_WhenStageDoesNotExist_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() { DisplayName = "Build" } }
        };

        NUnitAssert.That(() => PipelineAssert.HasStage(result, "Deploy"), Throws.Exception);
    }

    [Test]
    public void StageCount_WhenCountMatches_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new(), new() }
        };

        NUnitAssert.DoesNotThrow(() => PipelineAssert.StageCount(result, 2));
    }

    [Test]
    public void StageCount_WhenCountDiffers_Throws()
    {
        var result = new PipelineTestResult
        {
            Stages = new List<PipelineStage> { new() }
        };

        NUnitAssert.That(() => PipelineAssert.StageCount(result, 2), Throws.Exception);
    }
}
