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
    Name,

    // Expressions
    ParenthesizedExpression,
    ConversionExpression,
    UnaryExpression,
    BinaryExpression,
    TernaryExpression,
    AssignmentExpression,

    // Clauses
    TypeClause,
    SeparatedClause,
    ElseClause,

    // Statements
    ExpressionStatement,
    DeclarationStatement,
    BlockStatement,
    IfStatement,
    WhileStatement,
}
