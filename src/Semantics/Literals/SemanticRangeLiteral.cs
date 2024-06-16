using Enuii.General.Positioning;
using Enuii.Symbols.Types;

namespace Enuii.Semantics;

public sealed class SemanticRangeLiteral(SemanticExpression? start, SemanticExpression? end, SemanticExpression? step, Span span)
    : SemanticExpression(TypeSymbol.Range)
{
    public SemanticExpression? Start { get; } = start;
    public SemanticExpression? End   { get; } = end;
    public SemanticExpression? Step  { get; } = step;

    public override SemanticKind Kind { get; } = SemanticKind.Range;
    public override Span         Span { get; } = span;
}
