using System.Collections.Immutable;
using Enuii.Reports;
using Enuii.Scoping;
using Enuii.Symbols;
using Enuii.Symbols.Names;
using Enuii.Symbols.Types;
using Enuii.Syntax.AST;
using Enuii.Semantics.Operations;
using Enuii.General.Constants;
using Enuii.Syntax.Lexing;

namespace Enuii.Semantics;

public class Analyzer
{
    public SyntaxTree    SimpleTree { get; }
    public Reporter      Reporter   { get; }
    public SemanticScope Scope      { get; private set; }

    public Analyzer(SyntaxTree tree, SemanticScope? scope = null, Reporter? reporter = null)
    {
        SimpleTree = tree;
        Scope      = scope    ?? new();
        Reporter   = reporter ?? new();

        Reporter.InRuntime = false;
    }

    /* =========================== Helper Methods =========================== */

    private void EnterScope()
        => Scope = new(Scope);

    private void ExitScope()
        => Scope = Scope.Parent!;

    private bool TryGet(NameLiteral a, out NameSymbol name)
    {
        if (!Scope.TryGet(a.Name, out name))
        {
            Reporter.ReportNameNotDefined(a.Name, a.Span);
            return false;
        }

        return true;
    }

    private void TryDeclare(NameSymbol symbol, Token token)
    {
        if (!Scope.TryDeclare(symbol))
            Reporter.ReportNameAlreadyDeclared(token.Value, token.Span);
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

            case NodeKind.ForStatement:
                return BindForStatement((ForStatement) stmt);

            case NodeKind.FunctionStatement:
                return BindFunctionStatement((FunctionStatement) stmt);

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
        var hintType = ds.TypeClause is not null ? BindTypeClause(ds.TypeClause) : null;
        var expr     = hintType is null ? BindExpression(ds.Expression) : BindExpression(ds.Expression, hintType);
        var name     = new NameSymbol(ds.Name.Value, hintType ?? expr.Type, ds.IsConstant);

        TryDeclare(name, ds.Name);

        return new(name, expr, ds.Span);
    }

