using Enuii.General.Positioning;

namespace Enuii.Syntax.AST;

public sealed class ExpressionStatement(Expression expression)
    : Statement
{
    public Expression Expression  { get; } = expression;

    public override NodeKind Kind { get; } = NodeKind.ExpressionStatement;
    public override Span     Span => Expression.Span;
}
