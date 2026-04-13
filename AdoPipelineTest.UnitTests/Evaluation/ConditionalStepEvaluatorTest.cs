using Xunit;
using AdoPipelineTest.Evaluation;
using AdoPipelineTest.Parsing.Ast;
using Assert = Xunit.Assert;

namespace AdoPipelineTest.UnitTests.Evaluation;

public class ConditionalStepEvaluatorTest
{
    private Dictionary<string, object> _parameters = null!;
    private Dictionary<string, object?> _variables = null!;

    public ConditionalStepEvaluatorTest()
    {
        _parameters = [];
        _variables = [];
    }

    #region eq() Tests

    [Fact]
    public void EvaluateCondition_WithEqFunction_ReturnsTrueWhenEqual()
    {
        // Condition: eq(parameters.toolset, 'msbuild')
        var condition = CreateCondition(
            "eq", [
                new ParameterExpression("toolset"),
                new StringLiteral { Value = "msbuild" }
            ]
        );

        _parameters["toolset"] = "msbuild";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithEqFunction_ReturnsFalseWhenNotEqual()
    {
        // Condition: eq(parameters.toolset, 'msbuild')
        var condition = CreateCondition(
            "eq",
            [
                new ParameterExpression("toolset"),
                new StringLiteral { Value = "msbuild" }
            ]);

        _parameters["toolset"] = "dotnet";

        Assert.False(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithEqFunction_ComparesTwoStringLiterals()
    {
        // Condition: eq('value1', 'value1')
        var condition = CreateCondition("eq",
            [
                new StringLiteral { Value = "value1" },
                new StringLiteral { Value = "value1" }
            ]
        );

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithEqFunction_ComparesTwoParameters()
    {
        // Condition: eq(parameters.a, parameters.b)
        var condition = CreateCondition(
            "eq",
            [
                new ParameterExpression("a"),
                new ParameterExpression("b")
            ]);

        _parameters["a"] = "same";
        _parameters["b"] = "same";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region ne() Tests

    [Fact]
    public void EvaluateCondition_WithNeFunction_ReturnsTrueWhenNotEqual()
    {
        // Condition: ne(parameters.option, 'one')
        var condition = CreateCondition(
            "ne",
            [
                new ParameterExpression("option"),
                new StringLiteral { Value = "one" }
            ]);

        _parameters["option"] = "two";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithNeFunction_ReturnsFalseWhenEqual()
    {
        // Condition: ne(parameters.option, 'one')
        var condition = CreateCondition(
            "ne",
            [
                new ParameterExpression("option"),
                new StringLiteral { Value = "one" }
            ]);

        _parameters["option"] = "one";

        Assert.False(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region and() Tests

    [Fact]
    public void EvaluateCondition_WithAndFunction_ReturnsTrueWhenBothTrue()
    {
        // Condition: and(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition("and",
        [
            new FunctionExpression("eq", [
                    new ParameterExpression("a"),
                    new StringLiteral { Value = "x" }
                ]
            ),
            new FunctionExpression("eq", [
                    new ParameterExpression("b"),
                    new StringLiteral { Value = "y" }
                ]
            )
        ]);

        _parameters["a"] = "x";
        _parameters["b"] = "y";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithAndFunction_ReturnsFalseWhenFirstFalse()
    {
        // Condition: and(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition(
            "and",
            [
                new FunctionExpression("eq", [
                        new ParameterExpression("a"),
                        new StringLiteral { Value = "x" }
                    ]
                ),
                new FunctionExpression("eq", [
                        new ParameterExpression("b"),
                        new StringLiteral { Value = "y" }
                    ]
                )
            ]);

        _parameters["a"] = "wrong";
        _parameters["b"] = "y";

        Assert.False(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithAndFunction_ReturnsFalseWhenSecondFalse()
    {
        // Condition: and(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition(
            "and",
            [
                new FunctionExpression("eq", [
                        new ParameterExpression("a"),
                        new StringLiteral { Value = "x" }
                    ]
                ),
                new FunctionExpression("eq",
                    [
                        new ParameterExpression("b"),
                        new StringLiteral { Value = "y" }
                    ]
                )
            ]);

        _parameters["a"] = "x";
        _parameters["b"] = "wrong";

        Assert.False(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region or() Tests

    [Fact]
    public void EvaluateCondition_WithOrFunction_ReturnsTrueWhenFirstTrue()
    {
        // Condition: or(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition(
            "or", [
                new FunctionExpression("eq", [
                    new ParameterExpression("a"),
                    new StringLiteral { Value = "x" }
                ]),
                new FunctionExpression("eq", [
                        new ParameterExpression("b"),
                        new StringLiteral { Value = "y" }
                    ]
                )
            ]);

        _parameters["a"] = "x";
        _parameters["b"] = "wrong";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithOrFunction_ReturnsTrueWhenSecondTrue()
    {
        // Condition: or(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition("or", [
            new FunctionExpression("eq", [
                    new ParameterExpression("a"),
                    new StringLiteral { Value = "x" }
                ]
            ),
            new FunctionExpression("eq",
                [
                    new ParameterExpression("b"),
                    new StringLiteral { Value = "y" }
                ]
            )
        ]);

        _parameters["a"] = "wrong";
        _parameters["b"] = "y";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithOrFunction_ReturnsFalseWhenBothFalse()
    {
        // Condition: or(eq(parameters.a, 'x'), eq(parameters.b, 'y'))
        var condition = CreateCondition("or", [
            new FunctionExpression("eq", [
                    new ParameterExpression("a"),
                    new StringLiteral { Value = "x" }
                ]
            ),
            new FunctionExpression("eq", [
                    new ParameterExpression("b"),
                    new StringLiteral { Value = "y" }
                ]
            )
        ]);

        _parameters["a"] = "wrong1";
        _parameters["b"] = "wrong2";

        Assert.False(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region not() Tests

    [Fact]
    public void EvaluateCondition_WithNotFunction_InvertsTrue()
    {
        // Condition: not(eq(parameters.option, 'one'))
        var condition = CreateCondition("not", [
            new FunctionExpression("eq", [
                    new ParameterExpression("option"),
                    new StringLiteral { Value = "one" }
                ]
            )
        ]);

        _parameters["option"] = "one";

        Assert.False(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithNotFunction_InvertsFalse()
    {
        // Condition: not(eq(parameters.option, 'one'))
        var condition = CreateCondition("not", [
            new FunctionExpression("eq", [
                new ParameterExpression("option"),
                new StringLiteral { Value = "one" }
            ])
        ]);

        _parameters["option"] = "two";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region contains() Tests

    [Fact]
    public void EvaluateCondition_WithContainsFunction_ReturnsTrueWhenFound()
    {
        // Condition: contains(parameters.tags, 'production')
        var condition = CreateCondition("contains", [
            new ParameterExpression("tags"),
            new StringLiteral { Value = "production" }
        ]);

        _parameters["tags"] = "staging,production,test";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithContainsFunction_ReturnsFalseWhenNotFound()
    {
        // Condition: contains(parameters.tags, 'production')
        var condition = CreateCondition("contains", [
            new ParameterExpression("tags"),
            new StringLiteral { Value = "production" }
        ]);

        _parameters["tags"] = "staging,test";

        Assert.False(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region startswith() Tests

    [Fact]
    public void EvaluateCondition_WithStartsWithFunction_ReturnsTrueWhenMatches()
    {
        // Condition: startswith(parameters.branch, 'refs/heads/')
        var condition = CreateCondition(
            "startswith",
            [
                new ParameterExpression("branch"),
                new StringLiteral { Value = "refs/heads/" }
            ]);

        _parameters["branch"] = "refs/heads/main";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithStartsWithFunction_ReturnsFalseWhenNotMatches()
    {
        // Condition: startswith(parameters.branch, 'refs/heads/')
        var condition = CreateCondition(
            "startswith", [
                new ParameterExpression("branch"),
                new StringLiteral { Value = "refs/heads/" }
            ]);

        _parameters["branch"] = "refs/tags/v1.0";

        Assert.False(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region endswith() Tests

    [Fact]
    public void EvaluateCondition_WithEndsWithFunction_ReturnsTrueWhenMatches()
    {
        // Condition: endswith(parameters.artifact, '.zip')
        var condition = CreateCondition(
            "endswith", [
                new ParameterExpression("artifact"),
                new StringLiteral { Value = ".zip" }
            ]);

        _parameters["artifact"] = "build-1.0.0.zip";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithEndsWithFunction_ReturnsFalseWhenNotMatches()
    {
        // Condition: endswith(parameters.artifact, '.zip')
        var condition = CreateCondition("endswith", [
            new ParameterExpression("artifact"),
            new StringLiteral { Value = ".zip" }
        ]);

        _parameters["artifact"] = "build-1.0.0.tar.gz";

        Assert.False(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region Numeric Comparison Tests

    [Fact]
    public void EvaluateCondition_WithLtFunction_ReturnsTrueWhenLess()
    {
        // Condition: lt(parameters.version, '2.0')
        var condition = CreateCondition("lt", [
            new ParameterExpression("version"),
            new StringLiteral { Value = "2.0" }
        ]);

        _parameters["version"] = "1.5";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithLeFunction_ReturnsTrueWhenLessOrEqual()
    {
        // Condition: le(parameters.version, '2.0')
        var condition = CreateCondition("le", [
            new ParameterExpression("version"),
            new StringLiteral { Value = "2.0" }
        ]);

        _parameters["version"] = "2.0";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithGtFunction_ReturnsTrueWhenGreater()
    {
        // Condition: gt(parameters.version, '1.0')
        var condition = CreateCondition("gt", [
            new ParameterExpression("version"),
            new StringLiteral { Value = "1.0" }
        ]);

        _parameters["version"] = "2.5";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithGeFunction_ReturnsTrueWhenGreaterOrEqual()
    {
        // Condition: ge(parameters.version, '1.0')
        var condition = CreateCondition("ge",
        [
            new ParameterExpression("version"),
            new StringLiteral { Value = "1.0" }
        ]);

        _parameters["version"] = "1.0";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region Variable Tests

    [Fact]
    public void EvaluateCondition_WithVariableExpression_ResolvesVariable()
    {
        // Condition: eq(variables.environment, 'production')
        var condition = CreateCondition("eq", [
            new VariableExpression("environment"),
            new StringLiteral { Value = "production" }
        ]);

        _variables["environment"] = "production";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region Complex Logic Tests

    [Fact]
    public void EvaluateCondition_WithNestedAndOr_EvaluatesCorrectly()
    {
        // Condition: and(eq(parameters.a, 'x'), or(eq(parameters.b, 'y'), eq(parameters.c, 'z')))
        var condition = CreateCondition("and",
            [
                new FunctionExpression("eq", [
                    new ParameterExpression("a"),
                    new StringLiteral { Value = "x" }
                ]),
                new FunctionExpression("or", [
                    new FunctionExpression("eq", [

                            new ParameterExpression("b"),
                            new StringLiteral { Value = "y" }
                        ]
                    ),
                    new FunctionExpression("eq",
                        [
                            new ParameterExpression("c"),
                            new StringLiteral { Value = "z" }
                        ]
                    )
                ])
            ]
        );

        _parameters["a"] = "x";
        _parameters["b"] = "not-y";
        _parameters["c"] = "z";

        // a='x' (true) AND (b='y' (false) OR c='z' (true)) = true AND true = true
        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithMultipleAndConditions_EvaluatesAll()
    {
        // Condition: and(eq(parameters.a, 'x'), eq(parameters.b, 'y'), eq(parameters.c, 'z'))
        var condition = CreateCondition(
            "and",
            [
                new FunctionExpression("eq", [
                        new ParameterExpression("a"),
                        new StringLiteral { Value = "x" }
                    ]
                ),
                new FunctionExpression("eq",
                    [
                        new ParameterExpression("b"),
                        new StringLiteral { Value = "y" }
                    ]
                ),
                new FunctionExpression("eq",
                    [
                        new ParameterExpression("c"),
                        new StringLiteral { Value = "z" }
                    ]
                )
            ]);

        _parameters["a"] = "x";
        _parameters["b"] = "y";
        _parameters["c"] = "z";

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void EvaluateCondition_WithUnknownFunction_ThrowsInvalidOperationException()
    {
        var condition = CreateCondition("unknownFunc", []);

        var ex = Assert.Throws<InvalidOperationException>(
            () => ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables)
        );
        Assert.Contains("Unknown function", ex.Message);
    }

    [Fact]
    public void EvaluateCondition_WithWrongParameterCount_ThrowsInvalidOperationException()
    {
        var condition = CreateCondition("eq",
            new List<Expression>
            {
                new StringLiteral { Value = "only-one-param" }
            });

        var ex = Assert.Throws<InvalidOperationException>(
            () => ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables)
        );
        Assert.Contains("requires exactly 2 parameters", ex.Message);
    }

    [Fact]
    public void EvaluateCondition_WithMissingParameter_ThrowsKeyNotFoundException()
    {
        var condition = CreateCondition("eq",
            new List<Expression>
            {
                new ParameterExpression("nonexistent"),
                new StringLiteral { Value = "value" }
            });

        var ex = Assert.Throws<KeyNotFoundException>(
            () => ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables)
        );
    }

    [Fact]
    public void EvaluateCondition_WithInvalidNumberForNumericComparison_ThrowsInvalidOperationException()
    {
        var condition = CreateCondition("lt",
            new List<Expression>
            {
                new ParameterExpression("version"),
                new StringLiteral { Value = "2.0" }
            });

        _parameters["version"] = "not-a-number";

        var ex = Assert.Throws<InvalidOperationException>(
            () => ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables)
        );
        Assert.Contains("Cannot convert", ex.Message);
    }

    #endregion

    #region Empty Condition Tests

    [Fact]
    public void EvaluateCondition_WithNullCondition_ReturnsTrue()
    {
        TemplateExpression? condition = null;

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition!, _parameters, _variables));
    }

    [Fact]
    public void EvaluateCondition_WithEmptyChildren_ReturnsTrue()
    {
        var condition = new TemplateExpression { Children = [] };

        Assert.True(ExpressionEvaluator.EvaluateCondition(condition, _parameters, _variables));
    }

    #endregion

    #region Helper Methods

    private static TemplateExpression CreateCondition(string name, IList<Expression> parameters)
    {
        return new TemplateExpression
        {
            Children = new List<Expression> { new FunctionExpression(name, parameters) }
        };
    }

    #endregion
}
