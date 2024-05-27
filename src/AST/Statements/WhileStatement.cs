using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class WhileStatement(Token whileKeyword, Expression condition, Statement loopStmt, ElseClause? elseClause)
    : Statement
{
    public Token       While      { get; } = whileKeyword;
    public Expression  Condition  { get; } = condition;
    public Statement   Loop       { get; } = loopStmt;
    public ElseClause? ElseClause { get; } = elseClause;

    public override NodeKind Kind { get; } = NodeKind.WhileStatement;
    public override Span     Span { get; } = whileKeyword.Span.To(elseClause is not null
                                                                ? elseClause.Span
                                                                : loopStmt.Span);
}
