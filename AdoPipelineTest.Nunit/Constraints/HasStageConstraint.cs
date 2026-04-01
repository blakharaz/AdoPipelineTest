using AdoPipelineTest.Model;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class HasStageConstraint(string stageName) : Constraint
{
    public override string Description => $"Pipeline has stage '{stageName}'";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is PipelineTestResult result)
        {
            var hasStage = result.Stages.Any(s => s.Name == stageName || s.DisplayName == stageName);
            return new ConstraintResult(this, actual,
                hasStage ? ConstraintStatus.Success : ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, ConstraintStatus.Failure);
    }
}
