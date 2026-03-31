using AdoPipelineTest.Evaluation;

namespace AdoPipelineTest.UnitTests.Evaluation;

[TestFixture]
public class ExpressionEvaluatorTest
{
    [Test]
    public void EvaluateBoolean_EvaluatesBooleanLiterals()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ExpressionEvaluator.EvaluateBool("true", true), Is.True);
            Assert.That(ExpressionEvaluator.EvaluateBool("true", false), Is.True);
            Assert.That(ExpressionEvaluator.EvaluateBool("false", true), Is.False);
            Assert.That(ExpressionEvaluator.EvaluateBool("false", false), Is.False);
        }
    }

    [Test]
    public void EvaluateBoolean_UsesDefaultValuesForNullString()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ExpressionEvaluator.EvaluateBool(null, true), Is.True);
            Assert.That(ExpressionEvaluator.EvaluateBool(null, false), Is.False);
        }
    }

    [Test]
    public void EvaluateVariables_ReplacesVariableExpressions()
    {
        const string stringWithVariables = "hello $(foo) $(bar) world";
        var variables = new Dictionary<string, object> { ["foo"] = "to", ["bar"] = "the" };

        Assert.That(ExpressionEvaluator.EvaluateVariables(stringWithVariables, variables), Is.EqualTo("hello to the world"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithNoParameters_ReturnsUnchangedString()
    {
        const string input = "hello world";
        var parameters = new Dictionary<string, object>();

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("hello world"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithSingleParameter_ReplacesParameterReference()
    {
        const string input = "Project: ${{parameters.projectName}}";
        var parameters = new Dictionary<string, object> { ["projectName"] = "MyProject" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Project: MyProject"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithSingleParameter_IgnoresWhitespaceInParameterExpressions()
    {
        const string input = "Project: ${{ parameters.projectName  }}";
        var parameters = new Dictionary<string, object> { ["projectName"] = "MyProject" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Project: MyProject"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithMultipleParameters_ReplacesAllReferences()
    {
        const string input = "Building ${{parameters.projectName}} with ${{parameters.buildConfig}} configuration";
        var parameters = new Dictionary<string, object>
        {
            ["projectName"] = "MyProject",
            ["buildConfig"] = "Release"
        };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Building MyProject with Release configuration"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithRepeatedParameterReference_ReplacesAllOccurrences()
    {
        const string input = "${{parameters.artifact}}-${{parameters.version}}-${{parameters.artifact}}.zip";
        var parameters = new Dictionary<string, object>
        {
            ["artifact"] = "build",
            ["version"] = "1.0.0"
        };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("build-1.0.0-build.zip"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithNumericParameterValue_ConvertsToString()
    {
        const string input = "Timeout: ${{parameters.timeoutMinutes}} minutes";
        var parameters = new Dictionary<string, object> { ["timeoutMinutes"] = 30 };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Timeout: 30 minutes"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithBooleanParameterValue_ConvertsToString()
    {
        const string input = "Enabled: ${{parameters.enableFeature}}";
        var parameters = new Dictionary<string, object> { ["enableFeature"] = true };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Enabled: True"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithEmptyParameterName_IsNotReplaced()
    {
        const string input = "Value: ${{}}";
        var parameters = new Dictionary<string, object> { [""] = "empty" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Value: ${{}}"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithSpecialCharactersInValue_PreservesCharacters()
    {
        const string input = "Path: ${{parameters.buildPath}}";
        var parameters = new Dictionary<string, object> { ["buildPath"] = "/home/user/build-output_v2.0" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Path: /home/user/build-output_v2.0"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithParameterValueContainingParameterSyntax_DoesNotRecurse()
    {
        const string input = "Template: ${{parameters.templateName}}";
        var parameters = new Dictionary<string, object> { ["templateName"] = "${{parameters.someOtherParam}}" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Template: ${{parameters.someOtherParam}}"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithEmptyString_ReturnsEmpty()
    {
        const string input = "";
        var parameters = new Dictionary<string, object> { ["param"] = "value" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithOnlyParameterReference_ReplacesWithValue()
    {
        const string input = "${{parameters.projectName}}";
        var parameters = new Dictionary<string, object> { ["projectName"] = "MyProject" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("MyProject"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithAdjacentParameterReferences_ReplacesCorrectly()
    {
        const string input = "${{parameters.owner}}/${{parameters.repo}}";
        var parameters = new Dictionary<string, object>
        {
            ["owner"] = "Microsoft",
            ["repo"] = "azure-pipelines"
        };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Microsoft/azure-pipelines"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithMissingParameter_ThrowsInvalidOperationException()
    {
        const string input = "Project: ${{parameters.projectName}}";
        var parameters = new Dictionary<string, object>();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters));

        Assert.That(ex?.Message, Does.Contain("Parameter 'projectName' not found"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithMultipleParametersAndOneMissing_ThrowsInvalidOperationException()
    {
        const string input = "Building ${{parameters.projectName}} with ${{parameters.buildConfig}} configuration";
        var parameters = new Dictionary<string, object> { ["projectName"] = "MyProject" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters));

        Assert.That(ex?.Message, Does.Contain("Parameter 'buildConfig' not found"));
    }

    [Test]
    public void EvaluateParametersInCompileTimeExpression_WithMissingParameter_ThrowsInvalidOperationException()
    {
        const string input = "parameters.missingParam";
        var parameters = new Dictionary<string, object>();
        var nullableParams = parameters.Cast<KeyValuePair<string, object?>>().ToDictionary(x => x.Key, x => x.Value);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ExpressionEvaluator.EvaluateParametersInCompileTimeExpression(input, nullableParams));

        Assert.That(ex?.Message, Does.Contain("Parameter 'missingParam' not found"));
    }

    [Test]
    public void EvaluateString_WithMissingParameter_ThrowsInvalidOperationException()
    {
        const string input = "Project: ${{parameters.projectName}}";
        var parameters = new Dictionary<string, object>();
        var variables = new Dictionary<string, object>();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ExpressionEvaluator.EvaluateString(input, parameters, variables));

        Assert.That(ex?.Message, Does.Contain("Parameter 'projectName' not found"));
    }

    [Test]
    public void EvaluateString_WithMissingVariable_DoesNotThrowButLeavesVariableUnreplaced()
    {
        // EvaluateVariables doesn't validate that variables exist, it just replaces
        // the ones that are provided. Missing variables are left as-is.
        const string input = "Value: $(missingVar)";
        var parameters = new Dictionary<string, object>();
        var variables = new Dictionary<string, object>();

        var result = ExpressionEvaluator.EvaluateString(input, parameters, variables);

        Assert.That(result, Is.EqualTo("Value: $(missingVar)"));
    }

    [Test]
    public void EvaluateVariables_WithMissingVariable_DoesNotThrowButLeavesVariableUnreplaced()
    {
        // EvaluateVariables doesn't validate that variables exist, it just replaces
        // the ones that are provided. Missing variables are left as-is.
        const string input = "Value: $(missingVar)";
        var variables = new Dictionary<string, object>();

        var result = ExpressionEvaluator.EvaluateVariables(input, variables);

        Assert.That(result, Is.EqualTo("Value: $(missingVar)"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithParameterReferencedMultipleTimesAndMissing_ThrowsOncePerParameter()
    {
        const string input = "Start ${{parameters.config}} middle ${{parameters.config}} end";
        var parameters = new Dictionary<string, object>();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters));

        Assert.That(ex?.Message, Does.Contain("Parameter 'config' not found"));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithNullParameterValue_ConvertsToEmptyString()
    {
        const string input = "Value: ${{parameters.nullParam}}";
        var parameters = new Dictionary<string, object> { ["nullParam"] = null! };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Value: "));
    }

    [Test]
    public void EvaluateCompileTimeExpression_WithEmptyStringParameterValue_ReplacesWithEmptyString()
    {
        const string input = "Value: '${{parameters.emptyParam}}'";
        var parameters = new Dictionary<string, object> { ["emptyParam"] = "" };

        var result = ExpressionEvaluator.EvaluateCompileTimeExpressions(input, parameters);

        Assert.That(result, Is.EqualTo("Value: ''"));
    }
}