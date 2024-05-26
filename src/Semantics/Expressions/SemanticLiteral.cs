using Enuii.General.Positioning;
using Enuii.Symbols.Typing;

namespace Enuii.Semantics;

public sealed class SemanticLiteral(string value, TypeSymbol type, Span span)
    : SemanticExpression(type)
{
    public string Value { get; } = value;

    public override SemanticKind Kind { get; } = SemanticKind.Literal;
    public override Span         Span { get; } = span;
}
