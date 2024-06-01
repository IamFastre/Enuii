using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public abstract class NumberValue
    : RuntimeValue
{
    public override string ToString()
        => Value.ToString()!.Replace('E', 'e')
         + (this is FloatValue ? "f" : "");

    public static NumberValue Parse(string value, bool floor = false)
    {
        var db = double.Parse(value.Replace("f", "").Replace("F", ""));
        return floor ? new IntValue(db) : new FloatValue(db);
    }

    public static NumberValue Get(double value, TypeID id)
        => id == TypeID.Integer
         ? new IntValue(value)
         : id == TypeID.Float
         ? new FloatValue(value)
         : throw new Exception($"Can only use int for float not: {id}");
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