    private SemanticBlockStatement BindBlockStatement(BlockStatement bs)
    {
        var body = ImmutableArray.CreateBuilder<SemanticStatement>();

        EnterScope();
        foreach (var statement in bs.Body)
            body.Add(BindStatement(statement));
        ExitScope();

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

    private SemanticForStatement BindForStatement(ForStatement fs)
    {
        var iterable = BindExpression(fs.Iterable);
        var elemType = iterable.Type.Properties.ElementType;

        if (elemType is null)
            if (iterable.Type.IsKnown)
                Reporter.ReportCannotIterate(iterable.Type.ToString(), iterable.Span);

        EnterScope();

        var variable = new NameSymbol(fs.Variable.Value, elemType ?? TypeSymbol.Unknown, true);
        Scope.TryDeclare(variable);
        var loop = BindStatement(fs.Loop);

        ExitScope();

        var elseStmt  = fs.ElseClause is not null
                      ? BindStatement(fs.ElseClause.Body)
                      : null;

        return new(variable, iterable, loop, elseStmt, fs.Span);
    }

    private SemanticFunctionStatement BindFunctionStatement(FunctionStatement ss)
    {
        // TODO: replace with `BindFunction` when implementing `FunctionLiteral`
        var @params  = BindParameters(ss.Parameters.Elements);
        var retType  = ss.ReturnType is not null
                     ? BindTypeClause(ss.ReturnType, true)
                     : TypeSymbol.Void;
        var function = new FunctionSymbol(ss.Function.Value, @params, retType, ss.IsConstant);

        TryDeclare(function, ss.Function);
        EnterScope();

        foreach (var p in @params)
            Scope.TryDeclare(p);

        var body = BindStatement(ss.Body);
        ExitScope();

        return new(function, body, ss.Span);
    }


    /* ====================================================================== */
    /*                                 Clauses                                */
    /* ====================================================================== */

    private TypeSymbol BindTypeClause(TypeClause tc)
        => BindTypeClause(tc, false);

    private TypeSymbol BindTypeClause(TypeClause tc, bool isFunc)
    {
        TypeSymbol? type = null;
        var parameters = tc.Parameters?.Select(BindTypeClause).ToArray() ?? [];
        var paramCount = parameters.Length;

        foreach (var t in isFunc ? Builtins.ALL_TYPES : Builtins.VALUE_TYPES)
        {
            if (t.Name == tc.Type.Value)
            {
                if (paramCount != 0)
                    Reporter.ReportTypeNotGeneric(t.ToString(), tc.Type.Span);

                type = t;
                break;
            }
        }

        if (type is null)
        {
            switch (tc.Type.Value)
            {
                case CONSTS.LIST:
                    if (paramCount == 1)
                        type = TypeSymbol.List(parameters[0]);
                    else
                        Reporter.ReportWrongTypeParametersCount(CONSTS.LIST, 1, paramCount, tc.Span);
                    break;

                case CONSTS.FUNCTION:
                    if (paramCount > 0)
                        type = TypeSymbol.Function(parameters);
                    else
                        Reporter.ReportArgumentlessGenericType(CONSTS.FUNCTION, tc.Span);
                    break;

                default:
                    Reporter.ReportUnusableType(tc.Type.Value, tc.Span);
                    break;
            }
        }
        else
            for (int i = 0; i < tc.ListDimension; i++)
                type = TypeSymbol.List(type);

        return type ?? TypeSymbol.Unknown;
    }

    private ParameterSymbol[] BindParameters(ParameterClause[] parameters)
    {
        var boundParams = ImmutableArray.CreateBuilder<ParameterSymbol>();
        for (int i = 0; i < parameters.Length; i++)
        {
            ParameterClause? param = parameters[i];

            var boundParam = BindParameterClause(param);
            boundParams.Add(boundParam);

            if (boundParams.Count > 1 && boundParams[i-1].HasDefaultValue && !boundParam.HasDefaultValue)
                Reporter.ReportDefaultlessParameter(boundParam.Name, param.Span);
        }

        return [..boundParams];
    }

    private ParameterSymbol BindParameterClause(ParameterClause parameter)
    {
        var type  = BindTypeClause(parameter.Type);
        var value = parameter.Value is not null
                  ? BindExpression(parameter.Value, type)
                  : null;

        return new(parameter.Name.Value, type, value);
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

            case NodeKind.CallExpression:
                return BindCallExpression((CallExpression) expr);

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

            case NodeKind.CountingExpression:
                return BindCountingExpression((CountingExpression) expr);

            case NodeKind.AssignmentExpression:
            case NodeKind.CompoundAssignmentExpression:
                return BindAssignmentExpression((AssignmentExpression) expr);

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
        if (TryGet(nl, out var name))
            return new SemanticNameLiteral(name, nl.Span);

        return new SemanticFailedExpression(nl.Span);
    }

    private SemanticExpression BindCallExpression(CallExpression ce)
    {
        var callee = BindExpression(ce.Callee);
        var args   = ce.Arguments.Elements.Select(BindExpression).ToArray();

        if (callee.Type.IsCallable)
        {
            // TODO: Fix this to work properly with default values
            if (callee.Type.Properties.Parameters.Length - 1 != args.Length)
            {
                Reporter.ReportInvalidArgumentCount(callee.Type.ToString(), callee.Type.Properties.Parameters.Length - 1, args.Length, ce.Span);
                return new SemanticFailedExpression(ce.Span);
            }

            var faulty = false;
            for (int i = 0; i < args.Length; i++)
            {
                var paramType = callee.Type.Properties.Parameters[i+1];
                var argType   = args[i].Type;

                if (!paramType.HasFlag(argType))
                {
                    if (argType.IsKnown)
                        Reporter.ReportTypesDoNotMatch(paramType.ToString(), argType.ToString(), args[i].Span);
                    faulty = true;
                }
            }

            if (faulty)
                return new SemanticFailedExpression(ce.Span);
            return new SemanticCallExpression(callee, callee.Type.Properties.Parameters[0], args, ce.Span);
        }

        if (callee.Type.IsKnown)
            Reporter.ReportNotCallable(callee.Type.ToString(), ce.Callee.Span);
        return new SemanticFailedExpression(ce.Span);
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

    private SemanticExpression BindCountingExpression(CountingExpression ce)
    {
        var operand = BindExpression(ce.Operand);
        var (opKind, canCount) = CountingOperation.GetOperation(ce.Operator.Kind, operand.Type);

        // Successfully found the operation
        if (opKind is not CountingKind.INVALID)
            if (canCount)
                return new SemanticCountingExpression((SemanticNameLiteral) operand, opKind, ce.IsBefore, ce.Span);
            else
                // Mismatched type
                Reporter.ReportInvalidCountingOperator(opKind.ToString(), operand.Type.ToString(), ce.Span);

        return new SemanticFailedOperation(operand);
    }

    private SemanticAssignmentExpression BindAssignmentExpression(AssignmentExpression ae)
    {
        var expr = ae.Kind is NodeKind.CompoundAssignmentExpression
                 ? BindExpression(new BinaryExpression(ae.Assignee, ((CompoundAssignmentExpression) ae).Operation, ae.Expression))
                 : BindExpression(ae.Expression);

        if (TryGet(ae.Assignee, out var name))
        {
            if (name.IsConstant)
                Reporter.ReportCannotAssignToConst(name.Name, ae.Assignee.Span);

            if (!name.Type.HasFlag(expr.Type))
                Reporter.ReportTypesDoNotMatch(name.Type.ToString(), expr.Type.ToString(), ae.Expression.Span);
        }

        return new(name, expr, ae.Span);
    }
}
