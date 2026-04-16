using AdoPipelineTest.Parsing.Ast;
using Sprache;

namespace AdoPipelineTest.Parsing.Expressions;

internal static class CommonGrammar
{
    internal static Parser<string> Identifier =>
        Parse.Identifier(Parse.Letter.Or(Parse.Char('_')), Parse.LetterOrDigit.Or(Parse.Chars("._"))).Text();
    
    internal static Parser<Expression> IdentExpr => Identifier.Select(n => new Identifier(n));
    
    internal static Parser<Expression> BoolLiteralParser =>
        Parse.IgnoreCase("true").Token().Select(Expression (_) => new BoolLiteral{Value = true})
            .Or(Parse.IgnoreCase("false").Token().Select(Expression (_) => new BoolLiteral{Value = false}));
    
    internal static Parser<Expression> ParameterReference =>
        from _ in Parse.String("parameters.")
        from parameterName in CommonGrammar.Identifier
        select new ParameterExpression(parameterName);

    internal static Parser<Expression> VariableReferenceTemplateExpression =>
        from _ in Parse.String("variables.")
        from variableName in CommonGrammar.Identifier
        select new VariableExpression(variableName);

    internal static Parser<Expression> VariableReferenceRuntime2Expression =>
        from _ in Parse.String("variables['").Then(_ => CommonGrammar.Identifier).Then(_ => Parse.String("']"))
        select new VariableExpression(_);

    internal static Parser<Expression> VariableReferenceRuntimeExpression =>
        from _ in Parse.String("$[variables.").Then(_ => CommonGrammar.Identifier).Then(_ => Parse.String("]"))
        select new VariableExpression(_);

    internal static Parser<Expression> VariableReferenceMacroExpression =>
        from _ in Parse.String("$(").Then(_ => CommonGrammar.Identifier).Then(_ => Parse.String(")"))
        select new VariableExpression(_);

    internal static Parser<Expression> StringLiteralSingleQuote =>
        from _ in Parse.Char('\'').Then(_ => Parse.CharExcept('\'').Many().Text()).Then(_ => Parse.Char('\'').Select(c => c.ToString()))
        select new StringLiteral {Value = _};

    internal static Parser<Expression> StringLiteralDoubleQuote =>
        from open in Parse.Char('"')
        from content in Parse.CharExcept('"').Many().Text()
        from close in Parse.Char('"')
        select new StringLiteral {Value = content};

    internal static Parser<Expression> Literal = BoolLiteralParser
        .Or(StringLiteralSingleQuote)
        .Or(StringLiteralDoubleQuote);
}