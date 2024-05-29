using Enuii.General.Constants;
using Enuii.General.Utilities;
using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public sealed class BoolValue(string value)
    : RuntimeValue
{
    public override object     Value { get; } = Parse(value);
    public override TypeSymbol Type  { get; } = TypeSymbol.Boolean;

    public override string ToString()
        => (bool) Value ? CONSTS.TRUE : CONSTS.FALSE;

    public static bool Parse(string value)
    {
        if (value == CONSTS.TRUE)
            return true;

        else if (value == CONSTS.FALSE)
            return false;

        else if (value == CONSTS.MAYBE)
            return Utils.CoinFlip();

        else
            throw new Exception($"Cannot parse boolean value: {value}");
    }
}
