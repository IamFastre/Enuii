using Enuii.General.Positioning;

namespace Enuii.Syntax.AST;

public sealed class ExpressionStatement(Expression expression) : Statement
{
    public Expression Expression  { get; } = expression;

    public override Span     Span => Expression.Span;
    public override NodeKind Kind => NodeKind.ExpressionStatement;
}
