namespace AdoPipelineTest.Evaluation;

internal class ExpressionEvaluator
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
}