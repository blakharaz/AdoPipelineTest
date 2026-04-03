using NUnit.Framework;
using AdoPipelineTest.Evaluation;
using AdoPipelineTest.Parsing.Ast;

namespace AdoPipelineTest.UnitTests.Evaluation;

[TestFixture]
public class ParameterEvaluatorTest
{
    [Test]
    public void EvaluateParameters_WithDefaultValues_UsesDefaults()
    {
        // Arrange
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "projectName",
                Type = "string",
                DisplayName = "Project Name",
                DefaultValue = "MySampleProject"
            },
            new()
            {
                Name = "enableTests",
                Type = "boolean",
                DisplayName = "Enable Unit Tests",
                DefaultValue = true
            }
        };
        var parameterValues = new Dictionary<string, object>();

        // Act
        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        var projectName = result.First(p => p.Name == "projectName");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(projectName.Value, Is.EqualTo("MySampleProject"));
            Assert.That(projectName.DisplayName, Is.EqualTo("Project Name"));
            Assert.That(projectName.DefaultValue, Is.EqualTo("MySampleProject"));
        }

        var enableTests = result.First(p => p.Name == "enableTests");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(enableTests.Value, Is.EqualTo(true));
            Assert.That(enableTests.DefaultValue, Is.EqualTo(true));
        }
    }

    [Test]
    public void EvaluateParameters_WithSuppliedValues_OverridesDefaults()
    {
        // Arrange
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "projectName",
                Type = "string",
                DefaultValue = "DefaultProject"
            },
            new()
            {
                Name = "buildConfiguration",
                Type = "string",
                DefaultValue = "Release"
            }
        };
        var parameterValues = new Dictionary<string, object>
        {
            { "projectName", "CustomProject" },
            { "buildConfiguration", "Debug" }
        };

        // Act
        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        var projectName = result.First(p => p.Name == "projectName");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(projectName.Value, Is.EqualTo("CustomProject"));
            Assert.That(projectName.DefaultValue, Is.EqualTo("DefaultProject"));
        }

        var buildConfig = result.First(p => p.Name == "buildConfiguration");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(buildConfig.Value, Is.EqualTo("Debug"));
            Assert.That(buildConfig.DefaultValue, Is.EqualTo("Release"));
        }
    }

    [Test]
    public void EvaluateParameters_WithNumericDefaults_PreservesType()
    {
        // Arrange
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "timeoutMinutes",
                Type = "number",
                DefaultValue = 30
            },
            new()
            {
                Name = "retryCount",
                Type = "number",
                DefaultValue = 3
            }
        };
        var parameterValues = new Dictionary<string, object>();

        // Act
        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        // Assert
        var timeout = result.First(p => p.Name == "timeoutMinutes");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(timeout.Value, Is.EqualTo(30));
            Assert.That(timeout.DefaultValue, Is.EqualTo(30));
        }
    }

    [Test]
    public void EvaluateParameters_WithAllowedValues_IncludesConstraints()
    {
        // Arrange
        var allowedConfigs = new List<object> { "Debug", "Release", "CI" };
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "buildConfiguration",
                Type = "string",
                DefaultValue = "Release",
                AllowedValues = allowedConfigs
            }
        };
        var parameterValues = new Dictionary<string, object>();

        // Act
        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        // Assert
        var buildConfig = result.First();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(buildConfig.AllowedValues, Is.Not.Null);
            Assert.That(buildConfig.AllowedValues, Has.Count.EqualTo(3));
            Assert.That(buildConfig.AllowedValues, Contains.Item("Debug"));
            Assert.That(buildConfig.AllowedValues, Contains.Item("Release"));
            Assert.That(buildConfig.AllowedValues, Contains.Item("CI"));
        }
    }

    [Test]
    public void EvaluateParameters_WithObjectDefaults_PreservesObjectType()
    {
        // Arrange
        var defaultSettings = new Dictionary<object, object>();
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "buildSettings",
                Type = "object",
                DefaultValue = defaultSettings
            }
        };
        var parameterValues = new Dictionary<string, object>();

        // Act
        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        // Assert
        var buildSettings = result.First();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(buildSettings.Value, Is.InstanceOf<Dictionary<object, object>>());
            Assert.That(buildSettings.DefaultValue, Is.InstanceOf<Dictionary<object, object>>());
        }
    }

    [Test]
    public void EvaluateParameters_WithEnvironmentVariableDefault_PreservesAsString()
    {
        // Arrange
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "outputDirectory",
                Type = "string",
                DefaultValue = "$(Build.ArtifactStagingDirectory)"
            }
        };
        var parameterValues = new Dictionary<string, object>();

        // Act
        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        // Assert
        var outputDir = result.First();
        Assert.That(outputDir.Value, Is.EqualTo("$(Build.ArtifactStagingDirectory)"));
    }

    [Test]
    public void EvaluateParameters_WithMixedProvidedAndDefault_UsesCorrectValues()
    {
        // Arrange
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "projectName",
                Type = "string",
                DefaultValue = "DefaultProject"
            },
            new()
            {
                Name = "buildConfiguration",
                Type = "string",
                DefaultValue = "Release"
            },
            new()
            {
                Name = "timeoutMinutes",
                Type = "number",
                DefaultValue = 30
            }
        };
        var parameterValues = new Dictionary<string, object>
        {
            { "projectName", "CustomProject" }
            // buildConfiguration and timeoutMinutes are not provided
        };

        // Act
        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        
        var projectName = result.First(p => p.Name == "projectName");
        Assert.That(projectName.Value, Is.EqualTo("CustomProject"));
        
        var buildConfig = result.First(p => p.Name == "buildConfiguration");
        Assert.That(buildConfig.Value, Is.EqualTo("Release"));
        
        var timeout = result.First(p => p.Name == "timeoutMinutes");
        Assert.That(timeout.Value, Is.EqualTo(30));
    }

    [Test]
    public void EvaluateParameters_WithNullDefault_ValueIsNull()
    {
        // Arrange
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "optionalField",
                Type = "string",
                DefaultValue = null
            }
        };
        var parameterValues = new Dictionary<string, object>();

        // Act
        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        // Assert
        var optionalField = result.First();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(optionalField.Value, Is.Null);
            Assert.That(optionalField.DefaultValue, Is.Null);
        }
    }

    [Test]
    public void EvaluateParameters_WithEmptyParameterList_ReturnsEmpty()
    {
        // Arrange
        var rawParameters = new List<PipelineParameterElement>();
        var parameterValues = new Dictionary<string, object>();

        // Act
        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void EvaluateParameters_PreservesDisplayName()
    {
        // Arrange
        var rawParameters = new List<PipelineParameterElement>
        {
            new()
            {
                Name = "projectName",
                Type = "string",
                DisplayName = "Project Name",
                DefaultValue = "MySampleProject"
            }
        };
        var parameterValues = new Dictionary<string, object>();

        // Act
        var result = ParameterEvaluator.EvaluateParameters(rawParameters, parameterValues);

        // Assert
        var projectName = result.First();
        Assert.That(projectName.DisplayName, Is.EqualTo("Project Name"));
    }
}