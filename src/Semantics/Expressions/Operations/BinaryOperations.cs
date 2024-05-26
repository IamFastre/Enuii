using Enuii.Symbols.Typing;
using Enuii.Syntax.Lexing;

namespace Enuii.Semantics;

public enum BinaryOperationKind
{
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
    public TokenKind           Operator { get; }
    public TypeSymbol          Left     { get; private set; }
    public TypeSymbol          Right    { get; private set; }
    public TypeSymbol          Result   { get; private set; }
    public BinaryOperationKind Kind     { get; }

    // Use this constructor if both operands and the result are of the same type
    private BinaryOperation(TokenKind op, BinaryOperationKind kind, TypeSymbol type)
        : this(op, kind, type, type, type) { }

    // Use this constructor if both operands are the same type but have a different result type
    private BinaryOperation(TokenKind op, BinaryOperationKind kind, TypeSymbol operands, TypeSymbol result)
        : this(op, kind, operands, operands, result) { }

    // Use this constructor if both your parents hate you
    private BinaryOperation(TokenKind op, BinaryOperationKind kind, TypeSymbol left, TypeSymbol right, TypeSymbol result)
    {
        Operator = op;
        Left     = left;
        Right    = right;
        Result   = result;
        Kind     = kind;
    }

    public static (BinaryOperationKind?, TypeSymbol) GetOperation(TypeSymbol left, TokenKind opKind, TypeSymbol right)
    {
        foreach (var op in operations)
            if (op.Matches(left, opKind, right))
                return (op.Kind, op.Result);

        return (null, TypeSymbol.Unknown);
    }

    public bool Matches(TypeSymbol left, TokenKind op, TypeSymbol right)
    {
        Left ??= left;
        Right ??= right;
        Result ??= left;

        return Operator == op
            && Left.Matches(left)
            && Right.Matches(right);
    }

