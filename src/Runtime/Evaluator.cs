using System.Collections.Immutable;
using Enuii.Reports;
using Enuii.Runtime.Conversion;
using Enuii.Scoping;
using Enuii.Semantics;
using Enuii.Semantics.Operations;
using Enuii.Symbols.Types;

namespace Enuii.Runtime.Evaluation;

public class Evaluator
{
    public SemanticTree SemanticTree { get; }
    public Reporter     Reporter     { get; }
    public Scope        Scope        { get; private  set; }
    public State        State        { get; internal set; }

    public Evaluator(SemanticTree tree, Scope scope, Reporter? reporter = null)
    {
        SemanticTree = tree;
        Reporter     = reporter ?? new();
        Scope        = scope;
        State        = State.None;

        Reporter.InRuntime = true;
    }

    /* =========================== Helper Methods =========================== */

    private void EnterScope()
        => Scope = new(Scope);

    private void ExitScope()
        => Scope = Scope.Parent!;

    /* ====================================================================== */

    public RuntimeValue Start()
    {
        RuntimeValue value = VoidValue.Template;

        foreach (var stmt in SemanticTree.Body)
            value = EvaluateStatement(stmt);

        return value;
    }


    /* ====================================================================== */
    /*                               Statements                               */
    /* ====================================================================== */

    public RuntimeValue EvaluateStatement(SemanticStatement stmt)
    {
        switch(stmt.Kind)
        {
            case SemanticKind.ExpressionStatement:
                return EvaluateExpressionStatement((SemanticExpressionStatement) stmt);

            case SemanticKind.DeclarationStatement:
                return EvaluateDeclarationStatement((SemanticDeclarationStatement) stmt);

            case SemanticKind.BlockStatement:
                return EvaluateBlockStatement((SemanticBlockStatement) stmt);

            case SemanticKind.IfStatement:
                return EvaluateIfStatement((SemanticIfStatement) stmt);

            case SemanticKind.WhileStatement:
                return EvaluateWhileStatement((SemanticWhileStatement) stmt);

            case SemanticKind.ForStatement:
                return EvaluateForStatement((SemanticForStatement) stmt);

            case SemanticKind.DeleteStatement:
                return EvaluateDeleteStatement((SemanticDeleteStatement) stmt);

            case SemanticKind.FunctionStatement:
                return EvaluateFunctionStatement((SemanticFunctionStatement) stmt);

            default:
                throw new Exception($"Unrecognized semantic statement kind while evaluating: {stmt.Kind}");
        }
    }

    private RuntimeValue EvaluateExpressionStatement(SemanticExpressionStatement es)
        => EvaluateExpression(es.Expression);

    private RuntimeValue EvaluateDeclarationStatement(SemanticDeclarationStatement ds)
    {
        var expr = EvaluateExpression(ds.Expression);

        if (Scope.TryDeclare(ds.Name, expr))
            return expr;

        return UnknownValue.Template;
    }

    private RuntimeValue EvaluateBlockStatement(SemanticBlockStatement bs)
    {
        RuntimeValue value = VoidValue.Template;

        EnterScope();
        foreach (var stmt in bs.Body)
            value = EvaluateStatement(stmt);
        ExitScope();

        return value;
    }

    private RuntimeValue EvaluateIfStatement(SemanticIfStatement fs)
    {
        var condition = EvaluateExpression(fs.Condition);
        var thenStmt  = EvaluateStatement(fs.Then);
        var elseStmt  = fs.Else is not null ? EvaluateStatement(fs.Else) : null;

        return (bool) condition.Value ? thenStmt : elseStmt ?? VoidValue.Template;
    }

    private RuntimeValue EvaluateWhileStatement(SemanticWhileStatement ws)
    {
        RuntimeValue value = VoidValue.Template;
        var conditionVal = EvaluateExpression(ws.Condition);

        if ((bool) conditionVal.Value)
            while ((bool) EvaluateExpression(ws.Condition).Value)
                value = EvaluateStatement(ws.Loop);
        else if (ws.Else is not null)
            value = EvaluateStatement(ws.Else);

        return value;
    }

