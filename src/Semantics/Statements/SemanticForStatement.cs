using Enuii.General.Positioning;
using Enuii.Symbols.Names;

namespace Enuii.Semantics;

public class SemanticForStatement(NameSymbol variable, SemanticExpression iterable, SemanticStatement loop, Span span)
    : SemanticStatement
{
    public NameSymbol         Variable { get; } = variable;
    public SemanticExpression Iterable { get; } = iterable;
    public SemanticStatement  Loop     { get; } = loop;

    public override SemanticKind Kind { get; } = SemanticKind.ForStatement;
    public override Span         Span { get; } = span;
}
