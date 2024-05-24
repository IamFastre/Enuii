using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public sealed class UnaryExpression(Token @operator, Expression operand)
    : Expression
{
    public Token      Operator { get; } = @operator;
    public Expression Operand  { get; } = operand;

    public override Span     Span { get; } = new(@operator.Span, operand.Span);
    public override NodeKind Kind { get; } = NodeKind.UnaryExpression;
}
