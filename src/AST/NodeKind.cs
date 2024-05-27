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
    ConversionExpression,
    UnaryExpression,
    BinaryExpression,
    TernaryExpression,

    // Statements
    ExpressionStatement,
    BlockStatement,
    IfStatement,
    WhileStatement,
    
    // Clauses
    ElseClause,
    TypeClause,
}
