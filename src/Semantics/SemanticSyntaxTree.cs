using Enuii.General.Positioning;

namespace Enuii.Semantics;

public class SemanticTree(IEnumerable<SemanticStatement> body)
{
    public SemanticStatement[] Body { get; } = body.ToArray();
    public Span                Span { get; } = body.Any()
                                             ? body.First().Span.To(body.Last().Span)
                                             : new();
}
