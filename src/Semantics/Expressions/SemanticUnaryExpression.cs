using Enuii.General.Positioning;
using Enuii.Symbols.Typing;
using Enuii.Syntax.Lexing;

namespace Enuii.Semantics;

public enum UnaryOperationKind
{
    Identity,
    Negation,
    Complement,
    BitwiseComplement,
}

public sealed class SemanticUnaryExpression(SemanticExpression operand, UnaryOperationKind kind, Span span)
    : SemanticExpression(operand.Type)
{
    public SemanticExpression Operand       { get; } = operand;
    public UnaryOperationKind OperationKind { get; } = kind;

    public override Span         Span { get; } = span;
    public override SemanticKind Kind { get; } = SemanticKind.UnaryExpression;

    public static UnaryOperationKind? GetOperationKind(TokenKind kind, TypeID operand)
    {
        if (operand is TypeID.Integer || operand is TypeID.Float)
        {
            if (kind is TokenKind.Plus)
                return UnaryOperationKind.Identity;
            if (kind is TokenKind.Minus)
                return UnaryOperationKind.Negation;
        }

        if (operand is TypeID.Integer)
            if (kind is TokenKind.Tilde)
                return UnaryOperationKind.BitwiseComplement;

        if (operand is TypeID.Boolean)
            if (kind is TokenKind.BangMark)
                return UnaryOperationKind.Complement;

        return null;
    }
}
