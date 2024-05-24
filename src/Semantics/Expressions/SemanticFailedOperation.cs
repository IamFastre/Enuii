using Enuii.General.Positioning;
using Enuii.Symbols.Typing;

namespace Enuii.Semantics;

public class SemanticFailedOperation(params SemanticExpression[] expressions)
    : SemanticExpression(TypeSymbol.Unknown)
{
    public SemanticExpression[] Expressions { get; } = expressions;

    public override SemanticKind Kind { get; } = SemanticKind.FailedOperation;
    public override Span         Span { get; } = new(expressions.First().Span, expressions.Last().Span);
}
