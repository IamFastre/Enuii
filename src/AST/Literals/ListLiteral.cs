using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class ListLiteral(Token open, SeparatedClause<Expression> exprs, Token close)
    : Expression
{
    public Token                       Open     { get; } = open;
    public SeparatedClause<Expression> Elements { get; } = exprs;
    public Token                       Close    { get; } = close;

    public override NodeKind Kind { get; } = NodeKind.List;
    public override Span     Span { get; } = open.Span.To(close.Span);
}
