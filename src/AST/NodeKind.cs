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
    Range,
    Identifier,

    // Expressions
    ParenthesizedExpression,
    UnaryExpression,
    BinaryExpression,
    TernaryExpression,

    // Statements
    ExpressionStatement,
    BlockStatement,
    IfStatement,
    
    // Clauses
    ElseClause,
}
