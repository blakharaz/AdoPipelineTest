using System.Text.RegularExpressions;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest.Evaluation;

internal static partial class ExpressionEvaluator
{
    /// <summary>
    /// Evaluates a TemplateExpression condition to a boolean value.
    /// Handles parameter references, variables, and logical/comparison functions.
    /// </summary>
    internal static bool EvaluateCondition(TemplateExpression? condition, 
        Dictionary<string, object> parameters, 
        Dictionary<string, object?> variables)
    {
        if (condition == null || condition.Children.Count == 0)
        {
            return true;
        }

        // For a simple condition with a single expression
        if (condition.Children.Count == 1)
        {
            var child = condition.Children[0];
            
            if (child is FunctionExpression funcExpr)
            {
                return EvaluateBooleanFunctionExpression(funcExpr, parameters, variables);
            }
            
            if (child is StringLiteral stringLiteral)
            {
                // Handle string literals like "true" from else branches
                return EvaluateBool(stringLiteral.Value, true);
            }
            
            if (child is ParameterExpression paramExpr)
            {
                var value = GetParameterValue(paramExpr.ParameterName, parameters);
                return EvaluateBool(value, true);
            }
            
            if (child is VariableExpression varExpr)
            {
                var value = GetVariableValue(varExpr.Name, variables);
                return EvaluateBool(value, true);
            }
        }

        throw new InvalidOperationException($"Unsupported condition structure with {condition.Children.Count} children");
    }

    private static bool EvaluateBooleanFunctionExpression(FunctionExpression funcExpr,
        Dictionary<string, object> parameters,
        Dictionary<string, object?> variables)
    {
        return funcExpr.FunctionName.ToLowerInvariant() switch
        {
            "eq" => EvaluateEq(funcExpr.FunctionParameters, parameters, variables),
            "ne" => EvaluateNe(funcExpr.FunctionParameters, parameters, variables),
            "and" => EvaluateAnd(funcExpr.FunctionParameters, parameters, variables),
            "or" => EvaluateOr(funcExpr.FunctionParameters, parameters, variables),
            "not" => EvaluateNot(funcExpr.FunctionParameters, parameters, variables),
            "contains" => EvaluateContains(funcExpr.FunctionParameters, parameters, variables),
            "startswith" => EvaluateStartsWith(funcExpr.FunctionParameters, parameters, variables),
            "endswith" => EvaluateEndsWith(funcExpr.FunctionParameters, parameters, variables),
            "lt" => EvaluateLt(funcExpr.FunctionParameters, parameters, variables),
            "le" => EvaluateLe(funcExpr.FunctionParameters, parameters, variables),
            "gt" => EvaluateGt(funcExpr.FunctionParameters, parameters, variables),
            "ge" => EvaluateGe(funcExpr.FunctionParameters, parameters, variables),
            _ => throw new InvalidOperationException($"Unknown function: {funcExpr.FunctionName}")
        };
    }

    private static bool EvaluateEq(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count != 2)
            throw new InvalidOperationException("eq() requires exactly 2 parameters");

        var left = ExpressionToString(parameters[0], paramValues, variables);
        var right = ExpressionToString(parameters[1], paramValues, variables);

