using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class HasResourceConstraint(string resourceType) : Constraint
{
    public override string Description => $"Pipeline has resource of type '{resourceType}'";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is PipelineTestResult result)
        {
            var hasResource = result.Resources.Any(r => r.Type == resourceType);
            return new ConstraintResult(this, actual,
                hasResource ? ConstraintStatus.Success : ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, ConstraintStatus.Failure);
    }
}
