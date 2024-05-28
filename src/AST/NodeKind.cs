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
    List,
    Identifier,

    // Expressions
    ParenthesizedExpression,
    ConversionExpression,
    UnaryExpression,
    BinaryExpression,
    TernaryExpression,

    // Clauses
    TypeClause,
    SeparatedClause,
    ElseClause,

    // Statements
    ExpressionStatement,
    BlockStatement,
    IfStatement,
    WhileStatement,
}
