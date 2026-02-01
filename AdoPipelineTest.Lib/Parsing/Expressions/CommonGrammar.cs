using AdoPipelineTest.Parsing.Ast;
using Sprache;

namespace AdoPipelineTest.Parsing.Expressions;

internal static class CommonGrammar
{
    internal static Parser<string> Identifier =>
        Parse.Letter.Or(Parse.Char('_'))
            .Once()
            .Then(c => Parse.LetterOrDigit.Or(Parse.Chars("._")).Many()
                .Select(chars => new string(c.Concat(chars).ToArray())))
            .Token();
    
    internal static Parser<Expression> IdentExpr => Identifier.Select(Expression (n) => new Identifier(n));
    
    internal static Parser<Expression> BoolLiteralParser =>
        Parse.IgnoreCase("true").Token().Select(Expression (_) => new BoolLiteral{Value = true})
            .Or(Parse.IgnoreCase("false").Token().Select(Expression (_) => new BoolLiteral{Value = false}));
    
    internal static Parser<Expression> ParameterReference =>
        from open in Parse.String("parameters.")
        from parameterName in CommonGrammar.Identifier
        select new ParameterExpression(parameterName);

    internal static Parser<Expression> VariableReferenceTemplateExpression =>
        from open in Parse.String("variables.")
        from variableName in CommonGrammar.Identifier
        select new VariableExpression(variableName);

    internal static Parser<Expression> VariableReferenceRuntime2Expression =>
        from open in Parse.String("variables['")
        from variableName in CommonGrammar.Identifier
        from close in Parse.String("']")
        select new VariableExpression(variableName);

    internal static Parser<Expression> VariableReferenceRuntimeExpression =>
        from open in Parse.String("$[variables.")
        from variableName in CommonGrammar.Identifier
        from close in Parse.Char(']')
        select new VariableExpression(variableName);

    internal static Parser<Expression> VariableReferenceMacroExpression =>
        from open in Parse.String("$(")
        from variableName in CommonGrammar.Identifier
        from close in Parse.Char(')')
        select new VariableExpression(variableName);

    internal static Parser<Expression> StringLiteralSingleQuote =>
        from open in Parse.Char('\'')
        from content in Parse.CharExcept('\'').Many().Text()
        from close in Parse.Char('\'')
        select new StringLiteral {Value = content};

    internal static Parser<Expression> StringLiteralDoubleQuote =>
        from open in Parse.Char('"')
        from content in Parse.CharExcept('"').Many().Text()
        from close in Parse.Char('"')
        select new StringLiteral {Value = content};

    internal static Parser<Expression> Literal = BoolLiteralParser
        .Or(StringLiteralSingleQuote)
        .Or(StringLiteralDoubleQuote);
}