namespace AdoPipelineTest.Parsing.Ast;

public class ConditionalStepExpression : PipelineStepElement
{
    public required TemplateExpression Condition { get; init; }
    
    /// <summary>
    /// Steps to execute when the condition is true
    /// </summary>
    public IList<PipelineStepElement> ThenSteps { get; init; } = [];
    
    /// <summary>
    /// Else branch - can be another ConditionalStepExpression (for else-if) or regular steps
    /// </summary>
    public PipelineStepElement? ElseBranch { get; init; }
}