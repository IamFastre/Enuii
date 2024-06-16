using Enuii.General.Positioning;
using Enuii.Symbols.Names;

namespace Enuii.Semantics;

public class SemanticAssignmentExpression(NameSymbol name, SemanticExpression expr, Span span)
    : SemanticExpression(name?.Type ?? expr.Type)
{
    public NameSymbol         Name       { get; } = name;
    public SemanticExpression Expression { get; } = expr;

    public override SemanticKind Kind { get; } = SemanticKind.AssignmentExpression;
    public override Span         Span { get; } = span;
}
