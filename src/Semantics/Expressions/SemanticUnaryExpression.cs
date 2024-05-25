using Enuii.General.Positioning;
using Enuii.Symbols.Typing;

namespace Enuii.Semantics;

public sealed class SemanticUnaryExpression(SemanticExpression operand, UnaryOperationKind kind, TypeSymbol result, Span span)
    : SemanticExpression(result)
{
    public SemanticExpression Operand       { get; } = operand;
    public UnaryOperationKind OperationKind { get; } = kind;

    public override SemanticKind Kind { get; } = SemanticKind.UnaryExpression;
    public override Span         Span { get; } = span;
}
