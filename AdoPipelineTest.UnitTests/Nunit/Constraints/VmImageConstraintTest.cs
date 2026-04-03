using NUnit.Framework;
using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class VmImageConstraintTest
{
    [Test]
    public void ApplyTo_AgentPool_MatchingVmImage_ReturnsSuccess()
    {
        var agentPool = new PipelineAgentPool
        {
            VmImage = "ubuntu-latest"
        };
        
        var constraint = new VmImageConstraint("ubuntu-latest");
        var result = constraint.ApplyTo(agentPool);
        
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void ApplyTo_AgentPool_DifferentVmImage_ReturnsFailure()
    {
        var agentPool = new PipelineAgentPool
        {
            VmImage = "windows-latest"
        };
        
        var constraint = new VmImageConstraint("ubuntu-latest");
        var result = constraint.ApplyTo(agentPool);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_AgentPool_NullVmImage_ReturnsFailure()
    {
        var agentPool = new PipelineAgentPool
        {
            VmImage = null!
        };
        
        var constraint = new VmImageConstraint("ubuntu-latest");
        var result = constraint.ApplyTo(agentPool);
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void ApplyTo_NotAgentPool_ReturnsFailure()
    {
        var constraint = new VmImageConstraint("ubuntu-latest");
        var result = constraint.ApplyTo("not an agent pool");
        
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new VmImageConstraint("ubuntu-latest");
        Assert.That(constraint.Description, Is.EqualTo("VM Image is ubuntu-latest"));
    }

    [TestCase("ubuntu-22.04")]
    [TestCase("ubuntu-20.04")]
    [TestCase("windows-2022")]
    [TestCase("windows-latest")]
    [TestCase("macOS-14")]
    public void ApplyTo_VariousVmImages_HandlesCorrectly(string vmImage)
    {
        var agentPool = new PipelineAgentPool
        {
            VmImage = vmImage
        };
        
        var constraint = new VmImageConstraint(vmImage);
        var result = constraint.ApplyTo(agentPool);
        
        Assert.That(result.IsSuccess, Is.True);
    }
}
