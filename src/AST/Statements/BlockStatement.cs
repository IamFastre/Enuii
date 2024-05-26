using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class BlockStatement(Token open, IEnumerable<Statement> statements, Token close)
    : Statement
{
    public Token       Open  { get; } = open;
    public Statement[] Body  { get; } = statements.ToArray();
    public Token       Close { get; } = close;

    public override NodeKind Kind { get; } = NodeKind.BlockStatement;
    public override Span     Span { get; } = open.Span.To(close.Span);
}
