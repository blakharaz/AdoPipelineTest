using AdoPipelineTest.Evaluation;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest.UnitTests.Evaluation;

[TestFixture]
public class ConditionalStepEvaluatorTest
{
    private Dictionary<string, object> _parameters = null!;
    private Dictionary<string, object> _variables = null!;

    [SetUp]
    public void Setup()
    {
        _parameters = [];
        _variables = [];
    }

    #region eq() Tests

    [Test]
    public void EvaluateCondition_WithEqFunction_ReturnsTrueWhenEqual()
    {
        // Condition: eq(parameters.toolset, 'msbuild')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "eq",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "toolset" },
                    new StringLiteral { Value = "msbuild" }
                }
            }
        );

        _parameters["toolset"] = "msbuild";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithEqFunction_ReturnsFalseWhenNotEqual()
    {
        // Condition: eq(parameters.toolset, 'msbuild')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "eq",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "toolset" },
                    new StringLiteral { Value = "msbuild" }
                }
            }
        );

        _parameters["toolset"] = "dotnet";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.False);
    }

    [Test]
    public void EvaluateCondition_WithEqFunction_ComparesTwoStringLiterals()
    {
        // Condition: eq('value1', 'value1')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "eq",
                FunctionParameters = new List<Expression>
                {
                    new StringLiteral { Value = "value1" },
                    new StringLiteral { Value = "value1" }
                }
            }
        );

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithEqFunction_ComparesTwoParameters()
    {
        // Condition: eq(parameters.a, parameters.b)
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "eq",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "a" },
                    new ParameterExpression { ParameterName = "b" }
                }
            }
        );

        _parameters["a"] = "same";
        _parameters["b"] = "same";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    #endregion

    #region ne() Tests

    [Test]
    public void EvaluateCondition_WithNeFunction_ReturnsTrueWhenNotEqual()
    {
        // Condition: ne(parameters.option, 'one')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "ne",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "option" },
                    new StringLiteral { Value = "one" }
                }
            }
        );

        _parameters["option"] = "two";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithNeFunction_ReturnsFalseWhenEqual()
    {
        // Condition: ne(parameters.option, 'one')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "ne",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "option" },
                    new StringLiteral { Value = "one" }
                }
            }
        );

        _parameters["option"] = "one";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.False);
    }

    #endregion

    #region and() Tests

    [Test]
    public void EvaluateCondition_WithAndFunction_ReturnsTrueWhenBothTrue()
    {
        // Condition: and(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "and",
                FunctionParameters = new List<Expression>
                {
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "a" },
                            new StringLiteral { Value = "x" }
                        }
                    },
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "b" },
                            new StringLiteral { Value = "y" }
                        }
                    }
                }
            }
        );

        _parameters["a"] = "x";
        _parameters["b"] = "y";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithAndFunction_ReturnsFalseWhenFirstFalse()
    {
        // Condition: and(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "and",
                FunctionParameters = new List<Expression>
                {
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "a" },
                            new StringLiteral { Value = "x" }
                        }
                    },
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "b" },
                            new StringLiteral { Value = "y" }
                        }
                    }
                }
            }
        );

        _parameters["a"] = "wrong";
        _parameters["b"] = "y";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.False);
    }

    [Test]
    public void EvaluateCondition_WithAndFunction_ReturnsFalseWhenSecondFalse()
    {
        // Condition: and(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "and",
                FunctionParameters = new List<Expression>
                {
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "a" },
                            new StringLiteral { Value = "x" }
                        }
                    },
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "b" },
                            new StringLiteral { Value = "y" }
                        }
                    }
                }
            }
        );

        _parameters["a"] = "x";
        _parameters["b"] = "wrong";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.False);
    }

    #endregion

    #region or() Tests

    [Test]
    public void EvaluateCondition_WithOrFunction_ReturnsTrueWhenFirstTrue()
    {
        // Condition: or(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "or",
                FunctionParameters = new List<Expression>
                {
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "a" },
                            new StringLiteral { Value = "x" }
                        }
                    },
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "b" },
                            new StringLiteral { Value = "y" }
                        }
                    }
                }
            }
        );

        _parameters["a"] = "x";
        _parameters["b"] = "wrong";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithOrFunction_ReturnsTrueWhenSecondTrue()
    {
        // Condition: or(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "or",
                FunctionParameters = new List<Expression>
                {
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "a" },
                            new StringLiteral { Value = "x" }
                        }
                    },
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "b" },
                            new StringLiteral { Value = "y" }
                        }
                    }
                }
            }
        );

        _parameters["a"] = "wrong";
        _parameters["b"] = "y";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithOrFunction_ReturnsFalseWhenBothFalse()
    {
        // Condition: or(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "or",
                FunctionParameters = new List<Expression>
                {
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "a" },
                            new StringLiteral { Value = "x" }
                        }
                    },
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "b" },
                            new StringLiteral { Value = "y" }
                        }
                    }
                }
            }
        );

        _parameters["a"] = "wrong1";
        _parameters["b"] = "wrong2";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.False);
    }

    #endregion

    #region not() Tests

    [Test]
    public void EvaluateCondition_WithNotFunction_InvertsTrue()
    {
        // Condition: not(eq(parameters.option, 'one'))
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "not",
                FunctionParameters = new List<Expression>
                {
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "option" },
                            new StringLiteral { Value = "one" }
                        }
                    }
                }
            }
        );

        _parameters["option"] = "one";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.False);
    }

    [Test]
    public void EvaluateCondition_WithNotFunction_InvertsFalse()
    {
        // Condition: not(eq(parameters.option, 'one'))
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "not",
                FunctionParameters = new List<Expression>
                {
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "option" },
                            new StringLiteral { Value = "one" }
                        }
                    }
                }
            }
        );

        _parameters["option"] = "two";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    #endregion

    #region contains() Tests

    [Test]
    public void EvaluateCondition_WithContainsFunction_ReturnsTrueWhenFound()
    {
        // Condition: contains(parameters.tags, 'production')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "contains",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "tags" },
                    new StringLiteral { Value = "production" }
                }
            }
        );

        _parameters["tags"] = "staging,production,test";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithContainsFunction_ReturnsFalseWhenNotFound()
    {
        // Condition: contains(parameters.tags, 'production')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "contains",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "tags" },
                    new StringLiteral { Value = "production" }
                }
            }
        );

        _parameters["tags"] = "staging,test";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.False);
    }

    #endregion

    #region startswith() Tests

    [Test]
    public void EvaluateCondition_WithStartsWithFunction_ReturnsTrueWhenMatches()
    {
        // Condition: startswith(parameters.branch, 'refs/heads/')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "startswith",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "branch" },
                    new StringLiteral { Value = "refs/heads/" }
                }
            }
        );

        _parameters["branch"] = "refs/heads/main";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithStartsWithFunction_ReturnsFalseWhenNotMatches()
    {
        // Condition: startswith(parameters.branch, 'refs/heads/')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "startswith",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "branch" },
                    new StringLiteral { Value = "refs/heads/" }
                }
            }
        );

        _parameters["branch"] = "refs/tags/v1.0";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.False);
    }

    #endregion

    #region endswith() Tests

    [Test]
    public void EvaluateCondition_WithEndsWithFunction_ReturnsTrueWhenMatches()
    {
        // Condition: endswith(parameters.artifact, '.zip')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "endswith",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "artifact" },
                    new StringLiteral { Value = ".zip" }
                }
            }
        );

        _parameters["artifact"] = "build-1.0.0.zip";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithEndsWithFunction_ReturnsFalseWhenNotMatches()
    {
        // Condition: endswith(parameters.artifact, '.zip')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "endswith",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "artifact" },
                    new StringLiteral { Value = ".zip" }
                }
            }
        );

        _parameters["artifact"] = "build-1.0.0.tar.gz";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.False);
    }

    #endregion

    #region Numeric Comparison Tests

    [Test]
    public void EvaluateCondition_WithLtFunction_ReturnsTrueWhenLess()
    {
        // Condition: lt(parameters.version, '2.0')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "lt",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "version" },
                    new StringLiteral { Value = "2.0" }
                }
            }
        );

        _parameters["version"] = "1.5";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithLeFunction_ReturnsTrueWhenLessOrEqual()
    {
        // Condition: le(parameters.version, '2.0')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "le",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "version" },
                    new StringLiteral { Value = "2.0" }
                }
            }
        );

        _parameters["version"] = "2.0";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithGtFunction_ReturnsTrueWhenGreater()
    {
        // Condition: gt(parameters.version, '1.0')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "gt",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "version" },
                    new StringLiteral { Value = "1.0" }
                }
            }
        );

        _parameters["version"] = "2.5";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithGeFunction_ReturnsTrueWhenGreaterOrEqual()
    {
        // Condition: ge(parameters.version, '1.0')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "ge",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "version" },
                    new StringLiteral { Value = "1.0" }
                }
            }
        );

        _parameters["version"] = "1.0";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    #endregion

    #region Variable Tests

    [Test]
    public void EvaluateCondition_WithVariableExpression_ResolvesVariable()
    {
        // Condition: eq(variables.environment, 'production')
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "eq",
                FunctionParameters = new List<Expression>
                {
                    new VariableExpression { Name = "environment" },
                    new StringLiteral { Value = "production" }
                }
            }
        );

        _variables["environment"] = "production";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    #endregion

    #region Complex Logic Tests

    [Test]
    public void EvaluateCondition_WithNestedAndOr_EvaluatesCorrectly()
    {
        // Condition: and(eq(parameters.a, 'x'), or(eq(parameters.b, 'y'), eq(parameters.c, 'z')))
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "and",
                FunctionParameters = new List<Expression>
                {
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "a" },
                            new StringLiteral { Value = "x" }
                        }
                    },
                    new FunctionExpression
                    {
                        FunctionName = "or",
                        FunctionParameters = new List<Expression>
                        {
                            new FunctionExpression
                            {
                                FunctionName = "eq",
                                FunctionParameters = new List<Expression>
                                {
                                    new ParameterExpression { ParameterName = "b" },
                                    new StringLiteral { Value = "y" }
                                }
                            },
                            new FunctionExpression
                            {
                                FunctionName = "eq",
                                FunctionParameters = new List<Expression>
                                {
                                    new ParameterExpression { ParameterName = "c" },
                                    new StringLiteral { Value = "z" }
                                }
                            }
                        }
                    }
                }
            }
        );

        _parameters["a"] = "x";
        _parameters["b"] = "not-y";
        _parameters["c"] = "z";

        // a='x' (true) AND (b='y' (false) OR c='z' (true)) = true AND true = true
        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithMultipleAndConditions_EvaluatesAll()
    {
        // Condition: and(eq(parameters.a, 'x'), eq(parameters.b, 'y'), eq(parameters.c, 'z'))
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "and",
                FunctionParameters = new List<Expression>
                {
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "a" },
                            new StringLiteral { Value = "x" }
                        }
                    },
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "b" },
                            new StringLiteral { Value = "y" }
                        }
                    },
                    new FunctionExpression
                    {
                        FunctionName = "eq",
                        FunctionParameters = new List<Expression>
                        {
                            new ParameterExpression { ParameterName = "c" },
                            new StringLiteral { Value = "z" }
                        }
                    }
                }
            }
        );

        _parameters["a"] = "x";
        _parameters["b"] = "y";
        _parameters["c"] = "z";

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public void EvaluateCondition_WithUnknownFunction_ThrowsInvalidOperationException()
    {
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "unknownFunc",
                FunctionParameters = []
            }
        );

        Assert.That(
            () => ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables),
            Throws.TypeOf<InvalidOperationException>().With.Message.Contains("Unknown function")
        );
    }

    [Test]
    public void EvaluateCondition_WithWrongParameterCount_ThrowsInvalidOperationException()
    {
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "eq",
                FunctionParameters = new List<Expression>
                {
                    new StringLiteral { Value = "only-one-param" }
                }
            }
        );

        Assert.That(
            () => ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables),
            Throws.TypeOf<InvalidOperationException>().With.Message.Contains("requires exactly 2 parameters")
        );
    }

    [Test]
    public void EvaluateCondition_WithMissingParameter_ThrowsKeyNotFoundException()
    {
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "eq",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "nonexistent" },
                    new StringLiteral { Value = "value" }
                }
            }
        );

        Assert.That(
            () => ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables),
            Throws.TypeOf<KeyNotFoundException>()
        );
    }

    [Test]
    public void EvaluateCondition_WithInvalidNumberForNumericComparison_ThrowsInvalidOperationException()
    {
        var condition = CreateCondition(
            new FunctionExpression
            {
                FunctionName = "lt",
                FunctionParameters = new List<Expression>
                {
                    new ParameterExpression { ParameterName = "version" },
                    new StringLiteral { Value = "2.0" }
                }
            }
        );

        _parameters["version"] = "not-a-number";

        Assert.That(
            () => ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables),
            Throws.TypeOf<InvalidOperationException>().With.Message.Contains("Cannot convert")
        );
    }

    #endregion

    #region Empty Condition Tests

    [Test]
    public void EvaluateCondition_WithNullCondition_ReturnsTrue()
    {
        TemplateExpression? condition = null;

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition!, _parameters, _variables), Is.True);
    }

    [Test]
    public void EvaluateCondition_WithEmptyChildren_ReturnsTrue()
    {
        var condition = new TemplateExpression { Children = [] };

        Assert.That(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables), Is.True);
    }

    #endregion

    #region Helper Methods

    private static TemplateExpression CreateCondition(FunctionExpression funcExpr)
    {
        return new TemplateExpression
        {
            Children = new List<Expression> { funcExpr }
        };
    }

    #endregion
}

