using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class Literal(Token token, NodeKind kind)
    : Expression
{
    public string Value { get; } = token.Value!;

    public override NodeKind Kind { get; } = kind;
    public override Span     Span { get; } = token.Span;

    public static Literal Fabricate(Span span)
        => new(Token.Fabricate(span), NodeKind.Unknown);
}
