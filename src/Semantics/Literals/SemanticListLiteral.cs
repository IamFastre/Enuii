using Enuii.General.Positioning;
using Enuii.Symbols.Typing;

namespace Enuii.Semantics;

public sealed class SemanticListLiteral(IEnumerable<SemanticExpression> exprs, TypeSymbol type, Span span)
    : SemanticExpression(TypeSymbol.List.SetParameters(type))
{
    public SemanticExpression[] Expressions { get; } = [..exprs];

    public override SemanticKind Kind { get; } = SemanticKind.List;
    public override Span         Span { get; } = span;
}
