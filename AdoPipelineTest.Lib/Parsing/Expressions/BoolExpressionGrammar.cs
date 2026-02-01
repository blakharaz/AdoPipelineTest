using AdoPipelineTest.Parsing.Ast;
using Sprache;

namespace AdoPipelineTest.Parsing.Expressions;

public static class BoolExpressionGrammar
{
    private static Parser<Expression> ExprRef => Parse.Ref(() => InnerExpression);

    private static Parser<Expression> TemplateExpression =>
        from l in Parse.String("${{").Token()
        from e in ExprRef
        from r in Parse.String("}}").Token()
        select e;

    // N-ary args helper (0+ args)
    private static Parser<IEnumerable<Expression>> NaryArgs(int minArgs, int maxArgs) =>
        ExprRef.DelimitedBy(Parse.Char(',').Optional().Token())
            .Where(a => minArgs <= a.Count())
            .Where(a => a.Count() <= maxArgs);

    // Arity-specific function parsers
    private static Parser<FunctionExpression> UnaryFunction(string name) =>
        from n in Parse.String(name).Token()
        from l in Parse.Char('(').Token()
        from arg in ExprRef.Once()
        from r in Parse.Char(')').Token()
        select new FunctionExpression(n, arg.ToList());

    internal static Parser<FunctionExpression> BinaryFunction(string name) =>
        from n in Parse.String(name).Token()
        from l in Parse.Char('(').Token()
        from args in NaryArgs(2, 2)
        from r in Parse.Char(')').Token()
        select new FunctionExpression(n, args.ToList());

    private static Parser<FunctionExpression> NaryFunction(string name, int minArgs) =>
        from n in Parse.String(name).Token()
        from l in Parse.Char('(').Token()
        from args in NaryArgs(minArgs, int.MaxValue)
        from r in Parse.Char(')').Token()
        select new FunctionExpression(n, args.ToList());

    private static Parser<FunctionExpression> NullaryFunction(string name) =>
        from n in Parse.String(name).Token()
        from l in Parse.Char('(').Token()
        from r in Parse.Char(')').Token()
        select new FunctionExpression(n, Array.Empty<Expression>());

    // ADO boolean functions by arity group
    internal static Parser<Expression> BinaryFunctions =>
        BinaryFunction("eq")
            .Or(BinaryFunction("ne"))
            .Or(BinaryFunction("gt"))
            .Or(BinaryFunction("ge"))
            .Or(BinaryFunction("lt"))
            .Or(BinaryFunction("le"))
            .Or(BinaryFunction("contains"))
            .Or(BinaryFunction("startsWith"))
            .Or(BinaryFunction("endsWith"))
            .Or(BinaryFunction("containsValue"))
            .Or(BinaryFunction("xor"));

    internal static Parser<Expression> UnaryFunctions => UnaryFunction("not");

    internal static Parser<Expression> NaryFunctions =>
        NaryFunction("and", 2)
            .Or(NaryFunction("or", 2))
            .Or(NaryFunction("in", 1))
            .Or(NaryFunction("notIn", 1))
            .Or(NullaryFunction("succeeded"))
            .Or(NullaryFunction("succeededOrFailed"))
            .Or(NullaryFunction("failed"))
            .Or(NaryFunction("succeeded", 1))
            .Or(NaryFunction("succeededOrFailed", 1))
            .Or(NaryFunction("failed", 1));
    
    private static Parser<Expression> NullaryFunctions =>
        NullaryFunction("canceled")
            .Or(NullaryFunction("always"));

    internal static Parser<Expression> Functions =>
        BinaryFunctions.Or(UnaryFunctions).Or(NaryFunctions).Or(NullaryFunctions);

    private static Parser<Expression> Parens =>
        from l in Parse.Char('(').Token()
        from e in ExprRef
        from r in Parse.Char(')').Token()
        select e;

    private static readonly Parser<Expression> InnerExpression =
        CommonGrammar.Literal
            .Or(Functions)
            .Or(Parens)
            .Or(CommonGrammar.VariableReferenceTemplateExpression)
            .Or(CommonGrammar.VariableReferenceRuntimeExpression)
            .Or(CommonGrammar.VariableReferenceRuntime2Expression)
            .Or(CommonGrammar.ParameterReference)
            .Or(CommonGrammar.IdentExpr);

    private static readonly Parser<Expression> Expr =
        CommonGrammar.Literal
            .Or(CommonGrammar.VariableReferenceMacroExpression)
            .Or(TemplateExpression);

    public static Expression ParseExpression(string input)
    {
        try
        {
            return Expr.Parse(input);
        }
        catch (ParseException ex)
        {
            throw new ParseException($"Failed to parse ADO expression '{input}': {ex.Message}", ex);
        }
    }
}