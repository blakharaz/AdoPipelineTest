using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.PipelineAssertions;
using FluentAssertions;
using Xunit;

namespace AdoPipelineTest.UnitTests.FluentAssertions;

public class PipelineJobAssertionsTests
{
    [Fact]
    public void HaveStepCount_WithCorrectCount_ShouldNotThrow()
    {
        var job = new PipelineJob
        {
            Steps = new List<PipelineStep>
            {
                new TaskStep { TaskName = "A" },
                new TaskStep { TaskName = "B" }
            }
        };

        var act = () => job.Should().HaveStepCount(2);
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveStepCount_WithWrongCount_ShouldThrow()
    {
        var job = new PipelineJob
        {
            Steps = new List<PipelineStep>
            {
                new TaskStep { TaskName = "A" }
            }
        };

        var act = () => job.Should().HaveStepCount(2);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void HaveStepCount_WithZeroSteps_ShouldNotThrow()
    {
        var job = new PipelineJob
        {
            Steps = new List<PipelineStep>()
        };

        var act = () => job.Should().HaveStepCount(0);
        act.Should().NotThrow();
    }

    [Fact]
    public void HaveStepCount_WithMoreSteps_ShouldThrow()
    {
        var job = new PipelineJob
        {
            Steps = new List<PipelineStep>
            {
                new TaskStep { TaskName = "A" },
                new TaskStep { TaskName = "B" },
                new TaskStep { TaskName = "C" }
            }
        };

        var act = () => job.Should().HaveStepCount(1);
        act.Should().Throw<Exception>();
    }
}