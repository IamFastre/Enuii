using Enuii.General.Positioning;
using Enuii.Symbols.Types;

namespace Enuii.Semantics;

public class SemanticAssignmentExpression(string name, SemanticExpression expr, TypeSymbol type, Span span)
    : SemanticExpression(type)
{
    public string             Name       { get; } = name;
    public SemanticExpression Expression { get; } = expr;

    public override SemanticKind Kind { get; } = SemanticKind.AssignmentExpression;
    public override Span         Span { get; } = span;
}
