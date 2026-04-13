using Xunit;
using System.Reflection.Metadata;
using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.Ast;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Parsing;

public class ExpressionParserTest
{
    [Fact]
    public void ParseStringExpression_WithPlainText_ReturnsSingleLiteralChild()
    {
        var result = ExpressionParser.ParseStringExpression("Hello world");

        Assert.NotNull(result);
        Assert.Equal(1, result.Children.Count);
        Assert.IsType<StringLiteral>(result.Children[0]);
    }

    [Fact]
    public void ParseStringExpression_WithSingleParameterReference_ReturnsParameterChild()
    {
        var result = ExpressionParser.ParseStringExpression("${{parameters.Foo}}");

        Assert.NotNull(result);
        Assert.Equal(1, result.Children.Count);
        Assert.IsType<TemplateExpression>(result.Children[0]);
    }

    [Fact]
    public void ParseStringExpression_WithLiteralAndParameter_ReturnsLiteralThenParameter()
    {
        var result = ExpressionParser.ParseStringExpression("Hello ${{parameters.Foo}}");

        Assert.NotNull(result);
        Assert.Equal(2, result.Children.Count);
        Assert.IsType<StringLiteral>(result.Children[0]);
        Assert.IsType<TemplateExpression>(result.Children[1]);
    }

    [Fact]
    public void ParseStringExpression_WithCompileTimeFunction_ReturnsTemplateExpression()
    {
        var result = ExpressionParser.ParseBoolExpression("${{ eq(parameters.Bar, \"foobar\") }}");

        Assert.NotNull(result);
        
        Assert.IsType<FunctionExpression>(result);
        var functionExpr = result as FunctionExpression;
        Assert.Equal("eq", functionExpr?.FunctionName);
        Assert.Equal(2, functionExpr?.FunctionParameters.Count);
        Assert.IsType<ParameterExpression>(functionExpr?.FunctionParameters[0]);
        Assert.IsType<StringLiteral>(functionExpr?.FunctionParameters[1]);
    }

    [Fact]
    public void ParseStringExpression_WithMultipleExpressions_ReturnsAllChildrenInOrder()
    {
        var result = ExpressionParser.ParseStringExpression("A ${{parameters.Foo}} B ${{ coalesce(parameters.Bar, \"foobar\") }} C ");

        Assert.NotNull(result);
        Assert.Equal(5, result.Children.Count);

        Assert.IsType<StringLiteral>(result.Children[0]);
        Assert.Equal("A ", (result.Children[0] as StringLiteral)?.Value);

        Assert.IsType<TemplateExpression>(result.Children[1]);

        Assert.IsType<StringLiteral>(result.Children[2]);
        Assert.Equal(" B ", (result.Children[2] as StringLiteral)?.Value);

        Assert.IsType<TemplateExpression>(result.Children[3]);

        Assert.IsType<StringLiteral>(result.Children[4]);
        Assert.Equal(" C ", (result.Children[4] as StringLiteral)?.Value);
    }
    
    [Fact]
    public void ParseStringExpression_WithEmptyString_ReturnsSingleLiteralChild()
    {
        var result = ExpressionParser.ParseStringExpression(string.Empty);

        Assert.NotNull(result);
        Assert.Equal(1, result.Children.Count);
        Assert.IsType<StringLiteral>(result.Children[0]);
        Assert.Equal("", (result.Children[0] as StringLiteral)?.Value);
    }

    [Fact]
    public void ParseStringExpression_WithWhitespaceInParameterExpression_StillParsesParameter()
    {
        var result = ExpressionParser.ParseStringExpression("${{  parameters.Foo   }}");

        Assert.NotNull(result);
        Assert.Equal(1, result.Children.Count);
        Assert.IsType<TemplateExpression>(result.Children[0]);

        var firstChild = result.Children[0] as TemplateExpression; 
        Assert.Equal(1, firstChild?.Children.Count);
        Assert.IsType<ParameterExpression>(firstChild?.Children[0]);
        Assert.IsType<ParameterExpression>(firstChild?.Children[0]);
        Assert.Equal("Foo", (firstChild?.Children[0] as ParameterExpression)?.ParameterName);
    }

    [Fact]
    public void ParseStringExpression_WithAdjacentParameterExpressions_ReturnsBothInOrder()
    {
        var result = ExpressionParser.ParseStringExpression("${{parameters.Foo}}${{parameters.Bar}}");

        Assert.NotNull(result);
        Assert.Equal(2, result.Children.Count);
  
        Assert.IsType<TemplateExpression>(result.Children[0]);
        var firstChild = result.Children[0] as TemplateExpression; 
        Assert.Equal(1, firstChild?.Children.Count);
        Assert.Equal("Foo", (firstChild?.Children[0] as ParameterExpression)?.ParameterName);

        Assert.IsType<TemplateExpression>(result.Children[1]);
        var secondChild = result.Children[1] as TemplateExpression; 
        Assert.Equal(1, secondChild?.Children.Count);
        Assert.Equal("Bar", (secondChild?.Children[0] as ParameterExpression)?.ParameterName);
    }

    [Fact]
    public void ParseStringExpression_WithUnterminatedStringInTemplateExpression_ThrowsInvalidPipelineException()
    {
        var ex = Assert.Throws<InvalidPipelineException>(
            () => ExpressionParser.ParseStringExpression("${{ \"unterminated }}")
        );

        Assert.Contains("Unexpected end of input reached; expected \"", ex?.Message);
    }
}