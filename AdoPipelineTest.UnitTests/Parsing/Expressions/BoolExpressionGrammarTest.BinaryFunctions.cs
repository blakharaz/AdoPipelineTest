using Xunit;
using AdoPipelineTest.Parsing.Ast;
using AdoPipelineTest.Parsing.Expressions;
using Sprache;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Parsing.Expressions;

public partial class BoolExpressionGrammarTest
{
    [Fact]
    public void BinaryFunctions_Eq_Simple()
    {
        var result = BoolExpressionGrammar.BinaryFunctions.Parse("eq(true, false)");
        Assert.IsType<FunctionExpression>(result);
    }

    [Fact]
    public void BinaryFunctions_Eq_Nested()
    {
        var result = BoolExpressionGrammar.BinaryFunctions.Parse("eq(eq(true, false), false)");
        Assert.IsType<FunctionExpression>(result);
    }
    
    [Fact]
    public void BinaryFunctions_Eq_ParameterUsage()
    {
        var result = BoolExpressionGrammar.BinaryFunctions.Parse("eq(false, parameters.Foo)");
        Assert.IsType<FunctionExpression>(result);

        var result2 = BoolExpressionGrammar.BinaryFunctions.Parse("eq(parameters.Foo, false)");
        Assert.IsType<FunctionExpression>(result2);
    }
    
    [Fact]
    public void BinaryFunctions_Eq_VariableUsage()
    {
        var result = BoolExpressionGrammar.BinaryFunctions.Parse("eq(false, variables.Bar)");
        Assert.IsType<FunctionExpression>(result);

        var result2 = BoolExpressionGrammar.BinaryFunctions.Parse("eq(variables.Bar, false)");
        Assert.IsType<FunctionExpression>(result2);
    }
}