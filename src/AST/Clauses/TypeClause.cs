using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class TypeClause(Token type, IEnumerable<TypeClause>? parameters, uint listDim, Span span)
    : Clause
{
    public Token         Type          { get; } = type;
    public TypeClause[]? Parameters    { get; } = parameters?.ToArray();
    public uint          ListDimension { get; } = listDim;

    public override NodeKind Kind { get; } = NodeKind.TypeClause;
    public override Span     Span { get; } = span;
}