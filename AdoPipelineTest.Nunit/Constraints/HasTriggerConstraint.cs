using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class HasTriggerConstraint : Constraint
{
    public override string Description => "Pipeline has triggers configured";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is AdoPipelineTest.Model.PipelineTriggers triggers)
        {
            var hasTriggers = triggers.IncludedBranches.Count > 0;
            return new ConstraintResult(this, actual,
                hasTriggers ? ConstraintStatus.Success : ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, ConstraintStatus.Failure);
    }
}
