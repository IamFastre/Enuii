using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public class FunctionStatement(Token symbol, bool isConst, Token function, SeparatedClause<ParameterClause> parameters, TypeClause? returnType, Statement body)
    : Statement
{
    public Token                            Symbol     { get; } = symbol;
    public bool                             IsConstant { get; } = isConst;
    public Token                            Function   { get; } = function;
    public SeparatedClause<ParameterClause> Parameters { get; } = parameters;
    public TypeClause?                      ReturnType { get; } = returnType;
    public Statement                        Body       { get; } = body;

    public override NodeKind Kind { get; } = NodeKind.FunctionStatement;
    public override Span     Span { get; } = symbol.Span.To(body.Span);
}
