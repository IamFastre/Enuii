using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public class CompoundAssignmentExpression(NameLiteral assignee, Token op, Expression expr)
    : AssignmentExpression(assignee, expr)
{
    public Token Operation { get; } = new(op.Value, GetOperationKind(op.Kind), op.Span);

    public override NodeKind Kind { get; } = NodeKind.CompoundAssignmentExpression;

    private static TokenKind GetOperationKind(TokenKind kind) => kind switch
    {
        TokenKind.PlusEqual               => TokenKind.Plus,
        TokenKind.MinusEqual              => TokenKind.Minus,
        TokenKind.AsteriskEqual           => TokenKind.Asterisk,
        TokenKind.ForwardSlashEqual       => TokenKind.ForwardSlash,
        TokenKind.PercentEqual            => TokenKind.Percent,
        TokenKind.AmpersandEqual          => TokenKind.Ampersand,
        TokenKind.PipeEqual               => TokenKind.Pipe,
        TokenKind.CaretEqual              => TokenKind.Caret,
        TokenKind.PowerEqual              => TokenKind.Power,
        TokenKind.DoubleAmpersandEqual    => TokenKind.DoubleAmpersand,
        TokenKind.DoublePipEqual          => TokenKind.DoublePipe,
        TokenKind.DoubleQuestionMarkEqual => TokenKind.DoubleQuestionMark,

        _ => throw new Exception($"if this happens I'mma just kms: {kind}"),
    };
}