    private RuntimeValue EvaluateForStatement(SemanticForStatement fs)
    {
        RuntimeValue value = VoidValue.Template;
        var iterable = (IEnumerableValue<RuntimeValue>) EvaluateExpression(fs.Iterable);

        if (iterable.Length > 0)
        {
            EnterScope();
            Scope.TryDeclare(fs.Variable, null!);

            for (int i = 0; i < iterable.Length; i++)
            {
                Scope.TryAssign(fs.Variable, iterable.ElementAt(i));
                value = EvaluateStatement(fs.Loop);
            }

            ExitScope();
        }
        else if (fs.Else is not null)
            value = EvaluateStatement(fs.Else);

        return value;
    }

    private VoidValue EvaluateDeleteStatement(SemanticDeleteStatement ds)
    {
        Scope.Delete(ds.Name);
        return VoidValue.Template;
    }

    private VoidValue EvaluateFunctionStatement(SemanticFunctionStatement ss)
    {
        var function = new FunctionValue(ss.Function.Name, ss.Function.Type, ss.Function.Parameters, ss.Body, Scope);
        Scope.TryDeclare(ss.Function.Name, function);

        return VoidValue.Template;
    }


    /* ====================================================================== */
    /*                               Expressions                              */
    /* ====================================================================== */

    public RuntimeValue EvaluateExpression(SemanticExpression expr)
    {
        switch(expr.Kind)
        {
            case SemanticKind.Constant:
                return EvaluateConstant((SemanticConstantLiteral) expr);

            case SemanticKind.Range:
                return EvaluateRange((SemanticRangeLiteral) expr);

            case SemanticKind.List:
                return EvaluateList((SemanticListLiteral) expr);

            case SemanticKind.Name:
                return EvaluateName((SemanticNameLiteral) expr);

            case SemanticKind.CallExpression:
                return EvaluateCallExpression((SemanticCallExpression) expr);

            case SemanticKind.NullForgivingExpression:
                return EvaluateNullForgivingExpression((SemanticNullForgivingExpression) expr);

            case SemanticKind.ConversionExpression:
                return EvaluateConversionExpression((SemanticConversionExpression) expr);

            case SemanticKind.UnaryExpression:
                return EvaluateUnaryExpression((SemanticUnaryExpression) expr);

            case SemanticKind.BinaryExpression:
                return EvaluateBinaryExpression((SemanticBinaryExpression) expr);

            case SemanticKind.CountingExpression:
                return EvaluateCountingExpression((SemanticCountingExpression) expr);

            case SemanticKind.AssignmentExpression:
                return EvaluateAssignmentExpression((SemanticAssignmentExpression) expr);

            case SemanticKind.FailedExpression:
                return EvaluateFailedExpression();

            case SemanticKind.FailedOperation:
                return EvaluateFailedOperation((SemanticFailedOperation) expr);

            default:
                throw new Exception($"Unrecognized semantic expression kind while evaluating: {expr.Kind} of type {expr.Type}");
        }
    }

    private RuntimeValue EvaluateConstant(SemanticConstantLiteral cl)
    {
        switch(cl.Type.ID)
        {
            case TypeID.Null:
                return NullValue.Template;

            case TypeID.Boolean:
                return BoolValue.Parse(cl.Value);

            case TypeID.Integer:
                return NumberValue.Parse(cl.Value, true);

            case TypeID.Float:
            case TypeID.Number:
                return NumberValue.Parse(cl.Value);

            case TypeID.Char:
                return CharValue.Parse(cl.Value);

            case TypeID.String:
                return StringValue.Parse(cl.Value);

            default:
                throw new Exception($"Unrecognized semantic constant type '{cl.Type}' while evaluating");
        }
    }

    private RuntimeValue EvaluateRange(SemanticRangeLiteral rl)
    {
        var start = (NumberValue?) (rl.Start is not null ? EvaluateExpression(rl.Start) : null);
        var end   = (NumberValue?) (rl.End   is not null ? EvaluateExpression(rl.End)   : null);
        var step  = (NumberValue?) (rl.Step  is not null ? EvaluateExpression(rl.Step)  : null);

        // if step is given as 0 then report an error
        if (step is not null && step.Value is double s && s == 0)
        {
            Reporter.ReportZeroStepRange(rl.Span);
            return UnknownValue.Template;
        }

        // the direction of the range is invalid report an error
        if (!RangeValue.Check((double?) start?.Value, (double?) end?.Value, (double?) step?.Value))
        {
            Reporter.ReportBadRangeDirection(rl.Span);
            return UnknownValue.Template;
        }

        return new RangeValue(start, end, step);
    }

