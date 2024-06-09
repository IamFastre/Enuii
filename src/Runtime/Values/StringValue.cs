using System.Text.RegularExpressions;
using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public sealed class StringValue(string value)
    : RuntimeValue
{
    public override object     Value { get; } = value;
    public override TypeSymbol Type  { get; } = TypeSymbol.String;

    public override string ToString()
        => (string) Value;

    public static StringValue Parse(string value)
        => new(value[1..^1]);
}
