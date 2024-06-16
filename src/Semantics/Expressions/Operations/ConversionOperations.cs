using Enuii.Symbols.Types;

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

    RangeToNumberList,
    RangeToIntList,
    RangeToFloatList,

    StringToCharList,
    StringToStringList,
}

public class ConversionOperation
{
    public static ConversionKind GetConversionKind(TypeSymbol from, TypeSymbol to)
    {
        if (to.HasFlag(from))
            return ConversionKind.Fuzzy;

        return (from.ID, to.ID) switch
        {
            ( _ , TypeID.String) when from.IsKnown => ConversionKind.AnyToString,

            (TypeID.Boolean, TypeID.Number)  => ConversionKind.BoolToNumber,  // bool -> number
            (TypeID.Boolean, TypeID.Integer) => ConversionKind.BoolToInt,     // bool -> int
            (TypeID.Boolean, TypeID.Float)   => ConversionKind.BoolToFloat,   // bool -> float

            (TypeID.Number, TypeID.Integer)  => ConversionKind.NumberToInt,   // number -> int
            (TypeID.Number, TypeID.Float)    => ConversionKind.NumberToFloat, // number -> float
            (TypeID.Number, TypeID.Char)     => ConversionKind.NumberToChar,  // number -> char

            (TypeID.Integer, TypeID.Char)    => ConversionKind.IntToChar,     // int -> char
            (TypeID.Integer, TypeID.Float)   => ConversionKind.IntToFloat,    // int -> float

            (TypeID.Float, TypeID.Integer)   => ConversionKind.FloatToInt,    // float -> int
            (TypeID.Float, TypeID.Char)      => ConversionKind.FloatToChar,   // float -> char

            (TypeID.Char, TypeID.Integer)    => ConversionKind.CharToInt,     // char -> int
            (TypeID.Char, TypeID.Float)      => ConversionKind.CharToFloat,   // char -> float

            (TypeID.Range, TypeID.List)      => to.Properties.ElementType!.ID is TypeID.Number  // range -> number[]
                                              ? ConversionKind.RangeToNumberList
                                              : to.Properties.ElementType.ID is TypeID.Integer  // range -> int[]
                                              ? ConversionKind.RangeToIntList
                                              : to.Properties.ElementType.ID is TypeID.Float    // range -> float[]
                                              ? ConversionKind.RangeToFloatList
                                              : ConversionKind.INVALID,

            (TypeID.String, TypeID.List)     => to.Properties.ElementType!.ID is TypeID.Char    // string -> char[]
                                              ? ConversionKind.StringToCharList
                                              : to.Properties.ElementType.ID is TypeID.String   // string -> string[]
                                              ? ConversionKind.StringToStringList
                                              : ConversionKind.INVALID,
            _  => ConversionKind.INVALID,
        };
    }
}