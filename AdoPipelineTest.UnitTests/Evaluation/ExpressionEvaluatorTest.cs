using AdoPipelineTest.Evaluation;

namespace AdoPipelineTest.UnitTests.Evaluation;

[TestFixture]
public class ExpressionEvaluatorTest
{
    [Test]
    public void EvaluateBoolean_EvaluatesBooleanLiterals()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ExpressionEvaluator.EvaluateBool("true", true), Is.True);
            Assert.That(ExpressionEvaluator.EvaluateBool("true", false), Is.True);
            Assert.That(ExpressionEvaluator.EvaluateBool("false", true), Is.False);
            Assert.That(ExpressionEvaluator.EvaluateBool("false", false), Is.False);
        }
    }
    
    [Test]
    public void EvaluateBoolean_UsesDefaultValuesForNullString()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ExpressionEvaluator.EvaluateBool(null, true), Is.True);
            Assert.That(ExpressionEvaluator.EvaluateBool(null, false), Is.False);
        }
    }

    [Test]
    public void EvaluateVariables_ReplacesVariableExpressions()
    {
        const string stringWithVariables = "hello $(foo) $(bar) world";
        var variables = new Dictionary<string, object>{["foo"] = "to", ["bar"] = "the"};
        
        Assert.That(ExpressionEvaluator.EvaluateVariables(stringWithVariables, variables), Is.EqualTo("hello to the world"));
    }
}