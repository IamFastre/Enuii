using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class ClassBodyClause(Token open, IEnumerable<Statement> statements, Token close)
    : Clause
{
    public Token       Open  { get; } = open;
    public Statement[] Body  { get; } = [..statements];
    public Token       Close { get; } = close;

    public override NodeKind Kind { get; } = NodeKind.ClassBodyClause;
    public override Span     Span { get; } = open.Span.To(close.Span);
}
