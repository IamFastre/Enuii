using Enuii.General.Positioning;

namespace Enuii.Semantics;

public sealed class SemanticDeleteStatement(string name, Span span)
    : SemanticStatement
{
    public string Name { get; } = name;

    public override SemanticKind Kind { get; } = SemanticKind.DeleteStatement;
    public override Span         Span { get; } = span;
}
