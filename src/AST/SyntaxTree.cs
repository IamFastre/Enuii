using Enuii.General.Positioning;

namespace Enuii.Syntax.AST;

public class SyntaxTree(IEnumerable<Statement> body)
{
    public Statement[] Body { get; } = [..body];
    public Span        Span { get; } = body.Any()
                                     ? body.First().Span.To(body.Last().Span)
                                     : new();
}
