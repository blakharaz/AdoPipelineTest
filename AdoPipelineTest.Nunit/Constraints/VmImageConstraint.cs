using AdoPipelineTest.Model;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class VmImageConstraint(string imageName) : Constraint
{
    public override string Description => $"VM Image is {imageName}";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is PipelineAgentPool agentPool)
        {
            return new ConstraintResult(this, actual,
                agentPool.VmImage == imageName 
                    ? ConstraintStatus.Success 
                    : ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, ConstraintStatus.Failure);
    }
}