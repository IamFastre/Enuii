using Enuii.Symbols.Types;
using Enuii.General.Positioning;

namespace Enuii.Semantics;

public class SemanticCallExpression(SemanticExpression callee, TypeSymbol returnType, IEnumerable<SemanticExpression> args, Span span)
    : SemanticExpression(returnType)
{
    public SemanticExpression   Callee    { get; } = callee;
    public SemanticExpression[] Arguments { get; } = [..args];

    public override SemanticKind Kind { get; } = SemanticKind.CallExpression;
    public override Span         Span { get; } = span;
}
