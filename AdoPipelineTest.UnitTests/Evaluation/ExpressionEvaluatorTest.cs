using AdoPipelineTest.Evaluation;
using Xunit;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Evaluation;

public class ExpressionEvaluatorTest
{
    [Fact]
    public void EvaluateBoolean_EvaluatesBooleanLiterals()
    {
        Assert.True(ExpressionEvaluator.EvaluateBool("true", true));
        Assert.True(ExpressionEvaluator.EvaluateBool("true", false));
        Assert.False(ExpressionEvaluator.EvaluateBool("false", true));
        Assert.False(ExpressionEvaluator.EvaluateBool("false", false));
    }

    [Fact]
    public void EvaluateBoolean_UsesDefaultValuesForNullString()
    {
        Assert.True(ExpressionEvaluator.EvaluateBool(null, true));
        Assert.False(ExpressionEvaluator.EvaluateBool(null, false));
    }

    [Fact]
    public void EvaluateVariables_ReplacesVariableExpressions()
    {
        const string stringWithVariables = "hello $(foo) $(bar) world";
        var variables = new Dictionary<string, object?> { ["foo"] = "to", ["bar"] = "the" };

        Assert.Equal("hello to the world", ExpressionEvaluator.EvaluateVariables(stringWithVariables, variables));
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithNoParameters_ReturnsUnchangedString()
    {
        const string input = "hello world";
        var parameters = new Dictionary<string, object>();

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("hello world", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithSingleParameter_ReplacesParameterReference()
    {
        const string input = "Project: ${{parameters.projectName}}";
        var parameters = new Dictionary<string, object> { ["projectName"] = "MyProject" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Project: MyProject", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithSingleParameter_IgnoresWhitespaceInParameterExpressions()
    {
        const string input = "Project: ${{ parameters.projectName  }}";
        var parameters = new Dictionary<string, object> { ["projectName"] = "MyProject" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Project: MyProject", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithMultipleParameters_ReplacesAllReferences()
    {
        const string input = "Building ${{parameters.projectName}} with ${{parameters.buildConfig}} configuration";
        var parameters = new Dictionary<string, object>
        {
            ["projectName"] = "MyProject",
            ["buildConfig"] = "Release"
        };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Building MyProject with Release configuration", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithRepeatedParameterReference_ReplacesAllOccurrences()
    {
        const string input = "${{parameters.artifact}}-${{parameters.version}}-${{parameters.artifact}}.zip";
        var parameters = new Dictionary<string, object>
        {
            ["artifact"] = "build",
            ["version"] = "1.0.0"
        };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("build-1.0.0-build.zip", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithNumericParameterValue_ConvertsToString()
    {
        const string input = "Timeout: ${{parameters.timeoutMinutes}} minutes";
        var parameters = new Dictionary<string, object> { ["timeoutMinutes"] = 30 };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Timeout: 30 minutes", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithBooleanParameterValue_ConvertsToString()
    {
        const string input = "Enabled: ${{parameters.enableFeature}}";
        var parameters = new Dictionary<string, object> { ["enableFeature"] = true };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Enabled: True", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithEmptyParameterName_IsNotReplaced()
    {
        const string input = "Value: ${{}}";
        var parameters = new Dictionary<string, object> { [""] = "empty" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Value: ${{}}", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithSpecialCharactersInValue_PreservesCharacters()
    {
        const string input = "Path: ${{parameters.buildPath}}";
        var parameters = new Dictionary<string, object> { ["buildPath"] = "/home/user/build-output_v2.0" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Path: /home/user/build-output_v2.0", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithParameterValueContainingParameterSyntax_DoesNotRecurse()
    {
        const string input = "Template: ${{parameters.templateName}}";
        var parameters = new Dictionary<string, object> { ["templateName"] = "${{parameters.someOtherParam}}" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Template: ${{parameters.someOtherParam}}", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithEmptyString_ReturnsEmpty()
    {
        const string input = "";
        var parameters = new Dictionary<string, object> { ["param"] = "value" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithOnlyParameterReference_ReplacesWithValue()
    {
        const string input = "${{parameters.projectName}}";
        var parameters = new Dictionary<string, object> { ["projectName"] = "MyProject" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("MyProject", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithAdjacentParameterReferences_ReplacesCorrectly()
    {
        const string input = "${{parameters.owner}}/${{parameters.repo}}";
        var parameters = new Dictionary<string, object>
        {
            ["owner"] = "Microsoft",
            ["repo"] = "azure-pipelines"
        };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Microsoft/azure-pipelines", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithMissingParameter_ThrowsInvalidOperationException()
    {
        const string input = "Project: ${{parameters.projectName}}";
        var parameters = new Dictionary<string, object>();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters));

        Assert.Contains("Parameter 'projectName' not found", ex.Message);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithMultipleParametersAndOneMissing_ThrowsInvalidOperationException()
    {
        const string input = "Building ${{parameters.projectName}} with ${{parameters.buildConfig}} configuration";
        var parameters = new Dictionary<string, object> { ["projectName"] = "MyProject" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters));

        Assert.Contains("Parameter 'buildConfig' not found", ex.Message);
    }

    [Fact]
    public void EvaluateParametersInCompileTimeExpression_WithMissingParameter_ThrowsInvalidOperationException()
    {
        const string input = "parameters.missingParam";
        var parameters = new Dictionary<string, object>();
        var nullableParams = parameters.Cast<KeyValuePair<string, object?>>().ToDictionary(x => x.Key, x => x.Value);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ExpressionEvaluator.EvaluateParametersInCompileTimeExpression(input, nullableParams));

        Assert.Contains("Parameter 'missingParam' not found", ex.Message);
    }

    [Fact]
    public void EvaluateString_WithMissingParameter_ThrowsInvalidOperationException()
    {
        const string input = "Project: ${{parameters.projectName}}";
        var parameters = new Dictionary<string, object>();
        var variables = new Dictionary<string, object?>();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ExpressionEvaluator.EvaluateString(input, parameters, variables));

        Assert.Contains("Parameter 'projectName' not found", ex.Message);
    }

    [Fact]
    public void EvaluateString_WithMissingVariable_DoesNotThrowButLeavesVariableUnreplaced()
    {
        const string input = "Value: $(missingVar)";
        var parameters = new Dictionary<string, object>();
        var variables = new Dictionary<string, object?>();

        var result = ExpressionEvaluator.EvaluateString(input, parameters, variables);

        Assert.Equal("Value: $(missingVar)", result);
    }

    [Fact]
    public void EvaluateVariables_WithMissingVariable_DoesNotThrowButLeavesVariableUnreplaced()
    {
        const string input = "Value: $(missingVar)";
        var variables = new Dictionary<string, object?>();

        var result = ExpressionEvaluator.EvaluateVariables(input, variables);

        Assert.Equal("Value: $(missingVar)", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithParameterReferencedMultipleTimesAndMissing_ThrowsOncePerParameter()
    {
        const string input = "Start ${{parameters.config}} middle ${{parameters.config}} end";
        var parameters = new Dictionary<string, object>();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters));

        Assert.Contains("Parameter 'config' not found", ex.Message);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithNullParameterValue_ConvertsToEmptyString()
    {
        const string input = "Value: ${{parameters.nullParam}}";
        var parameters = new Dictionary<string, object> { ["nullParam"] = null! };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Value: ", result);
    }

    [Fact]
    public void EvaluateCompileTimeExpression_WithEmptyStringParameterValue_ReplacesWithEmptyString()
    {
        const string input = "Value: '${{parameters.emptyParam}}'";
        var parameters = new Dictionary<string, object> { ["emptyParam"] = "" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.Equal("Value: ''", result);
    }
}
