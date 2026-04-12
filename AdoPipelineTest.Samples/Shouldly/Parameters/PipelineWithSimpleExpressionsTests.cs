using AdoPipelineTest.Model.Steps;
using AdoPipelineTest.Shouldly;
using NUnit.Framework;
using Shouldly;

namespace AdoPipelineTest.Samples.Shouldly.Parameters;

[TestFixture]
public class PipelineWithSimpleExpressionsTests
{
    private const string YamlPath = "pipelines/Parameters/pipeline_with_simple_expressions.yaml";

    [Test]
    public void VerifyPipelineStructure()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt")
            .Run();

        result.ShouldNotBeNull();
        result.Parameters.Count.ShouldBe(7);
        result.Stages.Count.ShouldBe(1);
        var job = result.Stages[0].Jobs[0];
        job.DisplayName.ShouldBe("Build and Test Job");
        job.Steps.Count.ShouldBe(4);
    }

    [Test]
    public void VerifyStepInputsWithParameters()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("targetFile", "myfile.txt")
            .WithParameter("buildConfiguration", "Debug")
            .Run();

        var steps = result.Stages[0].Jobs[0].Steps;
        
        var buildStep = steps[1].ShouldBeOfType<TaskStep>();
        buildStep.TaskName.ShouldBe("DotNetCoreCLI@2");
        buildStep.Inputs["arguments"].ShouldBe("--configuration Debug");

        var summaryStep = steps[3].ShouldBeOfType<ScriptStep>();
        summaryStep.Script.ShouldContain("Configuration: Debug");
    }
}
