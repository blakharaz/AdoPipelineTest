using AdoPipelineTest.Nunit.Constraints;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit;

public abstract class Has: NUnit.Framework.Has
{
    public static IConstraint Stage(string stageName)
    {
        return new HasStageConstraint(stageName);
    }

    public static IConstraint Job(string jobName)
    {
        return new HasJobConstraint(jobName);
    }

    public static IConstraint Step(string stepDisplayName)
    {
        return new HasStepConstraint(stepDisplayName);
    }

    public static IConstraint Variable(string variableName)
    {
        return new HasVariableConstraint(variableName);
    }

    public static IConstraint Task(string taskName)
    {
        return new HasTaskConstraint(taskName);
    }

    public static IConstraint Parameter(string parameterName)
    {
        return new HasParameterConstraint(parameterName);
    }

    public static IConstraint Resource(string resourceType)
    {
        return new HasResourceConstraint(resourceType);
    }
    

    public static IConstraint Trigger()
    {
        return new HasTriggerConstraint();
    }
}