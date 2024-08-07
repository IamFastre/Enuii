using Enuii.General.Colors;
using Enuii.Symbols.Types;

namespace Enuii.Runtime.Evaluation;

public abstract class NumberValue
    : RuntimeValue
{
    public override int GetHashCode()
        => HashCode.Combine(Value, Type.ID);

    public override bool Equals(object? obj)
        => obj is NumberValue nv && Equals(Value, nv.Value);

    public override string ToString()
        => Value.ToString()!.Replace('E', 'e');

    public override string Repr()
        => $"{C.CYAN}{ToString()}{(this is FloatValue ? "f" : "")}{C.END}";

    public static NumberValue Parse(string value, bool parseInt = false)
    {
        var db = double.Parse(value.Replace("f", "").Replace("F", ""));
        return parseInt
             ? new IntValue(db)
             : new FloatValue(db);
    }

    public static NumberValue Get(double value, TypeID id)
        => id == TypeID.Integer
         ? new IntValue(value)
         : id == TypeID.Float
         ? new FloatValue(value)
         : throw new Exception($"Can only use int or float not: {id}");

    public static NumberValue GetBest(double value)
        => double.IsInteger(value)
         ? new IntValue(value)
         : new FloatValue(value);

    public FloatValue ToFloat()
        => new((double) Value);

    public IntValue ToInt()
        => new((double) Value);

    public NumberValue ToBest()
        => GetBest((double) Value);
}


public sealed class IntValue(double value)
    : NumberValue
{
    public override object     Value { get; } = Math.Floor(value == double.NegativeZero ? 0d : value);
    public override TypeSymbol Type  { get; } = TypeSymbol.Integer;
}


public sealed class FloatValue(double value)
    : NumberValue
{
    public override object     Value { get; } = value == double.NegativeZero ? 0d : value;
    public override TypeSymbol Type  { get; } = TypeSymbol.Float;
}
