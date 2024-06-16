using Enuii.General.Colors;
using Enuii.General.Constants;
using Enuii.Symbols.Types;

namespace Enuii.Runtime.Evaluation;

public sealed class UnknownValue
    : RuntimeValue
{
    public static readonly UnknownValue Template = new();

    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.Unknown;

    public override string ToString()
        => CONSTS.UNKNOWN;

    public override string Repr()
        => C.RED + ToString() + C.END;
}

public sealed class NullValue
    : RuntimeValue
{
    public static readonly NullValue Template = new();

    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.Null;

    public override string ToString()
        => CONSTS.NULL;

    public override string Repr()
        => C.YELLOW + ToString() + C.END;
}

public sealed class VoidValue
    : RuntimeValue
{
    public static readonly VoidValue Template = new();

    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.Void;

    public override string ToString()
        => CONSTS.EMPTY;

    public override string Repr()
        => ToString();
}
