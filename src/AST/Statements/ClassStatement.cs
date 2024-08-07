using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class ClassStatement(Token symbol, Token name, SeparatedClause<ParameterClause> parameters, ClassBodyClause body)
    : Statement
{
    public Token                            Symbol     { get; } = symbol;
    public Token                            Name       { get; } = name;
    public SeparatedClause<ParameterClause> Parameters { get; } = parameters;
    public ClassBodyClause                  Body       { get; } = body;

    public override NodeKind Kind { get; } = NodeKind.ClassStatement;
    public override Span     Span { get; } = symbol.Span.To(body.Span);
}
