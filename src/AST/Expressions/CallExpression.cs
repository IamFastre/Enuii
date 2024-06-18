using Enuii.General.Positioning;

namespace Enuii.Syntax.AST;

public class CallExpression(Expression callee, SeparatedClause<Expression> args, Span end)
    : Expression
{
    public Expression                  Callee    { get; } = callee;
    public SeparatedClause<Expression> Arguments { get; } = args;

    public override NodeKind Kind { get; } = NodeKind.CallExpression;
    public override Span     Span { get; } = callee.Span.To(end);
}
