using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;
using Xunit;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasResourceConstraintTest
{
    [Fact]
    public void ApplyTo_PipelineTestResult_HasResource_ReturnsSuccess()
    {
        var result = new PipelineTestResult
        {
            Resources = [new PipelineResource { Type = "repositories", Name = "myRepo" }]
        };
        
        var constraint = new HasResourceConstraint("repositories");
        var res = constraint.ApplyTo(result);
        
        Assert.True(res.IsSuccess);
    }

    [Fact]
    public void ApplyTo_PipelineTestResult_ResourceNotFound_ReturnsFailure()
    {
        var result = new PipelineTestResult
        {
            Resources = [new PipelineResource { Type = "repositories", Name = "myRepo" }]
        };
        
        var constraint = new HasResourceConstraint("pipelines");
        var res = constraint.ApplyTo(result);
        
        Assert.False(res.IsSuccess);
    }

    [Fact]
    public void ApplyTo_PipelineTestResult_EmptyResources_ReturnsFailure()
    {
        var result = new PipelineTestResult { Resources = [] };
        
        var constraint = new HasResourceConstraint("repositories");
        var res = constraint.ApplyTo(result);
        
        Assert.False(res.IsSuccess);
    }

    [Fact]
    public void ApplyTo_NotPipelineTestResult_ReturnsFailure()
    {
        var constraint = new HasResourceConstraint("repositories");
        var res = constraint.ApplyTo("not a result");
        
        Assert.False(res.IsSuccess);
    }

    [Fact]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasResourceConstraint("repositories");
        Assert.Equal("Pipeline has resource of type 'repositories'", constraint.Description);
    }
}
