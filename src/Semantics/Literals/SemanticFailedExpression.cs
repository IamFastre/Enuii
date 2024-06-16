using Enuii.General.Positioning;
using Enuii.Symbols.Types;

namespace Enuii.Semantics;

public class SemanticFailedExpression(Span span)
    : SemanticExpression(TypeSymbol.Unknown)
{
    public override SemanticKind Kind { get; } = SemanticKind.FailedExpression;
    public override Span         Span { get; } = span;
}