    private ListValue EvaluateList(SemanticListLiteral ll)
    {
        var exprs = ImmutableArray.CreateBuilder<RuntimeValue>();

        foreach (var expr in ll.Expressions)
            exprs.Add(EvaluateExpression(expr));

        return new(exprs, ll.Type);
    }

    private RuntimeValue EvaluateName(SemanticNameLiteral nl)
        => Scope.Get(nl.Name) ?? UnknownValue.Template;

    private RuntimeValue EvaluateCallExpression(SemanticCallExpression ce)
    {
        var callee   = (ICallable) EvaluateExpression(ce.Callee);
        var args     = ce.Arguments.Select(EvaluateExpression).ToArray();
        var oldScope = Scope;

        Scope = new(callee.Scope ?? Scope);
        var value = callee.Call(this, [..args]);
        Scope = oldScope;

        return value;
    }

    private RuntimeValue EvaluateNullForgivingExpression(SemanticNullForgivingExpression nfe)
        => EvaluateExpression(nfe.Expression);

    private RuntimeValue EvaluateConversionExpression(SemanticConversionExpression ce)
    {
        var value  = EvaluateExpression(ce.Expression);

        // if it's a range conversion and the range is infinite
        // handle the error
        if (ce.OperationKind is ConversionKind.RangeToNumberList
                             or ConversionKind.RangeToIntList
                             or ConversionKind.RangeToFloatList
        && (((RangeValue) value).Start is null || ((RangeValue) value).End is null))
        {
            Reporter.ReportInfiniteRange(ce.Expression.Span);
            return UnknownValue.Template;
        }

        var result = Converter.Convert(value, ce.OperationKind, ce.Destination);

        return result;
    }

    private RuntimeValue EvaluateUnaryExpression(SemanticUnaryExpression ue)
    {
        var operand = EvaluateExpression(ue.Operand);

        if (operand.Type.IsNull)
            return NullValue.Template;

        return ue.OperationKind switch
        {
            UnaryKind.Identity          => operand,

            UnaryKind.Negation          => operand is IntValue
                                         ? new IntValue(-(double) operand.Value)
                                         : new FloatValue(-(double) operand.Value),

            UnaryKind.Complement        => new BoolValue(!(bool) operand.Value),

            UnaryKind.BitwiseComplement => new IntValue(-(double) operand.Value - 1),

            _ => throw new Exception($"Unrecognized unary operation kind while evaluating result: {ue.OperationKind}:{ue.Type} on '{ue.Operand.Type}'"),
        };
    }

