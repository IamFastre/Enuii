using Enuii.General.Positioning;
using Enuii.Symbols.Types;

namespace Enuii.Semantics;

public class SemanticNameLiteral(string name, TypeSymbol type, Span span)
    : SemanticExpression(type)
{
    public string Name { get; } = name;

    public override SemanticKind Kind { get; } = SemanticKind.Name;
    public override Span         Span { get; } = span;
}
