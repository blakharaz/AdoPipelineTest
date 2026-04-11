using AdoPipelineTest.Model;
using NUnit.Framework;
using NUnitAssert = NUnit.Framework.Assert;
using Assert = AdoPipelineTest.Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Xunit;

public class PipelineAssertionsVariableParameterTests
{
    [Test]
    public void HasParameter_WhenParameterExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>
            {
                { "env", new PipelineParameter { Name = "env", Value = "prod" } }
            }
        };

        NUnitAssert.DoesNotThrow(() => Assert.HasParameter(result, "env"));
    }

    [Test]
    public void HasParameter_WhenParameterDoesNotExist_Throws()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>()
        };

        NUnitAssert.That(() => Assert.HasParameter(result, "env"), Throws.Exception);
    }

    [Test]
    public void ParameterHasValue_WhenValueMatches_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>
            {
                { "env", new PipelineParameter { Name = "env", Value = "prod" } }
            }
        };

        NUnitAssert.DoesNotThrow(() => Assert.ParameterHasValue(result, "env", "prod"));
    }

    [Test]
    public void ParameterHasValue_WhenValueDiffers_Throws()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>
            {
                { "env", new PipelineParameter { Name = "env", Value = "prod" } }
            }
        };

        NUnitAssert.That(() => Assert.ParameterHasValue(result, "env", "dev"), Throws.Exception);
    }

    [Test]
    public void HasVariable_WhenVariableExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Variables = new List<PipelineVariable>
            {
                new() { Name = "buildConfig", DefaultValue = "Release" }
            }
        };

        NUnitAssert.DoesNotThrow(() => Assert.HasVariable(result, "buildConfig"));
    }

    [Test]
    public void HasVariable_WhenVariableDoesNotExist_Throws()
    {
        var result = new PipelineTestResult
        {
            Variables = new List<PipelineVariable>()
        };

        NUnitAssert.That(() => Assert.HasVariable(result, "buildConfig"), Throws.Exception);
    }

    [Test]
    public void HasVariable_WithValueMatches_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Variables = new List<PipelineVariable>
            {
                new() { Name = "buildConfig", DefaultValue = "Release" }
            }
        };

        NUnitAssert.DoesNotThrow(() => Assert.HasVariable(result, "buildConfig", "Release"));
    }
}
