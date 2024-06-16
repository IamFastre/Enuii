using System.Collections.Immutable;
using Enuii.Runtime.Evaluation;
using Enuii.Semantics;
using Enuii.Symbols.Types;

namespace Enuii.Runtime.Conversion;

public static class Converter
{
    public static RuntimeValue Convert(RuntimeValue value, ConversionKind kind, TypeSymbol to)
    {
        switch (kind)
        {
            /* =========================== General ========================== */

            case ConversionKind.Fuzzy:
                return value;

            case ConversionKind.Polish:
                return Convert(value, ConversionOperation.GetConversionKind(value.Type, to), to);

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
                return new IntValue((double) value.Value);

            case ConversionKind.IntToFloat:
                return new FloatValue((double) value.Value);

            case ConversionKind.NumberToChar:
            case ConversionKind.IntToChar:
            case ConversionKind.FloatToChar:
                return new CharValue((char) Math.Floor((double) value.Value));

            /* ============================ Chars =========================== */

            case ConversionKind.CharToInt:
                return new IntValue((char) value.Value);

            case ConversionKind.CharToFloat:
                return new FloatValue((char) value.Value);

            /* =========================== Ranges =========================== */

            case ConversionKind.RangeToNumberList:
                var nums = ImmutableArray.CreateBuilder<NumberValue>();

                for (int i = 0; i < ((RangeValue) value).Length; i++)
                    nums.Add(((RangeValue) value).ElementAt(i).ToBest());

                return new ListValue(nums, TypeSymbol.Number, true);

            case ConversionKind.RangeToIntList:
                var ints = ImmutableArray.CreateBuilder<IntValue>();

                for (int i = 0; i < ((RangeValue) value).Length; i++)
                    ints.Add(((RangeValue) value).ElementAt(i).ToInt());

                return new ListValue(ints, TypeSymbol.Integer, true);

            case ConversionKind.RangeToFloatList:
                var flts = ImmutableArray.CreateBuilder<FloatValue>();

                for (int i = 0; i < ((RangeValue) value).Length; i++)
                    flts.Add(((RangeValue) value).ElementAt(i).ToFloat());

                return new ListValue(flts, TypeSymbol.Float, true);

            /* =========================== Strings ========================== */

            case ConversionKind.StringToCharList:
                var chrs = ImmutableArray.CreateBuilder<CharValue>();

                for (int i = 0; i < ((StringValue) value).Length; i++)
                    chrs.Add(((StringValue) value).ElementAt(i));

                return new ListValue(chrs, TypeSymbol.Char, true);

            case ConversionKind.StringToStringList:
                var prts = ImmutableArray.CreateBuilder<StringValue>();

                for (int i = 0; i < ((StringValue) value).Length; i++)
                    prts.Add(new(((StringValue) value).ElementAt(i).ToString()));

                return new ListValue(prts, TypeSymbol.String, true);

            default:
                return UnknownValue.Template;
        }
    }
}