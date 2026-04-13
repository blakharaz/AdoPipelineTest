using AdoPipelineTest.Model;
using AdoPipelineTest.Nunit.Constraints;
using Xunit;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Nunit.Constraints;

public class VmImageConstraintTest
{
    [Fact]
    public void ApplyTo_AgentPool_MatchingVmImage_ReturnsSuccess()
    {
        var agentPool = new PipelineAgentPool
        {
            VmImage = "ubuntu-latest"
        };
        
        var constraint = new VmImageConstraint("ubuntu-latest");
        var result = constraint.ApplyTo(agentPool);
        
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_AgentPool_DifferentVmImage_ReturnsFailure()
    {
        var agentPool = new PipelineAgentPool
        {
            VmImage = "windows-latest"
        };
        
        var constraint = new VmImageConstraint("ubuntu-latest");
        var result = constraint.ApplyTo(agentPool);
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_AgentPool_NullVmImage_ReturnsFailure()
    {
        var agentPool = new PipelineAgentPool
        {
            VmImage = null!
        };
        
        var constraint = new VmImageConstraint("ubuntu-latest");
        var result = constraint.ApplyTo(agentPool);
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ApplyTo_NotAgentPool_ReturnsFailure()
    {
        var constraint = new VmImageConstraint("ubuntu-latest");
        var result = constraint.ApplyTo("not an agent pool");
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Description_ReturnsCorrectMessage()
    {
        var constraint = new VmImageConstraint("ubuntu-latest");
        Assert.Equal("VM Image is ubuntu-latest", constraint.Description);
    }

    [Theory]
    [InlineData("ubuntu-22.04")]
    [InlineData("ubuntu-20.04")]
    [InlineData("windows-2022")]
    [InlineData("windows-latest")]
    [InlineData("macOS-14")]
    public void ApplyTo_VariousVmImages_HandlesCorrectly(string vmImage)
    {
        var agentPool = new PipelineAgentPool
        {
            VmImage = vmImage
        };
        
        var constraint = new VmImageConstraint(vmImage);
        var result = constraint.ApplyTo(agentPool);
        
        Assert.True(result.IsSuccess);
    }
}
