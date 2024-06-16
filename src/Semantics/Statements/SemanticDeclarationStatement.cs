using Enuii.General.Positioning;
using Enuii.Symbols.Names;

namespace Enuii.Semantics;

public class SemanticDeclarationStatement(NameSymbol symbol, SemanticExpression expr, Span span)
    : SemanticStatement
{
    public NameSymbol         Symbol     { get; } = symbol;
    public SemanticExpression Expression { get; } = expr;

    public override SemanticKind Kind { get; } = SemanticKind.DeclarationStatement;
    public override Span         Span { get; } = span;
}
