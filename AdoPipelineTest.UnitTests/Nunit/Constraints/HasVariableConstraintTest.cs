using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;
using Xunit;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasVariableConstraintTest
{
    [Fact]
    public void ApplyTo_PipelineTestResult_HasVariable_ReturnsSuccess()
    {
        var result = new PipelineTestResult
        {
            Variables = [new PipelineVariable { Name = "buildConfiguration" }]
        };
        
        var constraint = new HasVariableConstraint("buildConfiguration");
        var res = constraint.ApplyTo(result);
        
        Assert.True(res.IsSuccess);
    }

    [Fact]
    public void ApplyTo_PipelineTestResult_VariableNotFound_ReturnsFailure()
    {
        var result = new PipelineTestResult
        {
            Variables = [new PipelineVariable { Name = "buildConfiguration" }]
        };
        
        var constraint = new HasVariableConstraint("otherVar");
        var res = constraint.ApplyTo(result);
        
        Assert.False(res.IsSuccess);
    }

    [Fact]
    public void ApplyTo_PipelineTestResult_EmptyVariables_ReturnsFailure()
    {
        var result = new PipelineTestResult { Variables = [] };
        
        var constraint = new HasVariableConstraint("buildConfiguration");
        var res = constraint.ApplyTo(result);
        
        Assert.False(res.IsSuccess);
    }

    [Fact]
    public void ApplyTo_NotPipelineTestResult_ReturnsFailure()
    {
        var constraint = new HasVariableConstraint("buildConfiguration");
        var res = constraint.ApplyTo("not a result");
        
        Assert.False(res.IsSuccess);
    }

    [Fact]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasVariableConstraint("buildConfiguration");
        Assert.Equal("Pipeline has variable 'buildConfiguration'", constraint.Description);
    }
}
