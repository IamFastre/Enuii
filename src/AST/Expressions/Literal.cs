using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class Literal(Token token, NodeKind kind) : Expression
{
    public string Value { get; } = token.Value!;

    public override Span     Span => token.Span;
    public override NodeKind Kind => kind;

    public static Literal Unknown(Span span)
        => new(new(null, TokenKind.Unknown, span), NodeKind.Unknown);
}
