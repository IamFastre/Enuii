using Enuii.General.Positioning;
using Enuii.Symbols.Typing;

namespace Enuii.Semantics;

public sealed class SemanticFailedOperation(params SemanticExpression[] expressions)
    : SemanticExpression(TypeSymbol.Unknown)
{
    public SemanticExpression[] Expressions { get; } = expressions;

    public override SemanticKind Kind { get; } = SemanticKind.FailedOperation;
    public override Span         Span { get; } = expressions.First().Span.To(expressions.Last().Span);
}
