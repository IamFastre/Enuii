using Enuii.General.Positioning;
using Enuii.Symbols.Types;

namespace Enuii.Semantics;

public sealed class SemanticListLiteral(IEnumerable<SemanticExpression> exprs, TypeSymbol type, Span span)
    : SemanticExpression(TypeSymbol.List(type))
{
    public SemanticExpression[] Expressions { get; } = [..exprs];

    public override SemanticKind Kind { get; } = SemanticKind.List;
    public override Span         Span { get; } = span;
}
