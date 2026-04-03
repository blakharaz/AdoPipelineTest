using NUnit.Framework;
using AdoPipelineTest.Parsing.Ast;
using AdoPipelineTest.Parsing.Expressions;
using Sprache;

namespace AdoPipelineTest.UnitTests.Parsing.Expressions;

[TestFixture]
public partial class BoolExpressionGrammarTest
{
    [Test]
    public void NaryFunctions_And_Simple()
    {
        var result = BoolExpressionGrammar.NaryFunctions.Parse("and(true, false, true)");
        var result2 = BoolExpressionGrammar.Functions.Parse("and(true, false, true)");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.InstanceOf<FunctionExpression>());
            Assert.That(result2, Is.InstanceOf<FunctionExpression>());
        }
    }

    [Test]
    public void NaryFunctions_And_NestedBinary()
    {
        var result = BoolExpressionGrammar.NaryFunctions.Parse("and(eq(true, false), false)");
        var result2 = BoolExpressionGrammar.Functions.Parse("and(eq(true, false), false)");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.InstanceOf<FunctionExpression>());
            Assert.That(result2, Is.InstanceOf<FunctionExpression>());
        }
    }

    [Test]
    public void NaryFunctions_And_NestedNary()
    {
        var result = BoolExpressionGrammar.NaryFunctions.Parse("and(succeeded(), not(eq(stage, 'Prod')))");
        var result2 = BoolExpressionGrammar.Functions.Parse("and(succeeded(), not(eq(stage, 'Prod')))");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.InstanceOf<FunctionExpression>());
            Assert.That(result2, Is.InstanceOf<FunctionExpression>());
        }
    }

    [Test]
    public void NaryFunctions_And_ParameterUsage()
    {
        var result = BoolExpressionGrammar.NaryFunctions.Parse("and(false, parameters.Foo)");
        var result2 = BoolExpressionGrammar.NaryFunctions.Parse("and(parameters.Foo, false)");
        var result3 = BoolExpressionGrammar.Functions.Parse("and(false, parameters.Foo)");
        var result4 = BoolExpressionGrammar.Functions.Parse("and(parameters.Foo, false)");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.InstanceOf<FunctionExpression>());
            Assert.That(result2, Is.InstanceOf<FunctionExpression>());
            Assert.That(result3, Is.InstanceOf<FunctionExpression>());
            Assert.That(result4, Is.InstanceOf<FunctionExpression>());
        }
    }
    
    [Test]
    public void NaryFunctions_And_VariableUsage()
    {
        var result = BoolExpressionGrammar.NaryFunctions.Parse("and(false, variables.Bar)");
        var result2 = BoolExpressionGrammar.NaryFunctions.Parse("and(variables.Bar, false)");
        var result3 = BoolExpressionGrammar.Functions.Parse("and(false, variables.Bar)");
        var result4 = BoolExpressionGrammar.Functions.Parse("and(variables.Bar, false)");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.InstanceOf<FunctionExpression>());
            Assert.That(result2, Is.InstanceOf<FunctionExpression>());
            Assert.That(result3, Is.InstanceOf<FunctionExpression>());
            Assert.That(result4, Is.InstanceOf<FunctionExpression>());
        }
    }
    
    [Test]
    public void NaryFunctions_Succeeded_Simple()
    {
        var result = BoolExpressionGrammar.NaryFunctions.Parse("succeeded('A', 'B', 'C')");
        var result2 = BoolExpressionGrammar.Functions.Parse("succeeded('A', 'B', 'C')");
        var result3 = BoolExpressionGrammar.NaryFunctions.Parse("succeeded()");
        var result4 = BoolExpressionGrammar.Functions.Parse("succeeded()");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.InstanceOf<FunctionExpression>());
            Assert.That(result2, Is.InstanceOf<FunctionExpression>());
            Assert.That(result3, Is.InstanceOf<FunctionExpression>());
            Assert.That(result4, Is.InstanceOf<FunctionExpression>());
        }
    }
}