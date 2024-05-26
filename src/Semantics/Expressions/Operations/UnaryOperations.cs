using Enuii.Symbols.Typing;
using Enuii.Syntax.Lexing;

namespace Enuii.Semantics;

public enum UnaryOperationKind
{
    INVALID,

    Identity,
    Negation,
    Complement,
    BitwiseComplement,
}

public class UnaryOperation
{
    public TokenKind          Operator { get; }
    public TypeSymbol         Operand  { get; private set; }
    public TypeSymbol         Result   { get; private set; }
    public UnaryOperationKind Kind     { get; }

    // Use this constructor if both the operand and the result are of the same type
    private UnaryOperation(TokenKind op, UnaryOperationKind kind, TypeSymbol operand)
        : this(op, kind, operand, operand) { }

    // Use this constructor if both your professional and love lives are null
    private UnaryOperation(TokenKind op, UnaryOperationKind kind, TypeSymbol operand, TypeSymbol result)
    {
        Operator = op;
        Operand  = operand;
        Result   = result;
        Kind     = kind;
    }

    public static (UnaryOperationKind, TypeSymbol) GetOperation(TokenKind opKind, TypeSymbol operand)
    {
        foreach (var op in operations)
            if (op.Matches(opKind, operand))
                return (op.Kind, op.Result);

        return (UnaryOperationKind.INVALID, TypeSymbol.Unknown);
    }

    public bool Matches(TokenKind op, TypeSymbol operand)
    {
        // if any of the types is null then it's generic
        Operand ??= operand;
        Result ??= operand;

        return Operator == op
            && Operand.Matches(operand);
    }

    // Big array of all possible native unary operations
    private static readonly UnaryOperation[] operations =
    [
        new(TokenKind.BangMark, UnaryOperationKind.Complement, TypeSymbol.Boolean),     // !bool -> bool

        new(TokenKind.Tilde, UnaryOperationKind.BitwiseComplement, TypeSymbol.Integer), // ~int -> int

        new(TokenKind.Plus, UnaryOperationKind.Identity, TypeSymbol.Integer),           // +int -> int
        new(TokenKind.Plus, UnaryOperationKind.Identity, TypeSymbol.Float),             // +float -> float

        new(TokenKind.Minus, UnaryOperationKind.Negation, TypeSymbol.Integer),          // -int -> int
        new(TokenKind.Minus, UnaryOperationKind.Negation, TypeSymbol.Float),            // -float -> float
    ];
}
