using System.Collections.Immutable;
using Enuii.Reports;
using Enuii.Scoping;
using Enuii.Symbols;
using Enuii.Symbols.Names;
using Enuii.Symbols.Types;
using Enuii.Syntax.AST;

namespace Enuii.Semantics;

public class Analyzer
{
    public SyntaxTree    SimpleTree { get; }
    public SemanticScope Scope      { get; }
    public Reporter      Reporter   { get; }

    public Analyzer(SyntaxTree tree, SemanticScope? scope = null, Reporter? reporter = null)
    {
        SimpleTree = tree;
        Scope      = scope    ?? new();
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

            case NodeKind.DeclarationStatement:
                return BindDeclarationStatement((DeclarationStatement) stmt);

            case NodeKind.BlockStatement:
                return BindBlockStatement((BlockStatement) stmt);

            case NodeKind.IfStatement:
                return BindIfStatement((IfStatement) stmt);

            case NodeKind.WhileStatement:
                return BindWhileStatement((WhileStatement) stmt);

            default:
                throw new Exception($"Unrecognized statement kind while analyzing: {stmt.Kind}");
        }
    }

    private SemanticExpressionStatement BindExpressionStatement(ExpressionStatement es)
    {
        var expr = BindExpression(es.Expression);
        return new(expr);
    }

    private SemanticDeclarationStatement BindDeclarationStatement(DeclarationStatement ds)
    {
        var type = ds.TypeClause is not null ? BindTypeClause(ds.TypeClause) : null; // bind type if given
        var expr = type is null ? BindExpression(ds.Expression) : BindExpression(ds.Expression, type); // if type is given; expect it
        var name = new NameSymbol(ds.Name.Value, type ?? expr.Type, ds.IsConstant);

        if (type is null || (type is not null && type.HasFlag(expr.Type)))
            if (!Scope.TryDeclare(ds.Name.Value, name))
                Reporter.ReportNameAlreadyDeclared(ds.Name.Value, ds.Name.Span);

        return new(name, expr, ds.Span);
    }

    private SemanticBlockStatement BindBlockStatement(BlockStatement bs)
    {
        var body = ImmutableArray.CreateBuilder<SemanticStatement>();

        foreach (var statement in bs.Body)
            body.Add(BindStatement(statement));

        return new([..body], bs.Span);
    }

    private SemanticIfStatement BindIfStatement(IfStatement fs)
    {
        var condition = BindExpression(fs.Condition, TypeSymbol.Boolean);
        var thenStmt  = BindStatement(fs.Then);
        var elseStmt  = fs.ElseClause is not null
                      ? BindStatement(fs.ElseClause.Body)
                      : null;

        return new(condition, thenStmt, elseStmt, fs.Span);
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
    /*                                 Clauses                                */
    /* ====================================================================== */

    private TypeSymbol BindTypeClause(TypeClause tc)
    {
        TypeSymbol? type = null;
        var parameters = tc.Parameters?.Select(BindTypeClause) ?? [];
        var paramCount = parameters.Count();

        foreach (var t in Builtins.USABLE_TYPES)
        {
            if (t.Name == tc.Type.Value)
            {
                if (t.IsGeneric)
                {
                    if (t.Properties.ArgSize == paramCount)
                        t.SetParameters([..parameters]);
                    else
                        Reporter.ReportWrongTypeParametersCount(t.ToString(), (int) t.Properties.ArgSize, paramCount, tc.Span);
                }
                else if (paramCount != 0)
                    Reporter.ReportTypeNotGeneric(t.ToString(), tc.Type.Span);

                type = t;
                break;
            }
        }

        if (type is null)
            Reporter.ReportUnusableType(tc.Type.Value, tc.Span);
        else
            for (int i = 0; i < tc.ListDimension; i++)
                type = TypeSymbol.List.SetParameters(type);

        return type ?? TypeSymbol.Unknown;
    }


    /* ====================================================================== */
    /*                               Expressions                              */
    /* ====================================================================== */

    private SemanticExpression BindExpression(Expression expr, TypeSymbol expected)
    {
        var val = BindExpression(expr);
        if (!expected.HasFlag(val.Type) && !TypeSymbol.Unknown.HasFlag(val.Type))
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

            case NodeKind.List:
                return BindList((ListLiteral) expr);

            case NodeKind.Name:
                return BindName((NameLiteral) expr);

            case NodeKind.ParenthesizedExpression:
                return BindParenthesizedExpression((ParenthesizedExpression) expr);

            case NodeKind.ConversionExpression:
                return BindConversionExpression((ConversionExpression) expr);

            case NodeKind.UnaryExpression:
                return BindUnaryExpression((UnaryExpression) expr);

            case NodeKind.BinaryExpression:
                return BindBinaryExpression((BinaryExpression) expr);

            case NodeKind.TernaryExpression:
                return BindTernaryExpression((TernaryExpression) expr);

            case NodeKind.AssignmentExpression:
                return BindAssignmentExpression((AssignmentExpression) expr);

            case NodeKind.CompoundAssignmentExpression:
                return BindCompoundAssignmentExpression((CompoundAssignmentExpression) expr);

            default:
                throw new Exception($"Unrecognized expression kind while analyzing: {expr.Kind}");
        }
    }

    private SemanticConstantLiteral BindConstant(ConstantLiteral cl)
        => new(cl.Value, TypeSymbol.GetConstantType(cl.Kind), cl.Span);

    private SemanticRangeLiteral BindRange(RangeLiteral rl)
    {
        var start = rl.Start is not null ? BindExpression(rl.Start, TypeSymbol.Number) : null;
        var end   = rl.End   is not null ? BindExpression(rl.End,   TypeSymbol.Number) : null;
        var step  = rl.Step  is not null ? BindExpression(rl.Step,  TypeSymbol.Number) : null;

        return new(start, end, step, rl.Span);
    }

    private SemanticListLiteral BindList(ListLiteral ll)
    {
        TypeSymbol? type  = null;
        var exprs = ImmutableArray.CreateBuilder<SemanticExpression>();

        foreach (var elem in ll.Elements.Elements)
        {
            var expr = BindExpression(elem);

            // if list is not typed yet
            // or the expression has a more fuzzy type
            // give it that type
            if (type is null || expr.Type.HasFlag(type))
                type ??= expr.Type;
            else if (!type.HasFlag(expr.Type) && type.IsKnown && expr.Type.IsKnown)
            {
                if(!TypeSymbol.GetCommonType(type, expr.Type, ref type))
                    Reporter.ReportHeteroList(type.ToString(), expr.Type.ToString(), ll.Span);
            }

            exprs.Add(expr);
        }

        return new(exprs, type ?? TypeSymbol.Any, ll.Span);
    }

    private SemanticExpression BindName(NameLiteral nl)
    {
        if (Scope.TryGet(nl.Value, out var name))
            return new SemanticNameLiteral(name, nl.Span);

        Reporter.ReportNameNotDefined(nl.Value, nl.Span);
        return new SemanticFailedExpression(nl.Span);
    }

    private SemanticExpression BindParenthesizedExpression(ParenthesizedExpression pe)
        => BindExpression(pe.Expression);

    private SemanticExpression BindConversionExpression(ConversionExpression ce)
    {
        var expr   = BindExpression(ce.Expression);
        var dest   = BindTypeClause(ce.Destination);
        var cvKind = ConversionOperation.GetConversionKind(expr.Type, dest);

        // Successfully found the operation
        if (cvKind is not ConversionKind.INVALID)
            return new SemanticConversionExpression(expr, dest, cvKind, ce.Span);

        // Failed to find the operation
        if (expr.Type.IsKnown && dest.IsKnown)
            Reporter.ReportCannotConvert(expr.Type.ToString(), dest.ToString(), ce.Span);

        return new SemanticFailedOperation(expr);
    }

    private SemanticExpression BindUnaryExpression(UnaryExpression ue)
    {
        var operand = BindExpression(ue.Operand);
        var (opKind, resultType) = UnaryOperation.GetOperation(ue.Operator.Kind, operand.Type);

        // Successfully found the operation
        if (opKind is not UnaryKind.INVALID)
            return new SemanticUnaryExpression(operand, opKind, resultType, ue.Span);

        // Failed to find the operation
        if (operand.Type.IsKnown)
            Reporter.ReportInvalidUnaryOperator(ue.Operator.Value, operand.Type.ToString(), ue.Span);

        return new SemanticFailedOperation(operand);
    }

    private SemanticExpression BindBinaryExpression(BinaryExpression be)
    {
        var left  = BindExpression(be.LHS);
        var right = BindExpression(be.RHS);
        var (opKind, resultType) = BinaryOperation.GetOperation(left.Type, be.Operator.Kind, right.Type);

        // Successfully found the operation
        if (opKind is not BinaryKind.INVALID)
            return new SemanticBinaryExpression(left, right, opKind, resultType, be.Span);

        // Failed to find the operation
        if (left.Type.IsKnown && right.Type.IsKnown)
            Reporter.ReportInvalidBinaryOperator(be.Operator.Value, left.Type.ToString(), right.Type.ToString(), be.Span);

        return new SemanticFailedOperation(left, right);
    }

    private SemanticTernaryExpression BindTernaryExpression(TernaryExpression te)
    {
        var condition = BindExpression(te.Condition, TypeSymbol.Boolean);
        var trueExpr  = BindExpression(te.TrueExpression);
        var falseExpr = BindExpression(te.FalseExpression);

        var match = trueExpr.Type.HasFlag(falseExpr.Type);
        if (match)
            Reporter.ReportTernaryTypesDoNotMatch(trueExpr.Type.ToString(), falseExpr.Type.ToString(), te.Span);

        return new(condition, trueExpr, falseExpr, match ? trueExpr.Type : TypeSymbol.Unknown, condition.Span.To(falseExpr.Span));
    }

    private SemanticExpression BindAssignmentExpression(AssignmentExpression ae)
    {
        var expr = BindExpression(ae.Expression);

        if (!Scope.TryGet(ae.Assignee.Value, out var name))
            Reporter.ReportNameNotDefined(ae.Assignee.Value, ae.Assignee.Span);

        else if (!name.Type.HasFlag(expr.Type))
            Reporter.ReportTypesDoNotMatch(name.Type.ToString(), expr.Type.ToString(), ae.Expression.Span);

        return new SemanticAssignmentExpression(name, expr, ae.Span);
    }

    private SemanticExpression BindCompoundAssignmentExpression(CompoundAssignmentExpression cae)
    {
        var bin  = new BinaryExpression(cae.Assignee, cae.Operation, cae.Expression);
        var expr = BindBinaryExpression(bin);

        if (Scope.TryGet(cae.Assignee.Value, out var name) && !name.Type.HasFlag(expr.Type))
            Reporter.ReportTypesDoNotMatch(name.Type.ToString(), expr.Type.ToString(), cae.Expression.Span);

        return new SemanticAssignmentExpression(name, expr, cae.Span);
    }
}
