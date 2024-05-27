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

            case NodeKind.BlockStatement:
                return BindBlockStatement((BlockStatement) stmt);

            case NodeKind.IfStatement:
                return BindIfStatement((IfStatement) stmt);

            case NodeKind.WhileStatement:
                return BindWhileStatement((WhileStatement) stmt);

            default:
                throw new Exception($"Unrecognized statement kind: {stmt.Kind}");
        }
    }

    private SemanticExpressionStatement BindExpressionStatement(ExpressionStatement stmt)
    {
        var expr = BindExpression(stmt.Expression);
        return new(expr);
    }

    private SemanticBlockStatement BindBlockStatement(BlockStatement bs)
    {
        var body = ImmutableArray.CreateBuilder<SemanticStatement>();

        foreach (var statement in bs.Body)
            body.Add(BindStatement(statement));

        return new([..body], bs.Span);
    }

    private SemanticIfStatement BindIfStatement(IfStatement @is)
    {
        var condition = BindExpression(@is.Condition, TypeSymbol.Boolean);
        var thenStmt  = BindStatement(@is.Then);
        var elseStmt  = @is.ElseClause is not null
                      ? BindStatement(@is.ElseClause.Body)
                      : null;

        return new(condition, thenStmt, elseStmt, @is.Span);
    }

    private SemanticWhileStatement BindWhileStatement(WhileStatement ws)
    {
        var condition = BindExpression(ws.Condition, TypeSymbol.Boolean);
        var loopStmt  = BindStatement(ws.Loop);
        var elseStmt  = ws.ElseClause is not null
                      ? BindStatement(ws.ElseClause.Body)
                      : null;

        return new(condition, loopStmt, elseStmt, ws.Span);
    }

    /* ====================================================================== */
    /*                               Expressions                              */
    /* ====================================================================== */

    private SemanticExpression BindExpression(Expression expr, TypeSymbol expected)
    {
        var val = BindExpression(expr);
        if (!expected.Matches(val.Type) && !TypeSymbol.Unknown.Matches(val.Type))
            Reporter.ReportUnexpectedType(expected.ToString(), val.Type.ToString(), expr.Span);

        return val;
    }

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
                return BindConstant((ConstantLiteral) expr);

            case NodeKind.Range:
                return BindRange((RangeLiteral) expr);

            case NodeKind.ParenthesizedExpression:
                return BindParenthesizedExpression((ParenthesizedExpression) expr);

            case NodeKind.UnaryExpression:
                return BindUnaryExpression((UnaryExpression) expr);

            case NodeKind.BinaryExpression:
                return BindBinaryExpression((BinaryExpression) expr);

            case NodeKind.TernaryExpression:
                return BindTernaryExpression((TernaryExpression) expr);

            default:
                throw new Exception($"Unrecognized expression kind: {expr.Kind}");
        }
    }

    private SemanticConstantLiteral BindConstant(ConstantLiteral cl)
    {
        var type = TypeSymbol.GetNodeType(cl.Kind);
        return new(cl.Value, type, cl.Span);
    }

    private SemanticRangeLiteral BindRange(RangeLiteral rl)
    {
        var start = rl.Start is not null ? BindExpression(rl.Start, TypeSymbol.Number) : null;
        var end   = rl.End   is not null ? BindExpression(rl.End,   TypeSymbol.Number) : null;
        var step  = rl.Step  is not null ? BindExpression(rl.Step,  TypeSymbol.Number) : null;

        return new(start, end, step, rl.Span);
    }

    private SemanticExpression BindParenthesizedExpression(ParenthesizedExpression pe)
        => BindExpression(pe.Expression);

    private SemanticExpression BindUnaryExpression(UnaryExpression ue)
    {
        var operand = BindExpression(ue.Operand);
        var (opKind, resultType) = UnaryOperation.GetOperation(ue.Operator.Kind, operand.Type);

        // Successfully found the operation
        if (opKind is not UnaryOperationKind.INVALID)
            return new SemanticUnaryExpression(operand, opKind, resultType, ue.Span);

        // Failed to find the operation
        Reporter.ReportInvalidUnaryOperator(ue.Operator.Value, operand.Type.ToString(), ue.Span);
        return new SemanticFailedOperation(operand);
    }

    private SemanticExpression BindBinaryExpression(BinaryExpression be)
    {
        var left  = BindExpression(be.LHS);
        var right = BindExpression(be.RHS);
        var (opKind, resultType) = BinaryOperation.GetOperation(left.Type, be.Operator.Kind, right.Type);

        // Successfully found the operation
        if (opKind is not BinaryOperationKind.INVALID)
            return new SemanticBinaryExpression(left, right, opKind, resultType, be.Span);

        // Failed to find the operation
        Reporter.ReportInvalidBinaryOperator(be.Operator.Value, left.Type.ToString(), right.Type.ToString(), be.Span);
        return new SemanticFailedOperation(left, right);
    }

    private SemanticTernaryExpression BindTernaryExpression(TernaryExpression te)
    {
        var condition = BindExpression(te.Condition, TypeSymbol.Boolean);
        var trueExpr  = BindExpression(te.TrueExpression);
        var falseExpr = BindExpression(te.FalseExpression);

        var match = trueExpr.Type.Matches(falseExpr.Type);
        if (match)
            Reporter.ReportTernaryTypesDoNotMatch(trueExpr.Type.ToString(), falseExpr.Type.ToString(), te.Span);

        return new(condition, trueExpr, falseExpr, match ? trueExpr.Type : TypeSymbol.Unknown, condition.Span.To(falseExpr.Span));
    }
}
