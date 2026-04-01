using AdoPipelineTest.Nunit.Constraints;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit;

public abstract class Is: NUnit.Framework.Is
{
    public static IConstraint BranchIncluded(string branchName)
    {
        return new TriggersIncludeBranchConstraint(branchName);
    }

    public static IConstraint VmImage(string imageName)
    {
        return new VmImageConstraint(imageName);
    }

    public static IConstraint HasStage(string stageName)
    {
        return new HasStageConstraint(stageName);
    }

    public static IConstraint HasJob(string jobName)
    {
        return new HasJobConstraint(jobName);
    }

    public static IConstraint HasStep(string stepDisplayName)
    {
        return new HasStepConstraint(stepDisplayName);
    }

    public static IConstraint HasVariable(string variableName)
    {
        return new HasVariableConstraint(variableName);
    }

    public static IConstraint HasTask(string taskName)
    {
        return new HasTaskConstraint(taskName);
    }

    public static IConstraint HasTrigger()
    {
        return new HasTriggerConstraint();
    }

    public static IConstraint DependsOn(string dependencyName)
    {
        return new DependsOnConstraint(dependencyName);
    }

    public static IConstraint HasParameter(string parameterName)
    {
        return new HasParameterConstraint(parameterName);
    }

    public static IConstraint HasResource(string resourceType)
    {
        return new HasResourceConstraint(resourceType);
    }
}