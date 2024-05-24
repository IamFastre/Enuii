using Enuii.General.Positioning;
using Enuii.Symbols.Typing;
using Enuii.Syntax.Lexing;

namespace Enuii.Semantics;

public enum BinaryOperationKind
{
    LogicalAND,
    LogicalOR,

    BitwiseAND,
    BitwiseOR,
    BitwiseXOR,

    Addition,
    Subtraction,
    Multiplication,
    Division,
    Power,
    Modulo,

    CharIncrementing,
    CharDecrementing,

    StringConcatenation,
    StringMultiplication,
}

public class SemanticBinaryExpression(SemanticExpression left, SemanticExpression right, BinaryOperationKind kind, TypeSymbol type, Span span)
    : SemanticExpression(type)
{
    public SemanticExpression  LHS           { get; } = left;
    public SemanticExpression  RHS           { get; } = right;
    public BinaryOperationKind OperationKind { get; } = kind;

    public override SemanticKind Kind { get; } = SemanticKind.BinaryExpression;
    public override Span         Span { get; } = span;
}
