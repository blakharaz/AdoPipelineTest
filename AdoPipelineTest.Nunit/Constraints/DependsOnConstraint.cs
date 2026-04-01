using AdoPipelineTest.Model;
using NUnit.Framework.Constraints;

namespace AdoPipelineTest.Nunit.Constraints;

public class DependsOnConstraint(string dependencyName) : Constraint
{
    public override string Description => $"Depends on '{dependencyName}'";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        bool hasDependency = actual switch
        {
            PipelineStage stage => stage.DependsOn.Contains(dependencyName),
            PipelineJob job => job.DependsOn.Contains(dependencyName),
            _ => false
        };

        return new ConstraintResult(this, actual,
            hasDependency ? ConstraintStatus.Success : ConstraintStatus.Failure);
    }
}
