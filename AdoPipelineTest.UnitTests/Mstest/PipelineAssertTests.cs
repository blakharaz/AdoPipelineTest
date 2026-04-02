using MsAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using CollectionAssert = Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert;
using TestClass = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestMethod = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
using TestInitialize = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using DataTestMethod = Microsoft.VisualStudio.TestTools.UnitTesting.DataTestMethodAttribute;
using DataRow = Microsoft.VisualStudio.TestTools.UnitTesting.DataRowAttribute;
using AssertFailedException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
using AdoPipelineTest.Mstest;
using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;

namespace AdoPipelineTest.UnitTests.Mstest;

[TestClass]
public class PipelineAssertTests
{
    private PipelineTestResult _result = null!;

    [TestInitialize]
    public void Setup()
    {
        _result = new PipelineTestResult
        {
            Triggers = new PipelineTriggers { IncludedBranches = ["main", "develop"] },
            AgentPool = new PipelineAgentPool { VmImage = "ubuntu-latest" },
            Variables =
            [
                new PipelineVariable { Name = "buildConfiguration", DefaultValue = "Release" },
                new PipelineVariable { Name = "version", DefaultValue = "1.0.0" }
            ],
            Parameters = new Dictionary<string, PipelineParameter>
            {
                ["projectName"] = new PipelineParameter { Name = "projectName", Value = "MyProject" },
                ["enableTests"] = new PipelineParameter { Name = "enableTests", Value = true }
            },
            Stages =
            [
                new PipelineStage
                {
                    Name = "Build",
                    DisplayName = "Build Stage",
                    Jobs =
                    [
                        new PipelineJob
                        {
                            Name = "BuildJob",
                            DisplayName = "Build Job",
                            Steps =
                            [
                                new TaskStep { DisplayName = "Use .NET", TaskName = "UseDotNet@2", Inputs = new Dictionary<string, string>() },
                                new TaskStep { DisplayName = "Restore", TaskName = "DotNetCoreCLI@2", Inputs = new Dictionary<string, string>() },
                                new ScriptStep { DisplayName = "Run Tests", Script = "dotnet test --configuration Release" }
                            ]
                        }
                    ]
                },
                new PipelineStage
                {
                    Name = "Deploy",
                    DisplayName = "Deploy Stage",
                    Jobs =
                    [
                        new PipelineJob
                        {
                            Name = "DeployJob",
                            DisplayName = "Deploy Job",
                            Steps =
                            [
                                new TaskStep { DisplayName = "Deploy", TaskName = "AzureWebApp@1", Inputs = new Dictionary<string, string>() }
                            ]
                        }
                    ]
                }
            ]
        };
    }

    #region HasStage

    [TestMethod]
    public void HasStage_ExistingStage_DoesNotThrow()
    {
        _result.HasStage("Build Stage");
        MsAssert.IsNotNull(_result.Stages.FirstOrDefault(s => s.DisplayName == "Build Stage"));
    }

