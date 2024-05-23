namespace Enuii.Semantics;

public enum SemanticKind
{
    // Literals
    Literal,
    Name,

    // Expressions
    FailedExpression,
    FailedExpressions,
    UnaryExpression,
    BinaryExpression,

    // Statements
    ExpressionStatement,
}
