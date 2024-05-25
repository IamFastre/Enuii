using System.Collections.Immutable;
using Enuii.Reports;
using Enuii.Symbols.Typing;
using Enuii.Syntax.AST;

namespace Enuii.Semantics;

public class Analyzer
{
    public SyntaxTree SimpleTree { get; }
    public Reporter   Reporter   { get; }

    public Analyzer(SyntaxTree tree, Reporter? reporter = null)
    {
        SimpleTree = tree;
        Reporter   = reporter ?? new();
    }

    /* ====================================================================== */

    public SemanticTree Start()
    {
        var statements = ImmutableArray.CreateBuilder<SemanticStatement>();

        foreach (var stmt in SimpleTree.Body)
            statements.Add(BindStatement(stmt));

        return new([..statements]);
    }


    /* ====================================================================== */
    /*                               Statements                               */
    /* ====================================================================== */

    private SemanticStatement BindStatement(Statement stmt)
    {
        switch (stmt.Kind)
        {
            case NodeKind.ExpressionStatement:
                return BindExpressionStatement((ExpressionStatement) stmt);

            default:
                throw new Exception($"Unrecognized statement kind: {stmt.Kind}");
        }
    }

    private SemanticExpressionStatement BindExpressionStatement(ExpressionStatement stmt)
    {
        var expr = BindExpression(stmt.Expression);
        return new(expr);
    }

    /* ====================================================================== */
    /*                               Expressions                              */
    /* ====================================================================== */

    private SemanticExpression BindExpression(Expression expr)
    {
        switch (expr.Kind)
        {
            case NodeKind.Unknown:
            case NodeKind.Null:
            case NodeKind.Boolean:
            case NodeKind.Integer:
            case NodeKind.Float:
            case NodeKind.Char:
            case NodeKind.String:
                return BindLiteral((Literal) expr);

            case NodeKind.UnaryExpression:
                return BindUnaryExpression((UnaryExpression) expr);

            case NodeKind.BinaryExpression:
                return BindBinaryExpression((BinaryExpression) expr);

            default:
                throw new Exception($"Unrecognized expression kind: {expr.Kind}");
        }
    }

    private SemanticLiteral BindLiteral(Literal l)
    {
        var type = TypeSymbol.GetNodeType(l.Kind);
        return new(l.Value, type, l.Span);
    }

    private SemanticExpression BindUnaryExpression(UnaryExpression ue)
    {
        var operand = BindExpression(ue.Operand);
        var (opKind, resultType) = UnaryOperation.GetOperation(ue.Operator.Kind, operand.Type);

        // Successfully found the operation
        if (opKind is not null)
            return new SemanticUnaryExpression(operand, opKind.Value, resultType, ue.Span);

        // Failed to find the operation
        Reporter.ReportInvalidUnaryOperator(ue.Operator.Value, operand.Type.Name, ue.Span);
        return new SemanticFailedOperation(operand);
    }

    private SemanticExpression BindBinaryExpression(BinaryExpression be)
    {
        var left  = BindExpression(be.LHS);
        var right = BindExpression(be.RHS);
        var (opKind, resultType) = BinaryOperation.GetOperation(left.Type, be.Operator.Kind, right.Type);

        // Successfully found the operation
        if (opKind is not null)
            return new SemanticBinaryExpression(left, right, opKind.Value, resultType, be.Span);

        // Failed to find the operation
        Reporter.ReportInvalidBinaryOperator(be.Operator.Value, left.Type.Name, right.Type.Name, be.Span);
        return new SemanticFailedOperation(left, right);
    }
}
