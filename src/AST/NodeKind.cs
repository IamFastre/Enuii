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
    CountingExpression,
    AssignmentExpression,
    CompoundAssignmentExpression,

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
    ForStatement,
}

public static class NodeKindExtension
{
    public static bool IsAssignableTo(this NodeKind kind)
        => kind is NodeKind.Name;
}
