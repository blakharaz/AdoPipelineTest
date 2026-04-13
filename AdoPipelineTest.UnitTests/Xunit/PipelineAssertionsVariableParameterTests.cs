using AdoPipelineTest.Model;
using Xunit;
using PipelineAssert = AdoPipelineTest.Xunit.Assert;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Xunit;

public class PipelineAssertionsVariableParameterTests
{
    [Fact]
    public void HasParameter_WhenParameterExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>
            {
                { "env", new PipelineParameter { Name = "env", Value = "prod" } }
            }
        };

        var ex = Record.Exception(() => PipelineAssert.HasParameter(result, "env"));
        Assert.Null(ex);
    }

    [Fact]
    public void HasParameter_WhenParameterDoesNotExist_Throws()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>()
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasParameter(result, "env"));
    }

    [Fact]
    public void ParameterHasValue_WhenValueMatches_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>
            {
                { "env", new PipelineParameter { Name = "env", Value = "prod" } }
            }
        };

        var ex = Record.Exception(() => PipelineAssert.ParameterHasValue(result, "env", "prod"));
        Assert.Null(ex);
    }

    [Fact]
    public void ParameterHasValue_WhenValueDiffers_Throws()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>
            {
                { "env", new PipelineParameter { Name = "env", Value = "prod" } }
            }
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.ParameterHasValue(result, "env", "dev"));
    }

    [Fact]
    public void HasVariable_WhenVariableExists_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Variables = new List<PipelineVariable>
            {
                new() { Name = "buildConfig", DefaultValue = "Release" }
            }
        };

        var ex = Record.Exception(() => PipelineAssert.HasVariable(result, "buildConfig"));
        Assert.Null(ex);
    }

    [Fact]
    public void HasVariable_WhenVariableDoesNotExist_Throws()
    {
        var result = new PipelineTestResult
        {
            Variables = new List<PipelineVariable>()
        };

        Assert.ThrowsAny<Exception>(() => PipelineAssert.HasVariable(result, "buildConfig"));
    }

    [Fact]
    public void HasVariable_WithValueMatches_DoesNotThrow()
    {
        var result = new PipelineTestResult
        {
            Variables = new List<PipelineVariable>
            {
                new() { Name = "buildConfig", DefaultValue = "Release" }
            }
        };

        var ex = Record.Exception(() => PipelineAssert.HasVariable(result, "buildConfig", "Release"));
        Assert.Null(ex);
    }
}