using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public sealed class CharValue(char value)
    : RuntimeValue
{
    public override object     Value { get; } = (char)(value % (char.MaxValue + 1));
    public override TypeSymbol Type  { get; } = TypeSymbol.Char;

    public override string ToString()
        => Value.ToString()!;

    public static CharValue Parse(string value)
        => new(value[1]);
}
