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
}