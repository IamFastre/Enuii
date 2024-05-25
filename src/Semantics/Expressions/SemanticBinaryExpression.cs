using Enuii.General.Positioning;
using Enuii.Symbols.Typing;

namespace Enuii.Semantics;

public class SemanticBinaryExpression(SemanticExpression left, SemanticExpression right, BinaryOperationKind kind, TypeSymbol result, Span span)
    : SemanticExpression(result)
{
    public SemanticExpression  LHS           { get; } = left;
    public SemanticExpression  RHS           { get; } = right;
    public BinaryOperationKind OperationKind { get; } = kind;

    public override SemanticKind Kind { get; } = SemanticKind.BinaryExpression;
    public override Span         Span { get; } = span;
}
