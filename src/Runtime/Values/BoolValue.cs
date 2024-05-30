using Enuii.General.Constants;
using Enuii.General.Utilities;
using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public sealed class BoolValue(bool value)
    : RuntimeValue
{
    public override object     Value { get; } = value;
    public override TypeSymbol Type  { get; } = TypeSymbol.Boolean;

    public override string ToString()
        => (bool) Value ? CONSTS.TRUE : CONSTS.FALSE;

    public static BoolValue Parse(string value)
    {
        if (value == CONSTS.TRUE)
            return new(true);

        else if (value == CONSTS.FALSE)
            return new(false);

        else if (value == CONSTS.MAYBE)
            return new(Utils.CoinFlip());

        else
            throw new Exception($"Cannot parse boolean value: {value}");
    }
}
