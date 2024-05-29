using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public class NumberValue(string value)
    : RuntimeValue
{
    public override object     Value { get; } = Parse(value);
    public override TypeSymbol Type  { get; } = TypeSymbol.Number;

    public override string ToString()
        => Value.ToString()!.Replace('E', 'e')
         + (this is FloatValue ? "f" : "");

    public static double Parse(string value, bool floor = false)
    {
        var db = double.Parse(value.Replace("f", "").Replace("F", ""));
        return db == double.NegativeZero ? 0D : floor ? Math.Floor(db) : db;
    }
}


public sealed class IntValue(string value)
    : NumberValue(value)
{
    public override object     Value { get; } = Parse(value, true);
    public override TypeSymbol Type  { get; } = TypeSymbol.Integer;
}


public sealed class FloatValue(string value)
    : NumberValue(value)
{
    public override object     Value { get; } = Parse(value);
    public override TypeSymbol Type  { get; } = TypeSymbol.Float;
}
