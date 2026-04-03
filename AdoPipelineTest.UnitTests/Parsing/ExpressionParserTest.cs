using NUnit.Framework;
using System.Reflection.Metadata;
using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest.UnitTests.Parsing;

[TestFixture]
public class ExpressionParserTest
{
    [Test]
    public void ParseStringExpression_WithPlainText_ReturnsSingleLiteralChild()
    {
        var result = ExpressionParser.ParseStringExpression("Hello world");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Children, Has.Count.EqualTo(1));
            Assert.That(result.Children[0], Is.InstanceOf<StringLiteral>());
        }
    }

    [Test]
    public void ParseStringExpression_WithSingleParameterReference_ReturnsParameterChild()
    {
        var result = ExpressionParser.ParseStringExpression("${{parameters.Foo}}");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Children, Has.Count.EqualTo(1));
            Assert.That(result.Children[0], Is.InstanceOf<TemplateExpression>());
        }
    }

    [Test]
    public void ParseStringExpression_WithLiteralAndParameter_ReturnsLiteralThenParameter()
    {
        var result = ExpressionParser.ParseStringExpression("Hello ${{parameters.Foo}}");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Children, Has.Count.EqualTo(2));
            Assert.That(result.Children[0], Is.InstanceOf<StringLiteral>());
            Assert.That(result.Children[1], Is.InstanceOf<TemplateExpression>());
        }
    }

    [Test]
    public void ParseStringExpression_WithCompileTimeFunction_ReturnsTemplateExpression()
    {
        var result = ExpressionParser.ParseBoolExpression("${{ eq(parameters.Bar, \"foobar\") }}");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            
            Assert.That(result, Is.InstanceOf<FunctionExpression>());
            var functionExpr = result as FunctionExpression;
            Assert.That(functionExpr?.FunctionName, Is.EqualTo("eq"));
            Assert.That(functionExpr?.FunctionParameters, Has.Count.EqualTo(2));
            Assert.That(functionExpr?.FunctionParameters[0], Is.InstanceOf<ParameterExpression>());
            Assert.That(functionExpr?.FunctionParameters[1], Is.InstanceOf<StringLiteral>());
        }
    }

    [Test]
    public void ParseStringExpression_WithMultipleExpressions_ReturnsAllChildrenInOrder()
    {
        var result = ExpressionParser.ParseStringExpression("A ${{parameters.Foo}} B ${{ coalesce(parameters.Bar, \"foobar\") }} C ");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Children, Has.Count.EqualTo(5));

            Assert.That(result.Children[0], Is.InstanceOf<StringLiteral>());
            Assert.That((result.Children[0] as StringLiteral)?.Value, Is.EqualTo("A "));

            Assert.That(result.Children[1], Is.InstanceOf<TemplateExpression>());

            Assert.That(result.Children[2], Is.InstanceOf<StringLiteral>());
            Assert.That((result.Children[2] as StringLiteral)?.Value, Is.EqualTo(" B "));

            Assert.That(result.Children[3], Is.InstanceOf<TemplateExpression>());

            Assert.That(result.Children[4], Is.InstanceOf<StringLiteral>());
            Assert.That((result.Children[4] as StringLiteral)?.Value, Is.EqualTo(" C "));
        }
    }
    
    [Test]
    public void ParseStringExpression_WithEmptyString_ReturnsSingleLiteralChild()
    {
        var result = ExpressionParser.ParseStringExpression(string.Empty);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Children, Has.Count.EqualTo(1));
            Assert.That(result.Children[0], Is.InstanceOf<StringLiteral>());
            Assert.That((result.Children[0] as StringLiteral)?.Value, Is.Empty);
        }
    }

    [Test]
    public void ParseStringExpression_WithWhitespaceInParameterExpression_StillParsesParameter()
    {
        var result = ExpressionParser.ParseStringExpression("${{  parameters.Foo   }}");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Children, Has.Count.EqualTo(1));
            Assert.That(result.Children[0], Is.InstanceOf<TemplateExpression>());

            var firstChild = result.Children[0] as TemplateExpression; 
            Assert.That(firstChild?.Children, Has.Count.EqualTo(1));
            Assert.That(firstChild?.Children[0], Is.InstanceOf<ParameterExpression>());
            Assert.That(firstChild?.Children[0], Is.InstanceOf<ParameterExpression>());
            Assert.That((firstChild?.Children[0] as ParameterExpression)?.ParameterName, Is.EqualTo("Foo"));
        }
    }

    [Test]
    public void ParseStringExpression_WithAdjacentParameterExpressions_ReturnsBothInOrder()
    {
        var result = ExpressionParser.ParseStringExpression("${{parameters.Foo}}${{parameters.Bar}}");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Children, Has.Count.EqualTo(2));
  
            Assert.That(result.Children[0], Is.InstanceOf<TemplateExpression>());
            var firstChild = result.Children[0] as TemplateExpression; 
            Assert.That(firstChild?.Children, Has.Count.EqualTo(1));
            Assert.That((firstChild?.Children[0] as ParameterExpression)?.ParameterName, Is.EqualTo("Foo"));

            Assert.That(result.Children[1], Is.InstanceOf<TemplateExpression>());
            var secondChild = result.Children[1] as TemplateExpression; 
            Assert.That(secondChild?.Children, Has.Count.EqualTo(1));
            Assert.That((secondChild?.Children[0] as ParameterExpression)?.ParameterName, Is.EqualTo("Bar"));
        }
    }

    [Test]
    public void ParseStringExpression_WithUnterminatedStringInTemplateExpression_ThrowsInvalidPipelineException()
    {
        var ex = Assert.Throws<InvalidPipelineException>(
            () => ExpressionParser.ParseStringExpression("${{ \"unterminated }}")
        );

        Assert.That(ex?.Message, Does.Contain("Unexpected end of input reached; expected \""));
    }
}