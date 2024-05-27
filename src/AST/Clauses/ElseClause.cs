using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class ElseClause(Token elseKeyword, Statement body)
    : Clause
{
    public Token     Else { get; } = elseKeyword;
    public Statement Body { get; } = body;

    public override NodeKind Kind { get; } = NodeKind.ElseClause;
    public override Span     Span { get; } = elseKeyword.Span.To(body.Span);
}
