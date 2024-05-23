using Enuii.General.Positioning;

namespace Enuii.Semantics;

public sealed class SemanticExpressionStatement(SemanticExpression expression)
    : SemanticStatement
{
    public SemanticExpression Expression { get; } = expression;

    public override Span         Span => Expression.Span;
    public override SemanticKind Kind { get; } = SemanticKind.ExpressionStatement;
}
