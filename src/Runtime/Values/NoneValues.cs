using Enuii.General.Constants;
using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public sealed class UnknownValue
    : RuntimeValue
{
    public static readonly UnknownValue Template = new();

    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.Unknown;

    public override string ToString()
        => CONSTS.UNKNOWN;
}

public sealed class NullValue
    : RuntimeValue
{
    public static readonly NullValue Template = new();

    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.Null;

    public override string ToString()
        => CONSTS.NULL;
}

public sealed class VoidValue
    : RuntimeValue
{
    public static readonly VoidValue Template = new();

    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.Void;

    public override string ToString()
        => CONSTS.EMPTY;
}
