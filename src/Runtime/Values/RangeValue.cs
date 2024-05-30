using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public sealed class RangeValue(NumberValue? start, NumberValue? end, NumberValue? step)
    : RuntimeValue
{
    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.Range;

    public NumberValue? Start { get; } = start;
    public NumberValue? End   { get; } = end;
    public NumberValue  Step  { get; } = step ?? new NumberValue("1");

    public override string ToString()
        => $"|{Start}:{End}:{Step}|";
}
