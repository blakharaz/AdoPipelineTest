using System.Linq.Expressions;
using AdoPipelineTest.Parsing.Ast;
using Sprache;
using Expression = AdoPipelineTest.Parsing.Ast.Expression;

namespace AdoPipelineTest.Parsing.Expressions;

public static class StringExpressionGrammar
{
    // private static Parser<Expression> IdentExpr => Common.Identifier.Select(n => new Ident(n) as Expr);

    // Forward reference for recursion
    private static Parser<Expression> ExprRef = Parse.Ref(() => Expr);

    private static Parser<TemplateExpression> TemplateExpression =>
        from l in Parse.String("${{").Token()
        from e in ExprRef.Many()
        from r in Parse.String("}}").Token()
        select new TemplateExpression { Children = e.ToList() };
    
    // N-ary args helper (0+ args)
    private static Parser<IEnumerable<Expression>> NaryArgs(int minArgs, int maxArgs) =>
        ExprRef.DelimitedBy(Parse.Char(',').Token())
            .Where(a => minArgs <= a.Count())
            .Where(a => a.Count() <= maxArgs);

    // Arity-specific function parsers
    private static Parser<FunctionExpression> UnaryFunction(string name) =>
        from n in Parse.IgnoreCase(name).Token()
        from l in Parse.Char('(').Token()
        from arg in ExprRef.Once()
        from r in Parse.Char(')').Token()
        select new FunctionExpression(n, arg.ToList());

    private static Parser<FunctionExpression> BinaryFunction(string name) =>
        from n in Parse.IgnoreCase(name).Token()
        from l in Parse.Char('(').Token()
        from args in NaryArgs(2, 2)
        from r in Parse.Char(')').Token()
        select new FunctionExpression(n, args.ToList());

    private static Parser<FunctionExpression> NaryFunction(string name, int minArgs, int? maxArgs = null) =>
        from n in Parse.IgnoreCase(name).Token()
        from l in Parse.Char('(').Token()
        from args in NaryArgs(minArgs, maxArgs.GetValueOrDefault(int.MaxValue))
        from r in Parse.Char(')').Token()
        select new FunctionExpression(n, args.ToList());

    // ADO boolean functions by arity group
    private static Parser<Expression> BinaryFunctions =>
        BinaryFunction("join")
            .Or(BinaryFunction("split"));

    private static Parser<Expression> UnaryFunctions =>
        UnaryFunction("convertToJson")
            .Or(UnaryFunction("lower"))
            .Or(UnaryFunction("trim"))
            .Or(UnaryFunction("upper"));

    private static Parser<Expression> NaryFunctions =>
        NaryFunction("coalesce", 2)
            .Or(NaryFunction("format", 1))
            .Or(NaryFunction("iif", 1, 3))
            .Or(NaryFunction("replace", 3, 3));
    
    private static Parser<Expression> Functions =>
        BinaryFunctions.Or(UnaryFunctions).Or(NaryFunctions);

    private static Parser<Expression> Parens =>
        from l in Parse.Char('(').Token()
        from e in ExprRef
        from r in Parse.Char(')').Token()
        select e;

    private static Parser<Expression> Primary =>
        CommonGrammar.Literal
            .Or(Parens)
            .Or(Functions)
            .Or(CommonGrammar.ParameterReference)
            .Or(CommonGrammar.VariableReferenceTemplateExpression)
            .Or(CommonGrammar.VariableReferenceRuntimeExpression);

    public static readonly Parser<Expression> Expr = Primary; // No .End() - let caller handle

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