using Enuii.Symbols.Typing;

namespace Enuii.Semantics;

public enum ConversionKind
{
    INVALID,

    Fuzzy,
    AnyToString,

    BoolToInt,
    BoolToFloat,
    BoolToNumber,

    NumberToInt,
    NumberToFloat,
    NumberToChar,

    IntToFloat,
    IntToChar,

    FloatToInt,
    FloatToChar,

    CharToInt,
    CharToFloat,
}

public class ConversionOperation
{
    public static ConversionKind GetConversionKind(TypeSymbol from, TypeSymbol to)
    {
        if (to.Matches(from))
            return ConversionKind.Fuzzy;

        return (from.ID, to.ID) switch
        {
            ( _ , TypeID.String) when from.IsKnown => ConversionKind.AnyToString,

            (TypeID.Boolean, TypeID.Integer)       => ConversionKind.BoolToInt,
            (TypeID.Boolean, TypeID.Float)         => ConversionKind.BoolToFloat,
            (TypeID.Boolean, TypeID.Number)        => ConversionKind.BoolToNumber,

            (TypeID.Number, TypeID.Integer)        => ConversionKind.NumberToInt,
            (TypeID.Number, TypeID.Float)          => ConversionKind.NumberToFloat,
            (TypeID.Number, TypeID.Char)           => ConversionKind.NumberToChar,

            (TypeID.Integer, TypeID.Char)          => ConversionKind.IntToChar,
            (TypeID.Integer, TypeID.Float)         => ConversionKind.IntToFloat,

            (TypeID.Float, TypeID.Integer)         => ConversionKind.FloatToInt,
            (TypeID.Float, TypeID.Char)            => ConversionKind.FloatToChar,

            (TypeID.Char, TypeID.Integer)          => ConversionKind.CharToInt,
            (TypeID.Char, TypeID.Float)            => ConversionKind.CharToFloat,

            _  => ConversionKind.INVALID,
        };
    }
}