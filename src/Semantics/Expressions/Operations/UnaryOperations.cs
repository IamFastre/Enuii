using Enuii.Symbols.Types;
using Enuii.Syntax.Lexing;

namespace Enuii.Semantics.Operations;

public enum UnaryKind
{
    INVALID,

    Identity,
    Negation,
    Complement,
    BitwiseComplement,
}

public class UnaryOperation
{
    public TokenKind  Operator { get; }
    public UnaryKind  Kind     { get; }
    public TypeSymbol Operand  { get; private set; }
    public TypeSymbol Result   { get; private set; }

    // Use this constructor if both the operand and the result are of the same type
    private UnaryOperation(TokenKind op, UnaryKind kind, TypeSymbol operand)
        : this(op, kind, operand, operand) { }

    // Use this constructor if both your professional and love lives are null
    private UnaryOperation(TokenKind op, UnaryKind kind, TypeSymbol operand, TypeSymbol result)
    {
        Operator = op;
        Operand  = operand;
        Result   = result;
        Kind     = kind;
    }

    public static (UnaryKind, TypeSymbol) GetOperation(TokenKind opKind, TypeSymbol operand)
    {
        foreach (var op in operations)
            if (op.Matches(opKind, operand.NullSafe()))
                return (op.Kind, op.Result);

        return (UnaryKind.INVALID, TypeSymbol.Unknown);
    }

    public bool Matches(TokenKind op, TypeSymbol operand)
    {
        // if any of the types is null then it's generic
        Operand ??= operand;
        Result ??= operand;

        return Operator == op
            && Operand.HasFlag(operand);
    }

    // Big array of all possible native unary operations
    private static readonly UnaryOperation[] operations =
    [
        new(TokenKind.BangMark, UnaryKind.Complement, TypeSymbol.Boolean),     // !bool -> bool

        new(TokenKind.Tilde, UnaryKind.BitwiseComplement, TypeSymbol.Integer), // ~int -> int

        new(TokenKind.Plus, UnaryKind.Identity, TypeSymbol.Integer),           // +int -> int
        new(TokenKind.Plus, UnaryKind.Identity, TypeSymbol.Float),             // +float -> float

        new(TokenKind.Minus, UnaryKind.Negation, TypeSymbol.Integer),          // -int -> int
        new(TokenKind.Minus, UnaryKind.Negation, TypeSymbol.Float),            // -float -> float
    ];
}
