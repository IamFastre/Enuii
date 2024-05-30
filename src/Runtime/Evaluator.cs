using Enuii.Reports;
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
}