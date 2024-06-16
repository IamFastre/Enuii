using Enuii.General.Positioning;
using Enuii.Symbols.Types;

namespace Enuii.Semantics;

public sealed class SemanticConversionExpression(SemanticExpression expr, TypeSymbol dest, ConversionKind cvKind, Span span)
    : SemanticExpression(dest)
{
    public SemanticExpression Expression    { get; } = expr;
    public TypeSymbol         Destination   { get; } = dest;
    public ConversionKind     OperationKind { get; } = cvKind;

    public override SemanticKind Kind { get; } = SemanticKind.ConversionExpression;
    public override Span         Span { get; } = span;
}
