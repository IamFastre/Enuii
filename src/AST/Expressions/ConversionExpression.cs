using Enuii.General.Positioning;

namespace Enuii.Syntax.AST;

public sealed class ConversionExpression(Expression expr, TypeClause dest)
    : Expression
{
    public Expression Expression  { get; } = expr;
    public TypeClause Destination { get; } = dest;

    public override NodeKind Kind { get; } = NodeKind.ConversionExpression;
    public override Span     Span { get; } = expr.Span.To(dest.Span);
}
