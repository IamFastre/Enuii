using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class NameLiteral(Token token)
    : Expression
{
    public string Value { get; } = token.Value!;

    public override Span     Span => token.Span;
    public override NodeKind Kind => NodeKind.Identifier;
}
