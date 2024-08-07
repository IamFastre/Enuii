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
    CallExpression,
    ParenthesizedExpression,
    NullForgivingExpression,
    ConversionExpression,
    UnaryExpression,
    BinaryExpression,
    TernaryExpression,
    CountingExpression,
    AssignmentExpression,
    CompoundAssignmentExpression,

    // Clauses
    TypeClause,
    ElseClause,
    SeparatedClause,
    ParameterClause,
    ClassBodyClause,

    // Statements
    ExpressionStatement,
    DeclarationStatement,
    BlockStatement,
    IfStatement,
    WhileStatement,
    ForStatement,
    DeleteStatement,
    FunctionStatement,
    ClassStatement,
}

public static class NodeKindExtension
{
    public static bool IsAssignableTo(this NodeKind kind)
        => kind is NodeKind.Name;
}
