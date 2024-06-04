using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public class RangeValue(NumberValue? start, NumberValue? end, NumberValue? step)
    : RuntimeValue
{
    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.Range;

    public NumberValue? Start { get; } = start;
    public NumberValue? End   { get; } = end;
    public NumberValue  Step  { get; } = step ?? new IntValue((double) (start?.Value ?? 0) > (double) (end?.Value ?? 0) ? -1 : 1);

    public override int GetHashCode()
        => HashCode.Combine(Start, End, Step);

    public override bool Equals(object? obj)
        => obj is RangeValue rv && Start! == rv.Start! && End! == rv.End! && Step == rv.Step;

    public override string ToString()
        => $"|{Start}:{End}:{Step}|";

    public static bool Check(double? start, double? end, double? step)
    {
        start ??= double.NegativeInfinity;
        end   ??= double.PositiveInfinity;
        step  ??= start > end ? -1 : 1;

        return double.IsPositive(step.Value)
             ? end > start
             : end < start;
    }
}
