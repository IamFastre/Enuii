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
                    return new NullValue();
                
                case TypeID.Boolean:
                    return new BoolValue(cl.Value);

                case TypeID.Integer:
                    return new IntValue(cl.Value);

                case TypeID.Float:
                    return new FloatValue(cl.Value);

                case TypeID.Number:
                    return new NumberValue(cl.Value);

                case TypeID.Char:
                    return new CharValue(cl.Value);

                case TypeID.String:
                    return new StringValue(cl.Value);
            }
        }
        catch
        {
            Reporter.ReportInternalParsingError(cl.Value, cl.Type.ToString(), cl.Span);
            return UnknownValue.Template;
        }

        throw new Exception($"Unrecognized semantic constant type '{cl.Type}' while evaluating");
    }
}