    private RuntimeValue EvaluateBinaryExpression(SemanticBinaryExpression be)
    {
        var left  = EvaluateExpression(be.LHS);
        var right = EvaluateExpression(be.RHS);

        if (left.Type.IsNull || right.Type.IsNull)
            return NullValue.Template;

        switch(be.OperationKind)
        {
            /* =========================== General ========================== */

            case BinaryKind.Equality:
                return new BoolValue(left == right);

            case BinaryKind.Inequality:
                return new BoolValue(left != right);

            case BinaryKind.NullishCoalescence:
                return left is NullValue ? right : left;

            /* =========================== Boolean ========================== */

            case BinaryKind.LogicalAND:
            case BinaryKind.BitwiseAND when be.Type.ID is TypeID.Boolean:
                return new BoolValue(((bool) left.Value) && ((bool) right.Value));

            case BinaryKind.LogicalOR:
            case BinaryKind.BitwiseOR  when be.Type.ID is TypeID.Boolean:
                return new BoolValue(((bool) left.Value) || ((bool) right.Value));

            case BinaryKind.BitwiseXOR when be.Type.ID is TypeID.Boolean:
                return new BoolValue(((bool) left.Value) ^  ((bool) right.Value));
            
            case BinaryKind.BitwiseAND when be.Type.ID is TypeID.Integer:
                return new IntValue(((int)(double) left.Value) & ((int)(double) right.Value));

            case BinaryKind.BitwiseOR  when be.Type.ID is TypeID.Integer:
                return new IntValue(((int)(double) left.Value) | ((int)(double) right.Value));

            case BinaryKind.BitwiseXOR when be.Type.ID is TypeID.Integer:
                return new IntValue(((int)(double) left.Value) ^ ((int)(double) right.Value));

            case BinaryKind.LeftShift when be.Type.ID is TypeID.Integer:
                return new IntValue(((double) left.Value) * Math.Pow(2D, (double) right.Value));

            case BinaryKind.RightShift when be.Type.ID is TypeID.Integer:
                return new IntValue(((double) left.Value) / Math.Pow(2D, (double) right.Value));

            /* ======================== Mathematical ======================== */

            case BinaryKind.Addition:
                return NumberValue.Get(((double) left.Value) + ((double) right.Value), be.Type.ID);

            case BinaryKind.Subtraction:
                return NumberValue.Get(((double) left.Value) - ((double) right.Value), be.Type.ID);

            case BinaryKind.Multiplication:
                return NumberValue.Get(((double) left.Value) * ((double) right.Value), be.Type.ID);

            case BinaryKind.Division:
                return NumberValue.Get(((double) left.Value) / ((double) right.Value), be.Type.ID);

            case BinaryKind.Power:
                return NumberValue.Get(Math.Pow((double) left.Value, (double) right.Value), be.Type.ID);

            case BinaryKind.Modulo:
                return NumberValue.Get(((double) left.Value) % ((double) right.Value), be.Type.ID);

            /* ========================= Comparative ======================== */

            case BinaryKind.Less:
                return new BoolValue(((double) left.Value) <  ((double) right.Value));

            case BinaryKind.Greater:
                return new BoolValue(((double) left.Value) >  ((double) right.Value));

            case BinaryKind.LessEqual:
                return new BoolValue(((double) left.Value) <= ((double) right.Value));

            case BinaryKind.GreaterEqual:
                return new BoolValue(((double) left.Value) >= ((double) right.Value));

            /* =========================== Stringy ========================== */

            case BinaryKind.CharIncrementing:
                return new CharValue(left.Type.ID is TypeID.Char
                    ? (char)(((char) left.Value)   + ((double) right.Value))
                    : (char)(((double) left.Value) + ((char) right.Value)));

            case BinaryKind.CharDecrementing:
                return new CharValue((char)(((char) left.Value) - ((double) right.Value)));

            case BinaryKind.StringConcatenation:
                return new StringValue(left.ToString() + right.ToString());

            case BinaryKind.StringMultiplication:
                var num = (double) (left.Type.ID is TypeID.Integer ? left.Value : right.Value);
                var str = (string) (left.Type.ID is TypeID.String  ? left.Value : right.Value);

                var result = string.Empty;

                if (num < 0)
                    str = new(str.Reverse().ToArray());

                for (int i = 0; i < Math.Abs(num); i++)
                    result += str;

                return new StringValue(result);

            case BinaryKind.StringInclusion:
                return new BoolValue(((string) right.Value).Contains(left.Value!.ToString()!)); // TODO: Change this piece of shit
                                                                                                // c'mon it's pretty (I might have dissociative identity disorder)
            default:
                throw new Exception($"Unrecognized binary operation kind while evaluating result: {be.OperationKind}:{be.Type} on '{be.LHS.Type}' and '{be.RHS.Type}'");
        };
    }

    private RuntimeValue EvaluateCountingExpression(SemanticCountingExpression ce)
    {
        var operand = EvaluateExpression(ce.Operand);
        var step    = ce.OperationKind is CountingKind.Increment ? 1 : -1;

        var result = ce.Type.ID switch
        {
            TypeID.Char    => new CharValue((char)((char)operand.Value + step)),
            TypeID.Integer => new IntValue((double)operand.Value + step),
            TypeID.Float   => new FloatValue((double)operand.Value + step),
            TypeID.Number  => NumberValue.GetBest((double)operand.Value + step),
            _              => operand,
        };

        Scope.TryAssign(ce.Operand.Name, result);
        return ce.IsBefore ? result : operand;
    }

    private RuntimeValue EvaluateAssignmentExpression(SemanticAssignmentExpression ae)
    {
        var expr = EvaluateExpression(ae.Expression);

        if (Scope.TryAssign(ae.Name, expr))
            return expr;

        return UnknownValue.Template;
    }

    private NullValue EvaluateFailedExpression()
        => NullValue.Template;

    private NullValue EvaluateFailedOperation(SemanticFailedOperation fo)
    {
        foreach (var o in fo.Expressions)
            EvaluateExpression(o);

        return NullValue.Template;
    }
}
