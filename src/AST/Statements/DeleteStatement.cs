using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class DeleteStatement(Token delKeyword, Token name)
    : Statement
{
    public Token Delete { get; } = delKeyword;
    public Token Name   { get; } = name;

    public override NodeKind Kind { get; } = NodeKind.DeleteStatement;
    public override Span     Span { get; } = delKeyword.Span.To(name.Span);
}
