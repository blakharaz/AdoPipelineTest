using Xunit;
using AdoPipelineTest.Parsing.Ast;
using AdoPipelineTest.Parsing.Expressions;
using Sprache;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Parsing.Expressions;

public class CommonGrammarTest
{
    [Fact]
    public void BoolLiterals()
    {
        var resultTrue = CommonGrammar.Literal.Parse("true");
        var resultFalse = CommonGrammar.Literal.Parse("false");
        Assert.IsType<BoolLiteral>(resultTrue);
        Assert.IsType<BoolLiteral>(resultFalse);
    }

    [Fact]
    public void StringLiterals_SingleQuote()
    {
        var result = CommonGrammar.Literal.Parse("'Hello'");
        var result2 = CommonGrammar.Literal.Parse("'true'");

        Assert.IsType<StringLiteral>(result);
        Assert.IsType<StringLiteral>(result2);
    }
    
    [Fact]
    public void StringLiterals_DoubleQuote()
    {
        var result = CommonGrammar.Literal.Parse("\"Hello\"");
        var result2 = CommonGrammar.Literal.Parse("\"false\"");

        Assert.IsType<StringLiteral>(result);
        Assert.IsType<StringLiteral>(result2);
    }
}