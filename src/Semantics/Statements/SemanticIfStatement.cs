using Enuii.General.Positioning;

namespace Enuii.Semantics;

public sealed class SemanticIfStatement(SemanticExpression condition, SemanticStatement thenStmt, SemanticStatement? elseStmt, Span span)
    : SemanticStatement
{
    public SemanticExpression Condition { get; } = condition;
    public SemanticStatement  Then      { get; } = thenStmt;
    public SemanticStatement? Else      { get; } = elseStmt;

    public override SemanticKind Kind { get; } = SemanticKind.IfStatement;
    public override Span         Span { get; } = span;
}
