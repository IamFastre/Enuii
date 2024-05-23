using Enuii.General.Positioning;

namespace Enuii.Syntax.AST;

public abstract class Node
{
    public abstract Span     Span { get; }
    public abstract NodeKind Kind { get; }
}

public abstract class Statement  : Node;
public abstract class Expression : Node;
