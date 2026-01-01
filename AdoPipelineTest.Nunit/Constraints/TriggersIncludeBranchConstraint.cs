using AdoPipelineTest.Model;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class TriggersIncludeBranchConstraint(string branchName) : Constraint
{
    public override string Description => $"Triggers include branch {branchName}";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is PipelineTriggers triggers)
        {
            return new ConstraintResult(this, actual,
                triggers.IncludedBranches.Contains(branchName) 
                    ? ConstraintStatus.Success 
                    : ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, ConstraintStatus.Failure);
    }
}