    [TestMethod]
    public void HasStage_NonExistingStage_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasStage("NonExistent"));
    }

    [DataTestMethod]
    [DataRow("Build Stage")]
    [DataRow("Deploy Stage")]
    public void HasStage_AllExistingStages_DoesNotThrow(string stageName)
    {
        _result.HasStage(stageName);
        MsAssert.IsNotNull(_result.Stages.FirstOrDefault(s => s.DisplayName == stageName));
    }

    #endregion

    #region HasStageCount

    [TestMethod]
    public void HasStageCount_CorrectCount_DoesNotThrow()
    {
        _result.HasStageCount(2);
        MsAssert.AreEqual(2, _result.Stages.Count);
    }

    [TestMethod]
    public void HasStageCount_WrongCount_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasStageCount(5));
    }

    #endregion

    #region HasJob (on PipelineTestResult)

    [TestMethod]
    public void HasJob_ExistingJob_DoesNotThrow()
    {
        _result.HasJob("Build Stage", "Build Job");
        MsAssert.IsNotNull(_result.Stages.FirstOrDefault(s => s.DisplayName == "Build Stage")?.Jobs.FirstOrDefault(j => j.DisplayName == "Build Job"));
    }

    [TestMethod]
    public void HasJob_NonExistingStage_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasJob("NonExistent", "Build Job"));
    }

    [TestMethod]
    public void HasJob_NonExistingJob_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasJob("Build Stage", "NonExistent"));
    }

    #endregion

    #region HasJob (on PipelineStage)

    [TestMethod]
    public void HasJobOnStage_ExistingJob_DoesNotThrow()
    {
        var stage = _result.Stages[0];
        stage.HasJob("Build Job");
        MsAssert.IsNotNull(stage.Jobs.FirstOrDefault(j => j.DisplayName == "Build Job"));
    }

    [TestMethod]
    public void HasJobOnStage_NonExistingJob_ThrowsAssertFailedException()
    {
        var stage = _result.Stages[0];
        MsAssert.ThrowsExactly<AssertFailedException>(() => stage.HasJob("NonExistent"));
    }

    #endregion

    #region HasStep (by name)

    [TestMethod]
    public void HasStep_ExistingStep_DoesNotThrow()
    {
        _result.HasStep("Use .NET");
        MsAssert.IsNotNull(_result.Stages.SelectMany(s => s.Jobs).SelectMany(j => j.Steps).FirstOrDefault(s => s.DisplayName == "Use .NET"));
    }

    [TestMethod]
    public void HasStep_NonExistingStep_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasStep("NonExistent"));
    }

    #endregion

    #region HasStep (by predicate)

    [TestMethod]
    public void HasStepByPredicate_MatchingStep_DoesNotThrow()
    {
        _result.HasStep(s => s is ScriptStep);
        MsAssert.IsTrue(_result.Stages.SelectMany(s => s.Jobs).SelectMany(j => j.Steps).Any(s => s is ScriptStep));
    }

    [TestMethod]
    public void HasStepByPredicate_NoMatchingStep_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasStep(s => s.DisplayName == "NonExistent"));
    }

    #endregion

    #region HasTask

    [TestMethod]
    public void HasTask_ExistingTask_DoesNotThrow()
    {
        _result.HasTask("UseDotNet@2");
        MsAssert.IsTrue(_result.Stages.SelectMany(s => s.Jobs).SelectMany(j => j.Steps).OfType<TaskStep>().Any(t => t.TaskName == "UseDotNet@2"));
    }

    [TestMethod]
    public void HasTask_NonExistingTask_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasTask("NonExistent@1"));
    }

    [DataTestMethod]
    [DataRow("UseDotNet@2")]
    [DataRow("DotNetCoreCLI@2")]
    [DataRow("AzureWebApp@1")]
    public void HasTask_AllExistingTasks_DoesNotThrow(string taskName)
    {
        _result.HasTask(taskName);
        MsAssert.IsTrue(_result.Stages.SelectMany(s => s.Jobs).SelectMany(j => j.Steps).OfType<TaskStep>().Any(t => t.TaskName == taskName));
    }

    #endregion

    #region HasVariable (existence)

    [TestMethod]
    public void HasVariable_ExistingVariable_DoesNotThrow()
    {
        _result.HasVariable("buildConfiguration");
        MsAssert.IsNotNull(_result.Variables.FirstOrDefault(v => v.Name == "buildConfiguration"));
    }

    [TestMethod]
    public void HasVariable_NonExistingVariable_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasVariable("nonExistent"));
    }

    #endregion

    #region HasVariable (with value)

    [TestMethod]
    public void HasVariableWithCorrectValue_DoesNotThrow()
    {
        _result.HasVariable("buildConfiguration", "Release");
        MsAssert.AreEqual("Release", _result.Variables.FirstOrDefault(v => v.Name == "buildConfiguration")?.DefaultValue?.ToString());
    }

    [TestMethod]
    public void HasVariableWithWrongValue_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasVariable("buildConfiguration", "Debug"));
    }

    [TestMethod]
    public void HasVariable_NonExistingVariableWithValue_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasVariable("nonExistent", "value"));
    }

    #endregion

    #region HasParameter

    [TestMethod]
    public void HasParameter_ExistingParameter_DoesNotThrow()
    {
        _result.HasParameter("projectName");
        MsAssert.IsTrue(_result.Parameters.ContainsKey("projectName"));
    }

    [TestMethod]
    public void HasParameter_NonExistingParameter_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasParameter("nonExistent"));
    }

    [DataTestMethod]
    [DataRow("projectName")]
    [DataRow("enableTests")]
    public void HasParameter_AllExistingParameters_DoesNotThrow(string paramName)
    {
        _result.HasParameter(paramName);
        MsAssert.IsTrue(_result.Parameters.ContainsKey(paramName));
    }

    #endregion

    #region HasTrigger

    [TestMethod]
    public void HasTrigger_WithTriggers_DoesNotThrow()
    {
        _result.HasTrigger();
        MsAssert.IsNotNull(_result.Triggers);
    }

    [TestMethod]
    public void HasTrigger_NoTriggers_ThrowsAssertFailedException()
    {
        var resultWithoutTriggers = new PipelineTestResult
        {
            Stages = [],
            Variables = [],
            Parameters = new Dictionary<string, PipelineParameter>()
        };
        MsAssert.ThrowsExactly<AssertFailedException>(() => resultWithoutTriggers.HasTrigger());
    }

    #endregion

    #region TriggersIncludeBranch

    [TestMethod]
    public void TriggersIncludeBranch_ExistingBranch_DoesNotThrow()
    {
        _result.TriggersIncludeBranch("main");
        CollectionAssert.Contains(_result.Triggers!.IncludedBranches.ToList(), "main");
    }

    [TestMethod]
    public void TriggersIncludeBranch_NonExistingBranch_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.TriggersIncludeBranch("feature/xyz"));
    }

    [TestMethod]
    public void TriggersIncludeBranch_NoTriggers_ThrowsAssertFailedException()
    {
        var resultWithoutTriggers = new PipelineTestResult
        {
            Stages = [],
            Variables = [],
            Parameters = new Dictionary<string, PipelineParameter>()
        };
        MsAssert.ThrowsExactly<AssertFailedException>(() => resultWithoutTriggers.TriggersIncludeBranch("main"));
    }

    #endregion

    #region HasVmImage

    [TestMethod]
    public void HasVmImage_CorrectImage_DoesNotThrow()
    {
        _result.HasVmImage("ubuntu-latest");
        MsAssert.AreEqual("ubuntu-latest", _result.AgentPool?.VmImage);
    }

    [TestMethod]
    public void HasVmImage_WrongImage_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasVmImage("windows-latest"));
    }

    [TestMethod]
    public void HasVmImage_NoPool_ThrowsAssertFailedException()
    {
        var resultWithoutPool = new PipelineTestResult
        {
            Stages = [],
            Variables = [],
            Parameters = new Dictionary<string, PipelineParameter>()
        };
        MsAssert.ThrowsExactly<AssertFailedException>(() => resultWithoutPool.HasVmImage("ubuntu-latest"));
    }

    #endregion

    #region HasScriptStep

    [TestMethod]
    public void HasScriptStep_ExistingPattern_DoesNotThrow()
    {
        _result.HasScriptStep("dotnet test");
        MsAssert.IsTrue(_result.Stages.SelectMany(s => s.Jobs).SelectMany(j => j.Steps).OfType<ScriptStep>().Any(s => s.Script.Contains("dotnet test")));
    }

    [TestMethod]
    public void HasScriptStep_NonExistingPattern_ThrowsAssertFailedException()
    {
        MsAssert.ThrowsExactly<AssertFailedException>(() => _result.HasScriptStep("nonexistent pattern"));
    }

    [TestMethod]
    public void HasScriptStep_NoScriptSteps_ThrowsAssertFailedException()
    {
        var resultWithoutScripts = new PipelineTestResult
        {
            Stages =
            [
                new PipelineStage
                {
                    Jobs =
                    [
                        new PipelineJob
                        {
                            Steps =
                            [
                                new TaskStep { DisplayName = "Task", TaskName = "Task@1", Inputs = new Dictionary<string, string>() }
                            ]
                        }
                    ]
                }
            ],
            Variables = [],
            Parameters = new Dictionary<string, PipelineParameter>()
        };
        MsAssert.ThrowsExactly<AssertFailedException>(() => resultWithoutScripts.HasScriptStep("anything"));
    }

    #endregion
}
