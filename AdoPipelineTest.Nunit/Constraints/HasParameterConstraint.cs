using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class HasParameterConstraint(string parameterName) : Constraint
{
    public override string Description => $"Pipeline has parameter '{parameterName}'";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is PipelineTestResult result)
        {
            var hasParameter = result.Parameters.ContainsKey(parameterName);
            return new ConstraintResult(this, actual,
                hasParameter ? ConstraintStatus.Success : ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, ConstraintStatus.Failure);
    }
}
