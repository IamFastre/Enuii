using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public class SeparatedClause<E>(IEnumerable<E> elems, IEnumerable<Token> seps)
    : Clause where E : Node
{
    public E[]     Elements   { get; } = [..elems];
    public Token[] Separators { get; } = [..seps];

    public override NodeKind Kind { get; } = NodeKind.SeparatedClause;
    public override Span     Span { get; } = elems.Any()
                                           ? elems.First().Span.To(elems.Last().Span)
                                           : new();

    public static SeparatedClause<E> Empty
        => new([], []);
}
