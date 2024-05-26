namespace Enuii.Semantics;

public enum SemanticKind
{
    // Literals
    Literal,
    Name,

    // Expressions
    FailedExpression,
    FailedOperation,
    UnaryExpression,
    BinaryExpression,

    // Statements
    ExpressionStatement,
    BlockStatement,
}
