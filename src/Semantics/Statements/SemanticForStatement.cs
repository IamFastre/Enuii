using Enuii.General.Positioning;

namespace Enuii.Semantics;

public class SemanticForStatement(string variable, SemanticExpression iterable, SemanticStatement loop, SemanticStatement? elseStmt, Span span)
    : SemanticStatement
{
    public string             Variable { get; } = variable;
    public SemanticExpression Iterable { get; } = iterable;
    public SemanticStatement  Loop     { get; } = loop;
    public SemanticStatement? Else     { get; } = elseStmt;

    public override SemanticKind Kind { get; } = SemanticKind.ForStatement;
    public override Span         Span { get; } = span;
}
