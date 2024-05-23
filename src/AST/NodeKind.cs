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
    UnaryExpression,

    // Statements
    ExpressionStatement,
    BinaryExpression,
}
