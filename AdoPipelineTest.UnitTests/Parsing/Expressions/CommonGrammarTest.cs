using NUnit.Framework;
using AdoPipelineTest.Parsing.Ast;
using AdoPipelineTest.Parsing.Expressions;
using Sprache;

namespace AdoPipelineTest.UnitTests.Parsing.Expressions;

[TestFixture]
public class CommonGrammarTest
{
    [Test]
    public void BoolLiterals()
    {
        var resultTrue = CommonGrammar.Literal.Parse("true");
        var resultFalse = CommonGrammar.Literal.Parse("false");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultTrue, Is.InstanceOf<BoolLiteral>());
            Assert.That(resultFalse, Is.InstanceOf<BoolLiteral>());
        }
    }

    [Test]
    public void StringLiterals_SingleQuote()
    {
        var result = CommonGrammar.Literal.Parse("'Hello'");
        var result2 = CommonGrammar.Literal.Parse("'true'");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.InstanceOf<StringLiteral>());
            Assert.That(result2, Is.InstanceOf<StringLiteral>());
        }
    }
    
    [Test]
    public void StringLiterals_DoubleQuote()
    {
        var result = CommonGrammar.Literal.Parse("\"Hello\"");
        var result2 = CommonGrammar.Literal.Parse("\"false\"");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.InstanceOf<StringLiteral>());
            Assert.That(result2, Is.InstanceOf<StringLiteral>());
        }
    }
}
