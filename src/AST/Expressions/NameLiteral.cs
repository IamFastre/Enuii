using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class NameLiteral(Token token)
    : Expression
{
    public string Value { get; } = token.Value!;

    public override NodeKind Kind { get; } = NodeKind.Identifier;
    public override Span     Span { get; } = token.Span;
}
