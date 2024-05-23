using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class BinaryExpression(Expression leftHandExpr, Token binOperator, Expression rightHandExpr)
    : Expression
{
    public Expression LHS         { get; } = leftHandExpr;
    public Token      Operator    { get; } = binOperator;
    public Expression RHS         { get; } = rightHandExpr;

    public override Span     Span { get; } = new Span(leftHandExpr.Span.Start, rightHandExpr.Span.End);
    public override NodeKind Kind => NodeKind.BinaryExpression;
}
