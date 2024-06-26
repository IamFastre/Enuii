using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class ParenthesizedExpression(Token open, Expression expr, Token close)
    : Expression
{
    public Token      Open       { get; } = open;
    public Expression Expression { get; } = expr;
    public Token      Close      { get; } = close;

    public override NodeKind Kind { get; } = NodeKind.ParenthesizedExpression;
    public override Span     Span { get; } = open.Span.To(close.Span);
}
