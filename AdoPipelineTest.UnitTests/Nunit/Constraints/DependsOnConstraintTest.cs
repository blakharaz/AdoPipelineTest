using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class DependsOnConstraintTest
{
    [Test]
    public void ApplyTo_PipelineStage_HasDependency_ReturnsSuccess()
    {
        var stage = new PipelineStage
        {
            DependsOn = ["Build", "Test"]
        };
        
        var constraint = new DependsOnConstraint("Build");
        var result = constraint.ApplyTo(stage);
        
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineStage_DependencyNotFound_ReturnsFailure()
    {
        var stage = new PipelineStage
        {
            DependsOn = ["Build"]
        };
        
        var constraint = new DependsOnConstraint("Deploy");
        var result = constraint.ApplyTo(stage);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineStage_EmptyDependsOn_ReturnsFailure()
    {
        var stage = new PipelineStage { DependsOn = [] };
        
        var constraint = new DependsOnConstraint("Build");
        var result = constraint.ApplyTo(stage);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_PipelineJob_HasDependency_ReturnsSuccess()
    {
        var job = new PipelineJob
        {
            DependsOn = ["Compile"]
        };
        
        var constraint = new DependsOnConstraint("Compile");
        var result = constraint.ApplyTo(job);
        
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_PipelineJob_DependencyNotFound_ReturnsFailure()
    {
        var job = new PipelineJob
        {
            DependsOn = ["Compile"]
        };
        
        var constraint = new DependsOnConstraint("Test");
        var result = constraint.ApplyTo(job);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_NotStageOrJob_ReturnsFailure()
    {
        var constraint = new DependsOnConstraint("Build");
        var result = constraint.ApplyTo("not a stage or job");
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new DependsOnConstraint("Build");
        Assert.That(constraint.Description, Is.EqualTo("Depends on 'Build'"));
    }
}
