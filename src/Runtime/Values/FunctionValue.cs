using Enuii.General.Colors;
using Enuii.Scoping;
using Enuii.Semantics;
using Enuii.Symbols;
using Enuii.Symbols.Names;
using Enuii.Symbols.Types;

namespace Enuii.Runtime.Evaluation;

public class FunctionValue(string name, TypeSymbol type, IEnumerable<ParameterSymbol> parameters, SemanticStatement body, Scope scope)
    : RuntimeValue, ICallable
{
    public string            Name        { get; } = name;
    public ParameterSymbol[] Parameters  { get; } = [..parameters];
    public SemanticStatement Body        { get; } = body;
    public Scope             Scope       { get; } = scope;

    public bool IsBuiltin => this is BuiltinFunctionValue;

    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = type;

    public override int  GetHashCode()       => HashCode.Combine(Parameters, Body, Type);
    public override bool Equals(object? obj) => obj is FunctionValue fv && GetHashCode() == fv.GetHashCode();

    public override string ToString()
    {
        var str = $"{Name}(";

        for (int i = 0; i < Parameters.Length; i++)
        {
            var name = Parameters[i].ToString() + (Parameters[i].HasDefaultValue ? "?" : "");
            var type = Parameters[i].Type.ToString();

            str += $"{name}:{type}";
            if (i != Parameters.Length - 1)
                str += ", ";
        }
        str += ")";

        return $"{str} -> {Type.Properties.Parameters[0]}";
    }

    public override string Repr()
        => $"{C.WHITE}{this}{C.END}";

    /* ============================= Callability ============================ */

    public RuntimeValue Call(Evaluator evaluator, RuntimeValue?[] arguments)
    {
        var before = evaluator.State;
        evaluator.State = State.Function;

        if (!IsBuiltin)
            for (int i = 0; i < Parameters.Length; i++)
            {
                var p = Parameters[i];
                var a = arguments.Length > i ? arguments[i] : null;
                var v = a ?? (p.Value is not null ? evaluator.EvaluateExpression(p.Value) : NullValue.Template);

                evaluator.Scope.TryDeclare(p.Name, v);
            }

        var value = IsBuiltin
                  ? Builtins.CallBuiltin(Name, arguments)
                  : evaluator.EvaluateStatement(Body);

        evaluator.State = before;

        return value;
    }
}

public sealed class BuiltinFunctionValue(FunctionSymbol function)
    : FunctionValue(function.Name, function.Type, function.Parameters, null!, null!);
