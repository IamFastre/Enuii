using Enuii.Symbols.Typing;
using Enuii.Syntax.Lexing;

namespace Enuii.Semantics;

public enum BinaryKind
{
    INVALID,

    Equality,
    Inequality,

    NullishCoalescence,

    LogicalAND,
    LogicalOR,

    BitwiseAND,
    BitwiseOR,
    BitwiseXOR,

    Addition,
    Subtraction,
    Multiplication,
    Division,
    Power,
    Modulo,

    Greater,
    Less,
    GreaterEqual,
    LessEqual,

    CharIncrementing,
    CharDecrementing,

    StringConcatenation,
    StringMultiplication,
    StringInclusion,
}

public class BinaryOperation
{
    public TokenKind   Operator  { get; }
    public BinaryKind  Kind      { get; }
    public TypeSymbol? Left      { get; private set; }
    public TypeSymbol? Right     { get; private set; }
    public TypeSymbol? Result    { get; private set; }

    // Use this constructor if both operands and the result are of the same type
    private BinaryOperation(TokenKind op, BinaryKind kind, TypeSymbol type)
        : this(op, kind, type, type, type) { }

    // Use this constructor if both operands are the same type but have a different result type
    private BinaryOperation(TokenKind op, BinaryKind kind, TypeSymbol operands, TypeSymbol result)
        : this(op, kind, operands, operands, result) { }

    // Use this constructor if both your parents hate you
    private BinaryOperation(TokenKind op, BinaryKind kind, TypeSymbol left, TypeSymbol right, TypeSymbol result)
    {
        Operator = op;
        Left     = left;
        Right    = right;
        Result   = result;
        Kind     = kind;
    }

    public static (BinaryKind, TypeSymbol) GetOperation(TypeSymbol left, TokenKind opKind, TypeSymbol right)
    {
        foreach (var op in operations)
            if (op.Matches(left, opKind, right))
                return (op.Kind, op.Result ?? TypeSymbol.GetCommonType(left, right) ?? left);

        return (BinaryKind.INVALID, TypeSymbol.Unknown);
    }

    public bool Matches(TypeSymbol left, TokenKind op, TypeSymbol right)
    {
        if (Operator == op)
        {

            if (Left is null || Right is null)
                return TypeSymbol.GetCommonType(left, right) is not null;

            return Left.HasFlag(left) && Right.HasFlag(right);
        }

        return false;
    }

