using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public sealed class ListValue(IEnumerable<RuntimeValue> values, TypeSymbol type)
    : RuntimeValue
{
    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = TypeSymbol.List.SetParameters(type);

    public RuntimeValue[] Values { get; } = values.ToArray();

    public override int GetHashCode()
        => HashCode.Combine(Values);

    public override bool Equals(object? obj)
    {
        if (obj is ListValue list)
        {
            if (Values.Length != list.Values.Length)
                return false;

            for (int i = 0; i < Values.Length; i++)
                if (!Values[i].Equals(list.Values[i]))
                    return false;
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        var str = "[";
        for (int i = 0; i < Values.Length; i++)
        {
            var val = Values[i];
            if (i != Values.Length - 1)
                str += $"{val}, ";
            else
                str += $"{val}";
        }
        str += "]";

        return str;
    }
}
