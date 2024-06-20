namespace Enuii.Semantics;

public enum SemanticKind
{
    // Literals
    Constant,
    Range,
    List,
    Name,

    // Expressions
    CallExpression,
    FailedExpression,
    FailedOperation,
    ConversionExpression,
    UnaryExpression,
    BinaryExpression,
    TernaryExpression,
    CountingExpression,
    AssignmentExpression,

    // Statements
    ExpressionStatement,
    DeclarationStatement,
    BlockStatement,
    IfStatement,
    WhileStatement,
    ForStatement,
    FunctionStatement,
}
