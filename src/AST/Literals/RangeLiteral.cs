using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class RangeLiteral(Token open, Expression? start, Expression? end, Expression? step, Token close)
    : Expression
{
    public Token       Open  { get; } = open;
    public Expression? Start { get; } = start;
    public Expression? End   { get; } = end;
    public Expression? Step  { get; } = step;
    public Token       Close { get; } = close;

    public override NodeKind Kind { get; } = NodeKind.Range;
    public override Span     Span { get; } = open.Span.To(close.Span);
}
