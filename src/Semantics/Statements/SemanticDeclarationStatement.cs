using Enuii.General.Positioning;

namespace Enuii.Semantics;

public class SemanticDeclarationStatement(string name, SemanticExpression expr, Span span)
    : SemanticStatement
{
    public string             Name       { get; } = name;
    public SemanticExpression Expression { get; } = expr;

    public override SemanticKind Kind { get; } = SemanticKind.DeclarationStatement;
    public override Span         Span { get; } = span;
}
