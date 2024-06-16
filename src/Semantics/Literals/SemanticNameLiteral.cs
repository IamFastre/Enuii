using Enuii.General.Positioning;
using Enuii.Symbols.Names;

namespace Enuii.Semantics;

public class SemanticNameLiteral(NameSymbol name, Span span)
    : SemanticExpression(name.Type)
{
    public string Name { get; } = name.Name;

    public override SemanticKind Kind { get; } = SemanticKind.Name;
    public override Span         Span { get; } = span;
}
