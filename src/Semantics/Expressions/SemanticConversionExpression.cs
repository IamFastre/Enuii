using Enuii.General.Positioning;
using Enuii.Symbols.Typing;

namespace Enuii.Semantics;

public sealed class SemanticConversionExpression(SemanticExpression expr, TypeSymbol dest, ConversionKind cvKind, Span span)
    : SemanticExpression(dest)
{
    public SemanticExpression Expr   { get; } = expr;
    public TypeSymbol         Dest   { get; } = dest;
    public ConversionKind     CvKind { get; } = cvKind;

    public override SemanticKind Kind { get; } = SemanticKind.ConversionExpression;
    public override Span         Span { get; } = span;
}
