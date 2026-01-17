using System.Text.RegularExpressions;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest.Parsing;

internal static partial class ExpressionParser
{
    internal static StringExpression ParseStringExpression(string expression)
    {
        var result = new StringExpression();
        
        foreach (var expr in ParseAsExpressions(expression))
        {
            result.Children.Add(expr);
        }

        return result;
    }

    private static List<Expression> ParseAsExpressions(string expression)
    {
        var cteRegex = TemplateExpressionRegex();

        var matches = cteRegex.Matches(expression);

        var expressionList = new List<Expression>();

        if (matches.Count == 0)
        {
            expressionList.Add(new StringLiteral { Value = expression });
        }
        else
        {
            var firstMatch = matches[0];
            var lastMatch = matches[^1];
            
            if (firstMatch.Index > 0)
            {
                expressionList.Add(new StringLiteral {Value = expression[..firstMatch.Index]});
            }

            for (int i = 0; i < matches.Count; ++i)
            {
                var match = matches[i];
                
                var templateExpression = new TemplateExpressionParser(match.Groups[1].Value).ParseExpression();

                expressionList.Add(new TemplateExpression { Children = [templateExpression]});

                if (i < matches.Count - 1)
                {
                    var gap = matches[i + 1].Index - match.Index - match.Length;

                    if (gap > 0)
                    {
                        expressionList.Add(new StringLiteral {Value = expression[(matches[i + 1].Index-gap)..matches[i + 1].Index]});
                    }
                }
            }

            if (lastMatch.Index + lastMatch.Length < expression.Length)
            {
                expressionList.Add(new StringLiteral {Value = expression[(lastMatch.Index + lastMatch.Length)..]});
            }
        }

        return expressionList;
    }

    private static IList<Expression> ParseTemplateExpression(string expression)
    {
        var result = new List<Expression>();
        
        var parameterRegex = ParametersInTemplateExpressionRegex();
        
        var matches = parameterRegex.Matches(expression).DistinctBy(m => m.Value);

        foreach (var match in matches)
        {
            var parameterName = match.Groups[1].Value;
            result.Add(new ParameterExpression {ParameterName =  parameterName});
        }

        return result;
    }

    [GeneratedRegex(@"parameters\.([a-zA-Z_][a-zA-Z0-9_]*)")]
    private static partial Regex ParametersInTemplateExpressionRegex();

    [GeneratedRegex(@"\$\{\{\s*(.+?)\s*\}\}")]
    private static partial Regex TemplateExpressionRegex();
}