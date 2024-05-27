using Enuii.General.Positioning;

namespace Enuii.Semantics;

public sealed class SemanticWhileStatement(SemanticExpression condition, SemanticStatement loopStmt, SemanticStatement? elseStmt, Span span)
    : SemanticStatement
{
    public SemanticExpression Condition { get; } = condition;
    public SemanticStatement  Loop      { get; } = loopStmt;
    public SemanticStatement? Else      { get; } = elseStmt;

    public override SemanticKind Kind { get; } = SemanticKind.WhileStatement;
    public override Span         Span { get; } = span;
}
