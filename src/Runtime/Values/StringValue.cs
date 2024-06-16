using Enuii.Symbols.Types;

namespace Enuii.Runtime.Evaluation;

public sealed class StringValue(string value)
    : RuntimeValue, IEnumerableValue<CharValue>
{
    public override object     Value { get; } = value;
    public override TypeSymbol Type  { get; } = TypeSymbol.String;

    public override string ToString()
        => (string) Value;

    public static StringValue Parse(string value)
        => new(value[1..^1]);

    /* ============================ Enumerability =========================== */

    public double Length => ((string) Value).Length;

    public CharValue ElementAt(int index)
        => new(((string) Value)[index]);

    public bool Contains(CharValue value)
        => ((string) Value).Contains((char) value.Value);
}
