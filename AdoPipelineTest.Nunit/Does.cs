using AdoPipelineTest.Nunit.Constraints;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit;

public abstract class Does : NUnit.Framework.Does
{
    public static IConstraint DependOn(string dependencyName)
    {
        return new DependsOnConstraint(dependencyName);
    }
}