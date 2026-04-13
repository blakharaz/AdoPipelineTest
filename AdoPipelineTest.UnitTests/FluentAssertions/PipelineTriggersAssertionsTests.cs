using AdoPipelineTest.Model;
using AdoPipelineTest.PipelineAssertions;
using FluentAssertions;
using Xunit;

namespace AdoPipelineTest.UnitTests.FluentAssertions;

public class PipelineTriggersAssertionsTests
{
    [Fact]
    public void HaveTrigger_WhenTriggersNull_ShouldThrow()
    {
        var result = new PipelineTestResult();

        var act = () => result.Should().HaveTrigger();
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void HaveTriggers_WhenTriggersDefined_ShouldNotThrow()
    {
        var result = new PipelineTestResult
        {
            Triggers = new PipelineTriggers()
        };

        var act = () => result.Should().HaveTriggers();
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveTrigger_WhenTriggersDefined_ShouldNotThrow()
    {
        var result = new PipelineTestResult
        {
            Triggers = new PipelineTriggers()
        };

        var act = () => result.Should().HaveTrigger();
        act.Should().NotThrow();
    }
}