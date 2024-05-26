using Enuii.General.Positioning;

namespace Enuii.Semantics;

public sealed class SemanticBlockStatement(IEnumerable<SemanticStatement> statements, Span span)
    : SemanticStatement
{
    public SemanticStatement[] Body { get; } = statements.ToArray();

    public override SemanticKind Kind { get; } = SemanticKind.BlockStatement;
    public override Span         Span { get; } = span;
}
