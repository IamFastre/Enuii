namespace Enuii.Semantics;

public enum SemanticKind
{
    // Literals
    Constant,
    Range,
    List,
    Name,

    // Expressions
    FailedExpression,
    FailedOperation,
    ConversionExpression,
    UnaryExpression,
    BinaryExpression,
    TernaryExpression,

    // Statements
    ExpressionStatement,
    BlockStatement,
    IfStatement,
    WhileStatement,
}