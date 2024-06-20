using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public class ParameterClause(Token name, TypeClause type, Expression? value)
    : Clause
{
    public Token       Name  { get; } = name;
    public TypeClause  Type  { get; } = type;
    public Expression? Value { get; } = value;

    public override NodeKind Kind { get; } = NodeKind.ParameterClause;
    public override Span     Span { get; } = name.Span.To(type.Span);
}
