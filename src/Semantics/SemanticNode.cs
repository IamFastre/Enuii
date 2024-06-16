using Enuii.General.Positioning;
using Enuii.Symbols.Types;

namespace Enuii.Semantics;

public abstract class SemanticNode
{
    public abstract Span         Span { get; }
    public abstract SemanticKind Kind { get; }
}

public abstract class SemanticStatement : SemanticNode;

public abstract class SemanticExpression(TypeSymbol type)
    : SemanticNode
{
    public TypeSymbol Type { get; private set; } = type;
}
