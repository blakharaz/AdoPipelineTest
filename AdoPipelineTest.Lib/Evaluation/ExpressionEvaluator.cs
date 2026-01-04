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
        return EvaluateVariables(str, variables);
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