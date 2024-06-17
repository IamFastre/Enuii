using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class NameLiteral(Token token)
    : Expression
{
    public string Name { get; } = token.Value!;

    public override NodeKind Kind { get; } = NodeKind.Name;
    public override Span     Span { get; } = token.Span;
}
