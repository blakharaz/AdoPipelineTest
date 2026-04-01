using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class HasVariableConstraintTest
{
    [Test]
    public void ApplyTo_PipelineTestResult_HasVariable_ReturnsSuccess()
    {
        var result = new PipelineTestResult
        {
            Variables = [new PipelineVariable { Name = "buildConfiguration" }]
        };
        
        var constraint = new HasVariableConstraint("buildConfiguration");
        var res = constraint.ApplyTo(result);
        
        Assert.That(res.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineTestResult_VariableNotFound_ReturnsFailure()
    {
        var result = new PipelineTestResult
        {
            Variables = [new PipelineVariable { Name = "buildConfiguration" }]
        };
        
        var constraint = new HasVariableConstraint("otherVar");
        var res = constraint.ApplyTo(result);
        
        Assert.That(res.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineTestResult_EmptyVariables_ReturnsFailure()
    {
        var result = new PipelineTestResult { Variables = [] };
        
        var constraint = new HasVariableConstraint("buildConfiguration");
        var res = constraint.ApplyTo(result);
        
        Assert.That(res.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_NotPipelineTestResult_ReturnsFailure()
    {
        var constraint = new HasVariableConstraint("buildConfiguration");
        var res = constraint.ApplyTo("not a result");
        
        Assert.That(res.IsSuccess, Is.False);
    }

    [Test]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new HasVariableConstraint("buildConfiguration");
        Assert.That(constraint.Description, Is.EqualTo("Pipeline has variable 'buildConfiguration'"));
    }
}
