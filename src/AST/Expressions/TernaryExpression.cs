using Enuii.General.Positioning;

namespace Enuii.Syntax.AST;

public sealed class TernaryExpression(Expression condition, Expression trueExpr, Expression falseExpr)
    : Expression
{
    public Expression Condition       { get; } = condition;
    public Expression TrueExpression  { get; } = trueExpr;
    public Expression FalseExpression { get; } = falseExpr;

    public override NodeKind Kind { get; } = NodeKind.TernaryExpression;
    public override Span     Span { get; } = condition.Span.To(falseExpr.Span);
}
