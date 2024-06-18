using Enuii.Symbols.Types;

namespace Enuii.Runtime.Evaluation;

public sealed class ListValue(IEnumerable<RuntimeValue> values, TypeSymbol type, bool typeIsElement = false)
    : RuntimeValue, IEnumerableValue<RuntimeValue>
{
    public override object     Value { get; } = null!;
    public override TypeSymbol Type  { get; } = typeIsElement ? TypeSymbol.List(type) : type;

    public RuntimeValue[] Values { get; } = values.ToArray();

    public override int GetHashCode()
        => HashCode.Combine(Values);

    public override bool Equals(object? obj)
    {
        if (obj is ListValue list)
        {
            if (Values.LongLength != list.Values.LongLength)
                return false;

            for (int i = 0; i < Values.LongLength; i++)
                if (!Values[i].Equals(list.Values[i]))
                    return false;
            return true;
        }
        return false;
    }

    public override string Repr()
        => "[" + string.Join(", ", Values.Select(e => e.Repr())) + "]";

    public override string ToString()
    {
        var str = "[";
        for (int i = 0; i < Values.LongLength; i++)
        {
            var val = Values[i];
            if (i != Values.LongLength - 1)
                str += $"{val}, ";
            else
                str += $"{val}";
        }
        str += "]";

        return str;
    }

    /* ============================ Enumerability =========================== */

    public double Length => Values.LongLength;

    public RuntimeValue ElementAt(int index)
        => Values[index];

    public bool Contains(RuntimeValue value)
        => Values.Contains(value);
}
