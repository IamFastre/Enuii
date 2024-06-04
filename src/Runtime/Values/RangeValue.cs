using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public sealed class RangeValue(NumberValue? start, NumberValue? end, NumberValue? step)
    : RuntimeValue
{
    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.Range;

    public NumberValue? Start { get; } = start;
    public NumberValue? End   { get; } = end;
    public NumberValue  Step  { get; } = step ?? new IntValue(1);

    public override int GetHashCode()
        => HashCode.Combine(Start, End, Step);

    public override bool Equals(object? obj)
        => obj is RangeValue rv && Start! == rv.Start! && End! == rv.End! && Step == rv.Step;

    public override string ToString()
        => $"|{Start}:{End}:{Step}|";
}
