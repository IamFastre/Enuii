using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public class CountingExpression(Token op, NameLiteral operand, bool isBefore)
    : Expression
{
    public Token       Operator { get; } = op;
    public NameLiteral Operand  { get; } = operand;
    public bool        IsBefore { get; } = isBefore;

    public override NodeKind Kind { get; } = NodeKind.CountingExpression;
    public override Span     Span { get; } = isBefore ? op.Span.To(operand.Span) : operand.Span.To(op.Span);
}