        return left.Equals(right, StringComparison.Ordinal);
    }

    private static bool EvaluateNe(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count != 2)
            throw new InvalidOperationException("ne() requires exactly 2 parameters");

        var left = ExpressionToString(parameters[0], paramValues, variables);
        var right = ExpressionToString(parameters[1], paramValues, variables);

        return !left.Equals(right, StringComparison.Ordinal);
    }

    private static bool EvaluateAnd(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count < 2)
            throw new InvalidOperationException("and() requires at least 2 parameters");

        foreach (var param in parameters)
        {
            if (param is FunctionExpression funcExpr)
            {
                if (!EvaluateBooleanFunctionExpression(funcExpr, paramValues, variables))
                    return false;
            }
            else
            {
                throw new InvalidOperationException("and() parameters must be function expressions");
            }
        }

        return true;
    }

    private static bool EvaluateOr(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count < 2)
            throw new InvalidOperationException("or() requires at least 2 parameters");

        foreach (var param in parameters)
        {
            if (param is FunctionExpression funcExpr)
            {
                if (EvaluateBooleanFunctionExpression(funcExpr, paramValues, variables))
                    return true;
            }
            else
            {
                throw new InvalidOperationException("or() parameters must be function expressions");
            }
        }

        return false;
    }

    private static bool EvaluateNot(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count != 1)
            throw new InvalidOperationException("not() requires exactly 1 parameter");

        if (parameters[0] is FunctionExpression funcExpr)
        {
            return !EvaluateBooleanFunctionExpression(funcExpr, paramValues, variables);
        }

        throw new InvalidOperationException("not() parameter must be a function expression");
    }

    private static bool EvaluateContains(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count != 2)
            throw new InvalidOperationException("contains() requires exactly 2 parameters");

        var haystack = ExpressionToString(parameters[0], paramValues, variables);
        var needle = ExpressionToString(parameters[1], paramValues, variables);

        return haystack.Contains(needle, StringComparison.Ordinal);
    }

    private static bool EvaluateStartsWith(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count != 2)
            throw new InvalidOperationException("startswith() requires exactly 2 parameters");

        var str = ExpressionToString(parameters[0], paramValues, variables);
        var prefix = ExpressionToString(parameters[1], paramValues, variables);

        return str.StartsWith(prefix, StringComparison.Ordinal);
    }

    private static bool EvaluateEndsWith(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count != 2)
            throw new InvalidOperationException("endswith() requires exactly 2 parameters");

        var str = ExpressionToString(parameters[0], paramValues, variables);
        var suffix = ExpressionToString(parameters[1], paramValues, variables);

        return str.EndsWith(suffix, StringComparison.Ordinal);
    }

    private static bool EvaluateLt(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count != 2)
            throw new InvalidOperationException("lt() requires exactly 2 parameters");

        var left = ExpressionToDouble(parameters[0], paramValues, variables);
        var right = ExpressionToDouble(parameters[1], paramValues, variables);

        return left < right;
    }

    private static bool EvaluateLe(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count != 2)
            throw new InvalidOperationException("le() requires exactly 2 parameters");

        var left = ExpressionToDouble(parameters[0], paramValues, variables);
        var right = ExpressionToDouble(parameters[1], paramValues, variables);

        return left <= right;
    }

    private static bool EvaluateGt(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count != 2)
            throw new InvalidOperationException("gt() requires exactly 2 parameters");

        var left = ExpressionToDouble(parameters[0], paramValues, variables);
        var right = ExpressionToDouble(parameters[1], paramValues, variables);

        return left > right;
    }

    private static bool EvaluateGe(IList<Expression> parameters,
        Dictionary<string, object> paramValues,
        Dictionary<string, object?> variables)
    {
        if (parameters.Count != 2)
            throw new InvalidOperationException("ge() requires exactly 2 parameters");

        var left = ExpressionToDouble(parameters[0], paramValues, variables);
        var right = ExpressionToDouble(parameters[1], paramValues, variables);

        return left >= right;
    }

    private static string ExpressionToString(Expression expr,
        Dictionary<string, object> parameters,
        Dictionary<string, object?> variables)
    {
        return expr switch
        {
            StringLiteral sl => sl.Value,
            ParameterExpression pe => GetParameterValue(pe.ParameterName, parameters),
            VariableExpression ve => GetVariableValue(ve.Name, variables),
            _ => throw new InvalidOperationException($"Unsupported expression type: {expr.GetType().Name}")
        };
    }

    private static double ExpressionToDouble(Expression expr,
        Dictionary<string, object> parameters,
        Dictionary<string, object?> variables)
    {
        var str = ExpressionToString(expr, parameters, variables);
        if (double.TryParse(str, out var value))
        {
            return value;
        }

        throw new InvalidOperationException($"Cannot convert '{str}' to a number");
    }

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

    internal static IDictionary<string, string> EvaluateDictionaryValues(IDictionary<string, string>? dict, Dictionary<string, object?> parameters, Dictionary<string, object?> variables)
    {
        if (dict == null || dict.Count == 0)
        {
            return new Dictionary<string, string>();
        }
        
        return dict.ToDictionary(entry => entry.Key, entry => EvaluateStringNullable(entry.Value, parameters, variables));
    }
    
    internal static string EvaluateString(string str, Dictionary<string, object> parameters, Dictionary<string, object?> variables)
    {
        var nullableParams = parameters.Cast<KeyValuePair<string, object?>>().ToDictionary(x => x.Key, x => x.Value);
        return EvaluateStringNullable(str, nullableParams, variables);
    }
    
    private static string EvaluateStringNullable(string str, Dictionary<string, object?> parameters, Dictionary<string, object?> variables)
    {
        var strWithEvaluatedParameters = EvaluateCompileTimeExpressionsNullable(str, parameters);
        return EvaluateVariables(strWithEvaluatedParameters, variables);
    }

    internal static string EvaluateCompileTimeExpressions(string str, Dictionary<string, object> parameters)
    {
        var nullableParams = parameters.Cast<KeyValuePair<string, object?>>().ToDictionary(x => x.Key, x => x.Value);
        return EvaluateCompileTimeExpressionsNullable(str, nullableParams);
    }
    
    private static string EvaluateCompileTimeExpressionsNullable(string str, Dictionary<string, object?> parameters)
    {
        var cteRegex = CompileTimeExpressionRegex();
        
        return cteRegex.Replace(str, match =>
        {
            var expression = match.Groups[1].Value;
            return EvaluateParametersInCompileTimeExpression(expression, parameters);
        });
    }

    internal static string EvaluateParametersInCompileTimeExpression(string str, Dictionary<string, object?> parameters)
    {
        var parameterRegex = ParametersInCompileTimeExpressionRegex();

        var matches = parameterRegex.Matches(str).DistinctBy(m => m.Value);

        foreach (var match in matches)
        {
            var parameterName = match.Groups[1].Value;
            
            if (!parameters.TryGetValue(parameterName, out var paramValue))
            {
                throw new InvalidOperationException($"Parameter '{parameterName}' not found");
            }
            
            str = str.Replace(match.Value, paramValue?.ToString() ?? string.Empty);
        }

        return str;
    }

    internal static string EvaluateVariables(string str, Dictionary<string, object?> variables)
    {
        var result = str;

        foreach (var entry in variables)
        {
            result = result.Replace($"$({entry.Key})", entry.Value?.ToString() ?? string.Empty);
        }

        return result;
    }

    [GeneratedRegex(@"parameters\.([a-zA-Z_][a-zA-Z0-9_]*)")]
    private static partial Regex ParametersInCompileTimeExpressionRegex();

    [GeneratedRegex(@"\$\{\{\s*(.+?)\s*\}\}")]
    private static partial Regex CompileTimeExpressionRegex();

    private static string GetParameterValue(string parameterName, Dictionary<string, object> parameters)
    {
        if (!parameters.TryGetValue(parameterName, out var value))
        {
            throw new KeyNotFoundException($"Parameter '{parameterName}' not found");
        }
        
        return value.ToString() ?? string.Empty;
    }

    private static string GetVariableValue(string variableName, Dictionary<string, object?> variables)
    {
        if (!variables.TryGetValue(variableName, out var value))
        {
            throw new InvalidOperationException($"Variable '{variableName}' not found");
        }
        
        return value?.ToString() ?? string.Empty;
    }
}