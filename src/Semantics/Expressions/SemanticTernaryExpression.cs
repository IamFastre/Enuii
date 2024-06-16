using Enuii.General.Positioning;
using Enuii.Symbols.Types;

namespace Enuii.Semantics;
public sealed class SemanticTernaryExpression(SemanticExpression condition, SemanticExpression trueExpr, SemanticExpression falseExpr, TypeSymbol result, Span span)
        : SemanticExpression(result)
{
    public SemanticExpression Condition       { get; } = condition;
    public SemanticExpression TrueExpression  { get; } = trueExpr;
    public SemanticExpression FalseExpression { get; } = falseExpr;

    public override SemanticKind Kind { get; } = SemanticKind.TernaryExpression;
    public override Span         Span { get; } = span;
}
