using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasParameterConstraintTest
{
    [Test]
    public void ApplyTo_PipelineTestResult_HasParameter_ReturnsSuccess()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>
            {
                ["environment"] = new PipelineParameter { Name = "environment", Value = "prod" }
            }
        };
        
        var constraint = new HasParameterConstraint("environment");
        var res = constraint.ApplyTo(result);
        
        Assert.That(res.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineTestResult_ParameterNotFound_ReturnsFailure()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>
            {
                ["environment"] = new PipelineParameter { Name = "environment", Value = "prod" }
            }
        };
        
        var constraint = new HasParameterConstraint("region");
        var res = constraint.ApplyTo(result);
        
        Assert.That(res.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineTestResult_EmptyParameters_ReturnsFailure()
    {
        var result = new PipelineTestResult
        {
            Parameters = new Dictionary<string, PipelineParameter>()
        };
        
        var constraint = new HasParameterConstraint("environment");
        var res = constraint.ApplyTo(result);
        
        Assert.That(res.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_NotPipelineTestResult_ReturnsFailure()
    {
        var constraint = new HasParameterConstraint("environment");
        var res = constraint.ApplyTo("not a result");
        
        Assert.That(res.IsSuccess, Is.False);
    }

    [Test]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasParameterConstraint("environment");
        Assert.That(constraint.Description, Is.EqualTo("Pipeline has parameter 'environment'"));
    }
}
