using System.Text.RegularExpressions;
using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public sealed class CharValue(string value)
    : RuntimeValue
{
    public override object     Value { get; } = Parse(value);
    public override TypeSymbol Type  { get; } = TypeSymbol.Char;

    public override string ToString()
        => Value.ToString()!;

    public static char Parse(string value)
        => char.Parse(Regex.Unescape(value[1..^1]));
}
