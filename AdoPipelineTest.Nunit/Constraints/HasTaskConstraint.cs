using AdoPipelineTest.Model;
using AdoPipelineTest.Model.Steps;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class HasTaskConstraint(string taskName) : Constraint
{
    public override string Description => $"Job has task '{taskName}'";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is PipelineJob job)
        {
            var hasTask = job.Steps
                .OfType<TaskStep>()
                .Any(s => s.TaskName == taskName);
            return new ConstraintResult(this, actual,
                hasTask ? ConstraintStatus.Success : ConstraintStatus.Failure);
        }

        return new ConstraintResult(this, actual, ConstraintStatus.Failure);
    }
}
