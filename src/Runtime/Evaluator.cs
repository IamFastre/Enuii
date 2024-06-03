using Enuii.Reports;
using Enuii.Runtime.Conversion;
using Enuii.Semantics;
using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public class Evaluator
{
    public SemanticTree SemanticTree { get; }
    public Reporter     Reporter     { get; }

    public Evaluator(SemanticTree tree, Reporter? reporter = null)
    {
        SemanticTree = tree;
        Reporter     = reporter ?? new();
    }

    /* ====================================================================== */

    public RuntimeValue Start()
    {
        RuntimeValue value = UnknownValue.Template;

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

            default:
                throw new Exception($"Unrecognized semantic statement kind while evaluating: {stmt.Kind}");
        }
    }

    private RuntimeValue EvaluateExpressionStatement(SemanticExpressionStatement stmt)
        => EvaluateExpression(stmt.Expression);


    /* ====================================================================== */
    /*                               Expressions                              */
    /* ====================================================================== */

    private RuntimeValue EvaluateExpression(SemanticExpression expr)
    {
        switch(expr.Kind)
        {
            case SemanticKind.Constant:
                return EvaluateConstant((SemanticConstantLiteral) expr);

            case SemanticKind.Range:
                return EvaluateRange((SemanticRangeLiteral) expr);

            case SemanticKind.ConversionExpression:
                return EvaluateConversionExpression((SemanticConversionExpression) expr);

            case SemanticKind.UnaryExpression:
                return EvaluateUnaryExpression((SemanticUnaryExpression) expr);

            case SemanticKind.BinaryExpression:
                return EvaluateBinaryExpression((SemanticBinaryExpression) expr);

            default:
                throw new Exception($"Unrecognized semantic expression kind while evaluating: {expr.Kind} of type {expr.Type}");
        }
    }

    private RuntimeValue EvaluateConstant(SemanticConstantLiteral cl)
    {
        try
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
            }
        }
        catch
        {
            Reporter.ReportInternalParsingError(cl.Value, cl.Type.ToString(), cl.Span);
            return UnknownValue.Template;
        }

        throw new Exception($"Unrecognized semantic constant type '{cl.Type}' while evaluating");
    }

    private RangeValue EvaluateRange(SemanticRangeLiteral rl)
    {
        var start = (NumberValue?) (rl.Start is not null ? EvaluateExpression(rl.Start) : null);
        var end   = (NumberValue?) (rl.End   is not null ? EvaluateExpression(rl.End)   : null);
        var step  = (NumberValue?) (rl.Step  is not null ? EvaluateExpression(rl.Step)  : null);

        return new(start, end, step);
    }

    private RuntimeValue EvaluateConversionExpression(SemanticConversionExpression ce)
    {
        var value  = EvaluateExpression(ce.Expression);
        var result = Converter.Convert(value, ce.OperationKind);

        return result;
    }

    private RuntimeValue EvaluateUnaryExpression(SemanticUnaryExpression ue)
    {
        var operand = EvaluateExpression(ue.Operand);

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

        switch(be.OperationKind)
        {
            /* =========================== General ========================== */

            case BinaryKind.Equality:
                return new BoolValue(left == right);

            case BinaryKind.Inequality:
                return new BoolValue(left != right);

            case BinaryKind.NullishCoalescence:
                return left.Type.IsNull ? right : left;

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
                return new IntValue(((int) left.Value) & ((int) right.Value));

            case BinaryKind.BitwiseOR  when be.Type.ID is TypeID.Integer:
                return new IntValue(((int) left.Value) | ((int) right.Value));

            case BinaryKind.BitwiseXOR when be.Type.ID is TypeID.Integer:
                return new IntValue(((int) left.Value) ^ ((int) right.Value));

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

            default:
                throw new Exception($"Unrecognized binary operation kind while evaluating result: {be.OperationKind}:{be.Type} on '{be.RHS.Type}' and '{be.RHS.Type}'");
        };
    }
}
