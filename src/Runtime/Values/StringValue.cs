using Enuii.General.Colors;
using Enuii.Symbols.Types;

namespace Enuii.Runtime.Evaluation;

public sealed class StringValue(string value)
    : RuntimeValue, IEnumerableValue<CharValue>
{
    public override object     Value { get; } = value;
    public override TypeSymbol Type  { get; } = TypeSymbol.String;

    public override string ToString()
        => (string) Value;

    public override string Repr()
        => $"{C.GREEN2}\"{ToString()}\"{C.END}";

    public static StringValue Parse(string value)
        => new(value[1..^1]);

    /* ============================ Enumerability =========================== */

    public double Length => ((string) Value).Length;

    public CharValue ElementAt(int index)
        => new(((string) Value)[index]);

    public bool Contains(RuntimeValue value)
        => value is CharValue && ((string) Value).Contains((char) value.Value);
}
