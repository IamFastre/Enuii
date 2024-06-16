using Enuii.General.Positioning;

namespace Enuii.Syntax.AST;

public class AssignmentExpression(NameLiteral assignee, Expression expr)
    : Expression
{
    public NameLiteral Assignee   { get; } = assignee;
    public Expression  Expression { get; } = expr;

    public override NodeKind Kind { get; } = NodeKind.AssignmentExpression;
    public override Span     Span { get; } = assignee.Span.To(expr.Span);
}
