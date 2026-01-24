using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.Samples.Nunit.TemplateExpressions;

[TestFixture]
public class IfElseConditionalInsertion
{
    private const string YamlPath = "Nunit/TemplateExpressions/ifelse_conditional_step_insertion.yml";
    
    [Test]
    public void TestConditionalInsertionSteps_ConditionMsBuild()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "one")
            .Run();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Stages, Has.Count.EqualTo(1));
            Assert.That(result.Stages[0].Jobs, Has.Count.EqualTo(1));
        }

        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Has.Count.EqualTo(1));

        var firstStep = steps[0] as TaskStep;
        Assert.That(firstStep, Is.Not.Null);
        Assert.That(firstStep.TaskName, Is.EqualTo("VSBuild@1"));
    }
    
    [Test]
    public void TestConditionalInsertionSteps_ConditionDotnet()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "two")
            .Run();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Stages, Has.Count.EqualTo(1));
            Assert.That(result.Stages[0].Jobs, Has.Count.EqualTo(1));
        }

        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Has.Count.EqualTo(3));

        var firstStep = steps[0] as TaskStep;
        Assert.That(firstStep, Is.Not.Null);
        Assert.That(firstStep.TaskName, Is.EqualTo("UseDotNet@2"));

        var secondStep = steps[1] as TaskStep;
        Assert.That(secondStep, Is.Not.Null);
        Assert.That(secondStep.TaskName, Is.EqualTo("UseDotNet@2"));

        var thirdStep = steps[2] as TaskStep;
        Assert.That(thirdStep, Is.Not.Null);
        Assert.That(thirdStep.TaskName, Is.EqualTo("UseDotNet@2"));
    }

    [Test]
    public void TestConditionalInsertionSteps_ConditionNothing()
    {
        var result = new PipelineTester()
            .WithPipeline(YamlPath)
            .WithParameter("option", "three")
            .Run();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Stages, Has.Count.EqualTo(1));
            Assert.That(result.Stages[0].Jobs, Has.Count.EqualTo(1));
        }

        var steps = result.Stages[0].Jobs[0].Steps;
        Assert.That(steps, Has.Count.EqualTo(1));
    }
}