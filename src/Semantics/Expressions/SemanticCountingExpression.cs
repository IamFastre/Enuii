using Enuii.Semantics.Operations;
using Enuii.General.Positioning;

namespace Enuii.Semantics;

public class SemanticCountingExpression(SemanticNameLiteral operand, CountingKind kind, bool isBefore, Span span)
    : SemanticExpression(operand.Type)
{
    public SemanticNameLiteral Operand       { get; } = operand;
    public CountingKind        OperationKind { get; } = kind;
    public bool                IsBefore      { get; } = isBefore;


    public override SemanticKind Kind { get; } = SemanticKind.CountingExpression;
    public override Span         Span { get; } = span;
}
