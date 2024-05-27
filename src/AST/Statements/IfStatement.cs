using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class IfStatement(Token ifKeyword, Expression condition, Statement thenStmt, ElseClause? elseClause)
    : Statement
{
    public Token       If         { get; } = ifKeyword;
    public Expression  Condition  { get; } = condition;
    public Statement   Then       { get; } = thenStmt;
    public ElseClause? ElseClause { get; } = elseClause;

    public override NodeKind Kind { get; } = NodeKind.IfStatement;
    public override Span     Span { get; } = ifKeyword.Span.To(elseClause is not null
                                                             ? elseClause.Span
                                                             : thenStmt.Span);
}
