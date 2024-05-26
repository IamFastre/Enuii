namespace Enuii.Syntax.AST;

public enum NodeKind
{
    Unknown,

    // Literals
    Null,
    Boolean,
    Integer,
    Float,
    Char,
    String,
    Identifier,

    // Expressions
    ParenthesizedExpression,
    UnaryExpression,
    BinaryExpression,

    // Statements
    ExpressionStatement,
    BlockStatement,
}
