using Enuii.Runtime.Evaluation;
using Enuii.Semantics;

namespace Enuii.Runtime.Conversion;

public static class Converter
{
    public static RuntimeValue Convert(RuntimeValue value, ConversionKind kind)
    {
        switch (kind)
        {
            /* =========================== General ========================== */

            case ConversionKind.Fuzzy:
                return value;
            
            case ConversionKind.AnyToString:
                return new StringValue(value.ToString());

            /* ============================ Bools =========================== */

            case ConversionKind.BoolToInt:
            case ConversionKind.BoolToNumber:
                return new IntValue((bool) value.Value ? 1 : 0);

            case ConversionKind.BoolToFloat:
                return new FloatValue((bool) value.Value ? 1 : 0);

            /* =========================== Numbers ========================== */

            case ConversionKind.FloatToInt:
            case ConversionKind.NumberToInt:
                return new IntValue((double) value.Value);

            case ConversionKind.NumberToFloat:
            case ConversionKind.IntToFloat:
                return new FloatValue((double) value.Value);

            case ConversionKind.NumberToChar:
            case ConversionKind.IntToChar:
            case ConversionKind.FloatToChar:
                return new CharValue((char) Math.Floor((double) value.Value));

            /* ============================ Char ============================ */

            case ConversionKind.CharToInt:
                return new IntValue((char) value.Value);

            case ConversionKind.CharToFloat:
                return new FloatValue((char) value.Value);

            /* ============================================================== */
            /* ||                       •-{ Lists }-•                      || */
            /* ============================================================== */
        
            default:
                return UnknownValue.Template;
        }
    }
}