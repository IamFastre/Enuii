using Enuii.Symbols.Typing;
using Enuii.Syntax.Lexing;

namespace Enuii.Semantics;

public enum BinaryOperationKind
{
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

    CharIncrementing,
    CharDecrementing,

    StringConcatenation,
    StringMultiplication,
}

public class BinaryOperation
{
    public TokenKind           Operator { get; }
    public TypeSymbol          Left     { get; }
    public TypeSymbol          Right    { get; }
    public TypeSymbol          Result   { get; }
    public BinaryOperationKind Kind     { get; }

    // Use this constructor if both operands and the result are of the same type
    public BinaryOperation(TokenKind op, BinaryOperationKind kind, TypeSymbol type)
        : this(op, kind, type, type, type) { }

    // Use this constructor if both operands are the same type but have a different result type
    public BinaryOperation(TokenKind op, BinaryOperationKind kind, TypeSymbol operands, TypeSymbol result)
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
            if (op.Left.Matches(left) && op.Operator == opKind && op.Right.Matches(right))
                return (op.Kind, op.Result);

        throw new Exception("Cannot find such binary operation");
    }

    // Big array of all possible native binary operations
    private static readonly BinaryOperation[] operations =
    [
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
        new(TokenKind.Plus, BinaryOperationKind.Addition, TypeSymbol.Number, TypeSymbol.Float),               // int + float -> float (interchangeable)

        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeSymbol.Integer),                            // int - int -> int
        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeSymbol.Float),                              // float - float -> float
        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeSymbol.Number, TypeSymbol.Float),           // int - float -> float (interchangeable)

        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeSymbol.Integer),                      // int * int -> int
        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeSymbol.Float),                        // float * float -> float
        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeSymbol.Number, TypeSymbol.Float),     // int * float -> float (interchangeable)

        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeSymbol.Integer),                        // int / int -> int (floored)
        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeSymbol.Float),                          // float / float -> float
        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeSymbol.Number, TypeSymbol.Float),       // int / float -> float (interchangeable)

        new(TokenKind.Power, BinaryOperationKind.Power, TypeSymbol.Integer),                                  // int ** int -> int (floored)
        new(TokenKind.Power, BinaryOperationKind.Power, TypeSymbol.Float),                                    // float ** float -> float
        new(TokenKind.Power, BinaryOperationKind.Power, TypeSymbol.Number, TypeSymbol.Float),                 // int ** float -> float (interchangeable)

        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeSymbol.Integer),                               // int % int -> int
        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeSymbol.Float),                                 // float % float -> float
        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeSymbol.Number, TypeSymbol.Float),              // int % float -> float (interchangeable)

        /* ============================= Stringy ============================ */

        new(TokenKind.Plus, BinaryOperationKind.CharIncrementing, TypeSymbol.Char, TypeSymbol.Integer, TypeSymbol.Char),             // char + int -> char
        new(TokenKind.Plus, BinaryOperationKind.CharIncrementing, TypeSymbol.Integer, TypeSymbol.Char, TypeSymbol.Char),             // int + char -> char

        new(TokenKind.Minus, BinaryOperationKind.CharDecrementing, TypeSymbol.Char, TypeSymbol.Integer, TypeSymbol.Char),            // char - int -> char

        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, TypeSymbol.String),                                             // string + string -> string

        new(TokenKind.Asterisk, BinaryOperationKind.StringMultiplication, TypeSymbol.Integer, TypeSymbol.String, TypeSymbol.String), // string * int -> string
        new(TokenKind.Asterisk, BinaryOperationKind.StringMultiplication, TypeSymbol.String, TypeSymbol.Integer, TypeSymbol.String), // int * string -> string
    ];
}
