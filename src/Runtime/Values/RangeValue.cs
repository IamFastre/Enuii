using Enuii.Symbols.Types;

namespace Enuii.Runtime.Evaluation;

public class RangeValue(NumberValue? start, NumberValue? end, NumberValue? step)
    : RuntimeValue, IEnumerableValue<NumberValue>
{
    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.Range;

    public NumberValue? Start { get; } = start;
    public NumberValue? End   { get; } = end;
    public NumberValue  Step  { get; } = step ?? new IntValue((double) (start?.Value ?? 0d) > (double) (end?.Value ?? 0d) ? -1d : 1d);

    public override int GetHashCode()
        => HashCode.Combine(Start, End, Step);

    public override bool Equals(object? obj)
        => obj is RangeValue rv
        && Start! == rv.Start!
        && End!   == rv.End!
        && Step   == rv.Step;

    public override string ToString()
        => $"|{Start}:{End}:{Step}|";

    public override string Repr()
        => $"|{Start?.Repr()}:{End?.Repr()}:{Step.Repr()}|";

    public static bool Check(double? start, double? end, double? step)
    {
        start ??= double.NegativeInfinity;
        end   ??= double.PositiveInfinity;
        step  ??= start > end ? -1 : 1;

        return double.IsPositive(step.Value)
             ? end > start
             : end < start;
    }

    /* ============================ Enumerability =========================== */

    public double Length => Start is null || End is null
                          ? double.PositiveInfinity
                          : (int) Math.Floor(((double) End.Value - (double) Start.Value) / (double) Step.Value) + 1;

    public NumberValue ElementAt(int index)
        => double.IsPositive(index)
         ? NumberValue.GetBest( index    * (double) Step.Value + (double) Start!.Value)
         : NumberValue.GetBest((index+1) * (double) Step.Value + (double) End!.Value);


    public bool Contains(RuntimeValue value)
    {
        if (value is not NumberValue)
            return false;

        var start = (double) (Start?.Value ?? double.NegativeInfinity);
        var end   = (double) (End?.Value ?? double.PositiveInfinity);
        var val   = (double) value.Value;

        var bigger  = start > end
                    ? start
                    : end;

        var smaller = start < end
                    ? start
                    : end;

        return (smaller <= val) && (bigger >= val);
    }
}
