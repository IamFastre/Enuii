using Enuii.Symbols.Types;
using Enuii.General.Positioning;

namespace Enuii.Semantics;

public sealed class SemanticNullForgivingExpression(SemanticExpression expr, TypeSymbol type, Span span)
    : SemanticExpression(type)
{
    public SemanticExpression Expression { get; } = expr;

    public override SemanticKind Kind { get; } = SemanticKind.NullForgivingExpression;
    public override Span         Span { get; } = span;
}
