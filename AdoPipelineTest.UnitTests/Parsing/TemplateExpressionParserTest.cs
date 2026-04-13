using Xunit;
using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.Ast;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Parsing;

public class TemplateExpressionParserTest
{
    [Fact]
    public void ParseExpression_WithUnterminatedString_ThrowsInvalidPipelineException()
    {
        var parser = new TemplateExpressionParser("\"unterminated");
        
        var ex = Assert.Throws<InvalidPipelineException>(() => parser.ParseExpression());
        
        Assert.Contains("Unterminated string", ex?.Message);
    }

    [Fact]
    public void ParseExpression_WithUnterminatedSingleQuoteString_ThrowsInvalidPipelineException()
    {
        var parser = new TemplateExpressionParser("'unterminated");
        
        var ex = Assert.Throws<InvalidPipelineException>(() => parser.ParseExpression());
        
        Assert.Contains("Unterminated string", ex?.Message);
    }

    [Fact]
    public void ParseExpression_WithTerminatedString_ReturnsStringLiteral()
    {
        var parser = new TemplateExpressionParser("\"hello\"");
        
        var result = parser.ParseExpression();
        
        Assert.IsType<StringLiteral>(result);
        Assert.Equal("hello", ((StringLiteral)result).Value);
    }

    [Fact]
    public void ParseExpression_WithEscapedQuoteInString_ReturnsStringLiteralWithEscapedChar()
    {
        var parser = new TemplateExpressionParser("\"hello\\\"world\"");
        
        var result = parser.ParseExpression();
        
        Assert.IsType<StringLiteral>(result);
        Assert.Equal("hello\"world", ((StringLiteral)result).Value);
    }

    [Fact]
    public void ParseExpression_WithParameterReference_ReturnsParameterExpression()
    {
        var parser = new TemplateExpressionParser("parameters.foo");
        
        var result = parser.ParseExpression();
        
        Assert.IsType<ParameterExpression>(result);
        Assert.Equal("foo", ((ParameterExpression)result).ParameterName);
    }

    [Fact]
    public void ParseExpression_WithVariableReference_ReturnsVariableExpression()
    {
        var parser = new TemplateExpressionParser("variables.bar");
        
        var result = parser.ParseExpression();
        
        Assert.IsType<VariableExpression>(result);
        Assert.Equal("bar", ((VariableExpression)result).Name);
    }

    [Fact]
    public void ParseExpression_WithFunctionCall_ReturnsFunctionExpression()
    {
        var parser = new TemplateExpressionParser("upper('hello')");
        
        var result = parser.ParseExpression();
        
        Assert.IsType<FunctionExpression>(result);
        var funcExpr = (FunctionExpression)result;
        
        Assert.Equal("upper", funcExpr.FunctionName);
        Assert.Equal(1, funcExpr.FunctionParameters.Count);
        Assert.IsType<StringLiteral>(funcExpr.FunctionParameters[0]);
    }

    [Fact]
    public void ParseExpression_WithEmptyString_ReturnsEmptyStringLiteral()
    {
        var parser = new TemplateExpressionParser("\"\"");
        
        var result = parser.ParseExpression();
        
        Assert.IsType<StringLiteral>(result);
        Assert.Equal("", ((StringLiteral)result).Value);
    }
}