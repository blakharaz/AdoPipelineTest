using AdoPipelineTest.Model;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class HasVariableConstraint(string variableName) : Constraint
{
    public override string Description => $"Pipeline has variable '{variableName}'";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is PipelineTestResult result)
        {
            var hasVariable = result.Variables.Any(v => v.Name == variableName);
            return new ConstraintResult(this, actual,
                hasVariable ? ConstraintStatus.Success : ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, ConstraintStatus.Failure);
    }
}