    // Big array of all possible native binary operations
    private static readonly BinaryOperation[] operations =
    [
        /* ============================= General ============================ */

        new(TokenKind.EqualEqual, BinaryKind.Equality, null!, TypeSymbol.Boolean), // <T> == <T> -> bool
        new(TokenKind.NotEqual, BinaryKind.Inequality, null!, TypeSymbol.Boolean), // <T> != <T> -> bool

        new(TokenKind.DoubleQuestionMark, BinaryKind.NullishCoalescence, null!),   // <T?> ?? <T> -> <T>

        /* ============================ Booleany ============================ */

        new(TokenKind.DoubleAmpersand, BinaryKind.LogicalAND, TypeSymbol.Boolean), // bool && bool -> bool
        new(TokenKind.DoublePipe, BinaryKind.LogicalOR, TypeSymbol.Boolean),       // bool || bool -> bool

        new(TokenKind.Ampersand, BinaryKind.BitwiseAND, TypeSymbol.Boolean),       // bool & bool -> bool
        new(TokenKind.Pipe, BinaryKind.BitwiseOR, TypeSymbol.Boolean),             // bool | bool -> bool
        new(TokenKind.Caret, BinaryKind.BitwiseXOR, TypeSymbol.Boolean),           // bool ^ bool -> bool

        new(TokenKind.Ampersand, BinaryKind.BitwiseAND, TypeSymbol.Integer),       // int & int -> int
        new(TokenKind.Pipe, BinaryKind.BitwiseOR, TypeSymbol.Integer),             // int | int -> int
        new(TokenKind.Caret, BinaryKind.BitwiseXOR, TypeSymbol.Integer),           // int ^ int -> int

        /* ============================== Mathy ============================= */

        new(TokenKind.Plus, BinaryKind.Addition, TypeSymbol.Integer),                                // int + int -> int
        new(TokenKind.Plus, BinaryKind.Addition, TypeSymbol.Float),                                  // float + float -> float
        new(TokenKind.Plus, BinaryKind.Addition, TypeSymbol.Number, TypeSymbol.Float),               // number + number -> float

        new(TokenKind.Minus, BinaryKind.Subtraction, TypeSymbol.Integer),                            // int - int -> int
        new(TokenKind.Minus, BinaryKind.Subtraction, TypeSymbol.Float),                              // float - float -> float
        new(TokenKind.Minus, BinaryKind.Subtraction, TypeSymbol.Number, TypeSymbol.Float),           // number - number -> float

        new(TokenKind.Asterisk, BinaryKind.Multiplication, TypeSymbol.Integer),                      // int * int -> int
        new(TokenKind.Asterisk, BinaryKind.Multiplication, TypeSymbol.Float),                        // float * float -> float
        new(TokenKind.Asterisk, BinaryKind.Multiplication, TypeSymbol.Number, TypeSymbol.Float),     // number * number -> float

        new(TokenKind.ForwardSlash, BinaryKind.Division, TypeSymbol.Integer),                        // int / int -> int (floored)
        new(TokenKind.ForwardSlash, BinaryKind.Division, TypeSymbol.Float),                          // float / float -> float
        new(TokenKind.ForwardSlash, BinaryKind.Division, TypeSymbol.Number, TypeSymbol.Float),       // number / number -> float

        new(TokenKind.Power, BinaryKind.Power, TypeSymbol.Integer),                                  // int ** int -> int (floored)
        new(TokenKind.Power, BinaryKind.Power, TypeSymbol.Float),                                    // float ** float -> float
        new(TokenKind.Power, BinaryKind.Power, TypeSymbol.Number, TypeSymbol.Float),                 // number ** number -> float

        new(TokenKind.Percent, BinaryKind.Modulo, TypeSymbol.Integer),                               // int % int -> int
        new(TokenKind.Percent, BinaryKind.Modulo, TypeSymbol.Float),                                 // float % float -> float
        new(TokenKind.Percent, BinaryKind.Modulo, TypeSymbol.Number, TypeSymbol.Float),              // number % number -> float


        /* =========================== Comparative ========================== */

        new(TokenKind.Less, BinaryKind.Less, TypeSymbol.Number, TypeSymbol.Boolean),                 // number < number -> bool
        new(TokenKind.Greater, BinaryKind.Greater, TypeSymbol.Number, TypeSymbol.Boolean),           // number > number -> bool
        new(TokenKind.LessEqual, BinaryKind.LessEqual, TypeSymbol.Number, TypeSymbol.Boolean),       // number <= number -> bool
        new(TokenKind.GreaterEqual, BinaryKind.GreaterEqual, TypeSymbol.Number, TypeSymbol.Boolean), // number <= number -> bool

        new(TokenKind.Less, BinaryKind.Less, TypeSymbol.Char, TypeSymbol.Boolean),                   // char < char -> bool
        new(TokenKind.Greater, BinaryKind.Greater, TypeSymbol.Char, TypeSymbol.Boolean),             // char > char -> bool
        new(TokenKind.LessEqual, BinaryKind.LessEqual, TypeSymbol.Char, TypeSymbol.Boolean),         // char <= char -> bool
        new(TokenKind.GreaterEqual, BinaryKind.GreaterEqual, TypeSymbol.Char, TypeSymbol.Boolean),   // char <= char -> bool

        /* ============================= Stringy ============================ */

        new(TokenKind.Plus, BinaryKind.CharIncrementing, TypeSymbol.Char, TypeSymbol.Integer, TypeSymbol.Char),             // char + int -> char
        new(TokenKind.Plus, BinaryKind.CharIncrementing, TypeSymbol.Integer, TypeSymbol.Char, TypeSymbol.Char),             // int + char -> char

        new(TokenKind.Minus, BinaryKind.CharDecrementing, TypeSymbol.Char, TypeSymbol.Integer, TypeSymbol.Char),            // char - int -> char

        new(TokenKind.Plus, BinaryKind.StringConcatenation, TypeSymbol.String, TypeSymbol.Any, TypeSymbol.String),          // string + any -> string
        new(TokenKind.Plus, BinaryKind.StringConcatenation, TypeSymbol.Any, TypeSymbol.String, TypeSymbol.String),          // any + string -> string

        new(TokenKind.Asterisk, BinaryKind.StringMultiplication, TypeSymbol.Integer, TypeSymbol.String, TypeSymbol.String), // string * int -> string
        new(TokenKind.Asterisk, BinaryKind.StringMultiplication, TypeSymbol.String, TypeSymbol.Integer, TypeSymbol.String), // int * string -> string

        new(TokenKind.In, BinaryKind.StringInclusion, TypeSymbol.Char, TypeSymbol.String, TypeSymbol.Boolean),              // char in string -> bool
        new(TokenKind.In, BinaryKind.StringInclusion, TypeSymbol.String, TypeSymbol.Boolean),                               // string in string -> bool
    ];
}
