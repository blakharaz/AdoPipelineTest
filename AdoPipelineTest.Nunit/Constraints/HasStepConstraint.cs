using AdoPipelineTest.Model;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class HasStepConstraint(string stepDisplayName) : Constraint
{
    public override string Description => $"Job has step with display name '{stepDisplayName}'";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is PipelineJob job)
        {
            var hasStep = job.Steps.Any(s => s.DisplayName == stepDisplayName);
            return new ConstraintResult(this, actual,
                hasStep ? ConstraintStatus.Success : ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, ConstraintStatus.Failure);
    }
}
