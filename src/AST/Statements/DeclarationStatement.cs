using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public class DeclarationStatement(Token hash, bool isConst, Token name, TypeClause? type, Expression expr)
    : Statement
{
    public Token       Name       { get; } = name;
    public TypeClause? TypeClause { get; } = type;
    public Expression  Expression { get; } = expr;
    public bool        IsConstant { get; } = isConst;

    public override NodeKind Kind { get; } = NodeKind.DeclarationStatement;
    public override Span     Span { get; } = hash.Span.To(expr?.Span ?? type?.Span ?? name.Span);
}
