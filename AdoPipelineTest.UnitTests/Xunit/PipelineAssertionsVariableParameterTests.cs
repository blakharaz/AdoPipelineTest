using NUnit.Framework;
using AdoPipelineTest.Model;
using AdoPipelineTest.Xunit;
using NUnitAssert = NUnit.Framework.Assert;
using PipelineAssert = AdoPipelineTest.Xunit.Assert;

namespace AdoPipelineTest.UnitTests.XunitHelpers;

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

        NUnitAssert.DoesNotThrow(() => PipelineAssert.HasParameter(result, "env"));
    }

    [Test]
    public void HasParameter_WhenParameterDoesNotExist_Throws()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>()
        };

        NUnitAssert.That(() => PipelineAssert.HasParameter(result, "env"), Throws.Exception);
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

        NUnitAssert.DoesNotThrow(() => PipelineAssert.ParameterHasValue(result, "env", "prod"));
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

        NUnitAssert.That(() => PipelineAssert.ParameterHasValue(result, "env", "dev"), Throws.Exception);
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

        NUnitAssert.DoesNotThrow(() => PipelineAssert.HasVariable(result, "buildConfig"));
    }

    [Test]
    public void HasVariable_WhenVariableDoesNotExist_Throws()
    {
        var result = new PipelineTestResult
        {
            Variables = new List<PipelineVariable>()
        };

        NUnitAssert.That(() => PipelineAssert.HasVariable(result, "buildConfig"), Throws.Exception);
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

        NUnitAssert.DoesNotThrow(() => PipelineAssert.HasVariable(result, "buildConfig", "Release"));
    }
}
