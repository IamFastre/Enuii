using Enuii.General.Positioning;
using Enuii.Symbols.Typing;

namespace Enuii.Semantics;

public sealed class SemanticConstantLiteral(string value, TypeSymbol type, Span span)
    : SemanticExpression(type)
{
    public string Value { get; } = value;

    public override SemanticKind Kind { get; } = SemanticKind.Constant;
    public override Span         Span { get; } = span;
}
