using Enuii.General.Positioning;

namespace Enuii.Syntax.AST;

public sealed class NullForgivingExpression(Expression expr, Span endSpan)
    : Expression
{
    public Expression Expression { get; } = expr;

    public override NodeKind Kind { get; } = NodeKind.NullForgivingExpression;
    public override Span     Span { get; } = expr.Span.To(endSpan);
}
