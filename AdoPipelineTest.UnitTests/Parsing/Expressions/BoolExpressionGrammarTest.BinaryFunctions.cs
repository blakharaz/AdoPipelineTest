using NUnit.Framework;
using AdoPipelineTest.Parsing.Ast;
using AdoPipelineTest.Parsing.Expressions;
using Sprache;

namespace AdoPipelineTest.UnitTests.Parsing.Expressions;

[TestFixture]
public partial class BoolExpressionGrammarTest
{
    [Test]
    public void BinaryFunctions_Eq_Simple()
    {
        var result = BoolExpressionGrammar.BinaryFunctions.Parse("eq(true, false)");
        Assert.That(result, Is.InstanceOf<FunctionExpression>());
    }

    [Test]
    public void BinaryFunctions_Eq_Nested()
    {
        var result = BoolExpressionGrammar.BinaryFunctions.Parse("eq(eq(true, false), false)");
        Assert.That(result, Is.InstanceOf<FunctionExpression>());
    }
    
    [Test]
    public void BinaryFunctions_Eq_ParameterUsage()
    {
        var result = BoolExpressionGrammar.BinaryFunctions.Parse("eq(false, parameters.Foo)");
        Assert.That(result, Is.InstanceOf<FunctionExpression>());

        var result2 = BoolExpressionGrammar.BinaryFunctions.Parse("eq(parameters.Foo, false)");
        Assert.That(result2, Is.InstanceOf<FunctionExpression>());
    }
    
    [Test]
    public void BinaryFunctions_Eq_VariableUsage()
    {
        var result = BoolExpressionGrammar.BinaryFunctions.Parse("eq(false, variables.Bar)");
        Assert.That(result, Is.InstanceOf<FunctionExpression>());

        var result2 = BoolExpressionGrammar.BinaryFunctions.Parse("eq(variables.Bar, false)");
        Assert.That(result2, Is.InstanceOf<FunctionExpression>());
    }
}