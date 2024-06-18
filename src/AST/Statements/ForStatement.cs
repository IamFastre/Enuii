using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public class ForStatement(Token forKeyword, Token variable, Expression iterable, Statement loopStmt, ElseClause? elseClause)
    : Statement
{
    public Token       Variable   { get; } = variable;
    public Expression  Iterable   { get; } = iterable;
    public Statement   Loop       { get; } = loopStmt;
    public ElseClause? ElseClause { get; } = elseClause;

    public override NodeKind Kind { get; } = NodeKind.ForStatement;
    public override Span     Span { get; } = forKeyword.Span.To(elseClause is not null
                                                              ? elseClause.Span
                                                              : loopStmt.Span);
}
