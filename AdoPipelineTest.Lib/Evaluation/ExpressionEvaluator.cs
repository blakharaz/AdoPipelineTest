using System.Text.RegularExpressions;

namespace AdoPipelineTest.Evaluation;

internal static class ExpressionEvaluator
{
    internal static bool EvaluateBool(string? expression, bool defaultValue)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return defaultValue;
        }

        if (expression.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (expression.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        
        throw new ArgumentException($"Invalid boolean expression: {expression}");
    }

    internal static IDictionary<string, string> EvaluateDictionaryValues(IDictionary<string, string> dict, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        return dict.ToDictionary(entry => entry.Key, entry => EvaluateString(entry.Value, parameters, variables));
    }
    
    internal static string EvaluateString(string str, Dictionary<string, object> parameters, Dictionary<string, object> variables)
    {
        var strWithEvaluatedParameters = EvaluateCompileTimeExpressions(str, parameters);
        return EvaluateVariables(strWithEvaluatedParameters, variables);
    }

    internal static string EvaluateCompileTimeExpressions(string str, Dictionary<string, object> parameters)
    {
        var cteRegex = new Regex(@"\$\{\{\s*(.+?)\s*\}\}");

        var matches = cteRegex.Matches(str).DistinctBy(m => m.Value);

        foreach (var match in matches)
        {
            var expression = match.Groups[1].Value;
            
            var evaluatedExpression = EvaluateParametersInCompileTimeExpression(expression, parameters);

            str = str.Replace(match.Value, evaluatedExpression);
        }

        return str;
    }

    internal static string EvaluateParametersInCompileTimeExpression(string str, Dictionary<string, object> parameters)
    {
        var parameterRegex = new Regex(@"parameters\.([a-zA-Z_][a-zA-Z0-9_]*)");

        var matches = parameterRegex.Matches(str).DistinctBy(m => m.Value);

        foreach (var match in matches)
        {
            var parameterName = match.Groups[1].Value;
            
            str = str.Replace(match.Value, parameters[parameterName].ToString());
        }

        return str;
    }

    internal static string EvaluateVariables(string str, Dictionary<string, object> variables)
    {
        var result = str;

        foreach (var entry in variables)
        {
            result = result.Replace($"$({entry.Key})", entry.Value.ToString());
        }

        return result;
    }
}