using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest.UnitTests.Parsing;

[TestFixture]
public class TemplateExpressionParserTest
{
    [Test]
    public void ParseExpression_WithUnterminatedString_ThrowsInvalidPipelineException()
    {
        var parser = new TemplateExpressionParser("\"unterminated");
        
        var ex = Assert.Throws<InvalidPipelineException>(() => parser.ParseExpression());
        
        Assert.That(ex?.Message, Does.Contain("Unterminated string"));
    }

    [Test]
    public void ParseExpression_WithUnterminatedSingleQuoteString_ThrowsInvalidPipelineException()
    {
        var parser = new TemplateExpressionParser("'unterminated");
        
        var ex = Assert.Throws<InvalidPipelineException>(() => parser.ParseExpression());
        
        Assert.That(ex?.Message, Does.Contain("Unterminated string"));
    }

    [Test]
    public void ParseExpression_WithTerminatedString_ReturnsStringLiteral()
    {
        var parser = new TemplateExpressionParser("\"hello\"");
        
        var result = parser.ParseExpression();
        
        Assert.That(result, Is.InstanceOf<StringLiteral>());
        Assert.That(((StringLiteral)result).Value, Is.EqualTo("hello"));
    }

    [Test]
    public void ParseExpression_WithEscapedQuoteInString_ReturnsStringLiteralWithEscapedChar()
    {
        var parser = new TemplateExpressionParser("\"hello\\\"world\"");
        
        var result = parser.ParseExpression();
        
        Assert.That(result, Is.InstanceOf<StringLiteral>());
        Assert.That(((StringLiteral)result).Value, Is.EqualTo("hello\"world"));
    }

    [Test]
    public void ParseExpression_WithParameterReference_ReturnsParameterExpression()
    {
        var parser = new TemplateExpressionParser("parameters.foo");
        
        var result = parser.ParseExpression();
        
        Assert.That(result, Is.InstanceOf<ParameterExpression>());
        Assert.That(((ParameterExpression)result).ParameterName, Is.EqualTo("foo"));
    }

    [Test]
    public void ParseExpression_WithVariableReference_ReturnsVariableExpression()
    {
        var parser = new TemplateExpressionParser("variables.bar");
        
        var result = parser.ParseExpression();
        
        Assert.That(result, Is.InstanceOf<VariableExpression>());
        Assert.That(((VariableExpression)result).Name, Is.EqualTo("bar"));
    }

    [Test]
    public void ParseExpression_WithFunctionCall_ReturnsFunctionExpression()
    {
        var parser = new TemplateExpressionParser("upper('hello')");
        
        var result = parser.ParseExpression();
        
        Assert.That(result, Is.InstanceOf<FunctionExpression>());
        var funcExpr = (FunctionExpression)result;
        Assert.That(funcExpr.FunctionName, Is.EqualTo("upper"));
        Assert.That(funcExpr.FunctionParameters, Has.Count.EqualTo(1));
        Assert.That(funcExpr.FunctionParameters[0], Is.InstanceOf<StringLiteral>());
    }

    [Test]
    public void ParseExpression_WithEmptyString_ReturnsEmptyStringLiteral()
    {
        var parser = new TemplateExpressionParser("\"\"");
        
        var result = parser.ParseExpression();
        
        Assert.That(result, Is.InstanceOf<StringLiteral>());
        Assert.That(((StringLiteral)result).Value, Is.EqualTo(""));
    }
}