    // Big array of all possible native binary operations
    private static readonly BinaryOperation[] operations =
    [
        /* ============================= General ============================ */

        new(TokenKind.EqualEqual, BinaryOperationKind.Equality, null!, TypeSymbol.Boolean), // <T?> == <T> -> bool
        new(TokenKind.NotEqual, BinaryOperationKind.Inequality, null!, TypeSymbol.Boolean), // <T?> != <T> -> bool

        new(TokenKind.DoubleQuestionMark, BinaryOperationKind.NullishCoalescence, null!),   // <T?> ?? <T> -> <T>

        /* ============================ Booleany ============================ */

        new(TokenKind.DoubleAmpersand, BinaryOperationKind.LogicalAND, TypeSymbol.Boolean), // bool && bool -> bool
        new(TokenKind.DoublePipe, BinaryOperationKind.LogicalOR, TypeSymbol.Boolean),       // bool || bool -> bool

        new(TokenKind.Ampersand, BinaryOperationKind.BitwiseAND, TypeSymbol.Boolean),       // bool & bool -> bool
        new(TokenKind.Pipe, BinaryOperationKind.BitwiseOR, TypeSymbol.Boolean),             // bool | bool -> bool
        new(TokenKind.Caret, BinaryOperationKind.BitwiseXOR, TypeSymbol.Boolean),           // bool ^ bool -> bool

        new(TokenKind.Ampersand, BinaryOperationKind.BitwiseAND, TypeSymbol.Integer),       // int & int -> int
        new(TokenKind.Pipe, BinaryOperationKind.BitwiseOR, TypeSymbol.Integer),             // int | int -> int
        new(TokenKind.Caret, BinaryOperationKind.BitwiseXOR, TypeSymbol.Integer),           // int ^ int -> int

        /* ============================== Mathy ============================= */

        new(TokenKind.Plus, BinaryOperationKind.Addition, TypeSymbol.Integer),                                // int + int -> int
        new(TokenKind.Plus, BinaryOperationKind.Addition, TypeSymbol.Float),                                  // float + float -> float
        new(TokenKind.Plus, BinaryOperationKind.Addition, TypeSymbol.Number, TypeSymbol.Float),               // number + number -> float

        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeSymbol.Integer),                            // int - int -> int
        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeSymbol.Float),                              // float - float -> float
        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeSymbol.Number, TypeSymbol.Float),           // number - number -> float

        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeSymbol.Integer),                      // int * int -> int
        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeSymbol.Float),                        // float * float -> float
        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeSymbol.Number, TypeSymbol.Float),     // number * number -> float

        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeSymbol.Integer),                        // int / int -> int (floored)
        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeSymbol.Float),                          // float / float -> float
        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeSymbol.Number, TypeSymbol.Float),       // number / number -> float

        new(TokenKind.Power, BinaryOperationKind.Power, TypeSymbol.Integer),                                  // int ** int -> int (floored)
        new(TokenKind.Power, BinaryOperationKind.Power, TypeSymbol.Float),                                    // float ** float -> float
        new(TokenKind.Power, BinaryOperationKind.Power, TypeSymbol.Number, TypeSymbol.Float),                 // number ** number -> float

        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeSymbol.Integer),                               // int % int -> int
        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeSymbol.Float),                                 // float % float -> float
        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeSymbol.Number, TypeSymbol.Float),              // number % number -> float


        /* =========================== Comparative ========================== */

        new(TokenKind.Less, BinaryOperationKind.Less, TypeSymbol.Number, TypeSymbol.Boolean),                 // number < number -> bool
        new(TokenKind.Greater, BinaryOperationKind.Greater, TypeSymbol.Number, TypeSymbol.Boolean),           // number > number -> bool
        new(TokenKind.LessEqual, BinaryOperationKind.LessEqual, TypeSymbol.Number, TypeSymbol.Boolean),       // number <= number -> bool
        new(TokenKind.GreaterEqual, BinaryOperationKind.GreaterEqual, TypeSymbol.Number, TypeSymbol.Boolean), // number <= number -> bool

        new(TokenKind.Less, BinaryOperationKind.Less, TypeSymbol.Char, TypeSymbol.Boolean),                   // char < char -> bool
        new(TokenKind.Greater, BinaryOperationKind.Greater, TypeSymbol.Char, TypeSymbol.Boolean),             // char > char -> bool
        new(TokenKind.LessEqual, BinaryOperationKind.LessEqual, TypeSymbol.Char, TypeSymbol.Boolean),         // char <= char -> bool
        new(TokenKind.GreaterEqual, BinaryOperationKind.GreaterEqual, TypeSymbol.Char, TypeSymbol.Boolean),   // char <= char -> bool

        /* ============================= Stringy ============================ */

        new(TokenKind.Plus, BinaryOperationKind.CharIncrementing, TypeSymbol.Char, TypeSymbol.Integer, TypeSymbol.Char),             // char + int -> char
        new(TokenKind.Plus, BinaryOperationKind.CharIncrementing, TypeSymbol.Integer, TypeSymbol.Char, TypeSymbol.Char),             // int + char -> char

        new(TokenKind.Minus, BinaryOperationKind.CharDecrementing, TypeSymbol.Char, TypeSymbol.Integer, TypeSymbol.Char),            // char - int -> char

        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, TypeSymbol.String),                                             // string + string -> string

        new(TokenKind.Asterisk, BinaryOperationKind.StringMultiplication, TypeSymbol.Integer, TypeSymbol.String, TypeSymbol.String), // string * int -> string
        new(TokenKind.Asterisk, BinaryOperationKind.StringMultiplication, TypeSymbol.String, TypeSymbol.Integer, TypeSymbol.String), // int * string -> string

        new(TokenKind.In, BinaryOperationKind.StringInclusion, TypeSymbol.Char, TypeSymbol.String, TypeSymbol.Boolean), // char in string -> bool
        new(TokenKind.In, BinaryOperationKind.StringInclusion, TypeSymbol.String, TypeSymbol.Boolean),                  // string in string -> bool
    ];
}
