using AdoPipelineTest.Model;
using AdoPipelineTest.PipelineAssertions;
using FluentAssertions;
using Xunit;

namespace AdoPipelineTest.UnitTests.FluentAssertions;

public class PipelineAgentPoolAssertionsTests
{
    [Fact]
    public void HaveVmImage_WithCorrectImage_ShouldNotThrow()
    {
        var pool = new PipelineAgentPool { VmImage = "ubuntu-latest" };

        var act = () => pool.Should().HaveVmImage("ubuntu-latest");
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveVmImage_WithWrongImage_ShouldThrow()
    {
        var pool = new PipelineAgentPool { VmImage = "ubuntu-latest" };

        var act = () => pool.Should().HaveVmImage("windows-latest");
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void HaveVmImage_WithNullImage_ShouldThrow()
    {
        var pool = new PipelineAgentPool { VmImage = null };

        var act = () => pool.Should().HaveVmImage("ubuntu-latest");
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void HaveVmImage_WithNullPool_ShouldThrow()
    {
        PipelineAgentPool pool = null!;

        var act = () => pool.Should().HaveVmImage("ubuntu-latest");
        act.Should().Throw<Exception>();
    }
}