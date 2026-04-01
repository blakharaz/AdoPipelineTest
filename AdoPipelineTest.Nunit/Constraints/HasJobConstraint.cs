using AdoPipelineTest.Model;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class HasJobConstraint(string jobName) : Constraint
{
    public override string Description => $"Stage has job '{jobName}'";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is PipelineStage stage)
        {
            var hasJob = stage.Jobs.Any(j => j.Name == jobName || j.DisplayName == jobName);
            return new ConstraintResult(this, actual,
                hasJob ? ConstraintStatus.Success : ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, ConstraintStatus.Failure);
    }
}
