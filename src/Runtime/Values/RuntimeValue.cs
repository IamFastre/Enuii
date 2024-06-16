using Enuii.Symbols.Types;

namespace Enuii.Runtime.Evaluation;

public abstract class RuntimeValue
{
    public abstract object     Value { get; }
    public abstract TypeSymbol Type  { get; }


    public override int GetHashCode()
        => Value is null ? 0 : Value.GetHashCode();

    public override bool Equals(object? obj)
        => obj is RuntimeValue rv && Type.HasFlag(rv.Type) && Equals(Value, rv.Value);

    public abstract override string ToString();

    public static bool operator !=(RuntimeValue left, RuntimeValue right)
        => !left.Equals(right);

    public static bool operator ==(RuntimeValue left, RuntimeValue right)
        =>  left.Equals(right);

    //* merge with properties when classes are there
    public abstract string Repr();
    public static string Representation(RuntimeValue value) => value switch
    {
        CharValue        => $"'{value}'",
        StringValue      => $"\"{value}\"",
        RangeValue range => $"|{(range.Start is null ? "" : Representation(range.Start)
                            )}:{(range.End   is null ? "" : Representation(range.End)
                            )}:{Representation(range.Step)}|",
        ListValue  list  => $"[{string.Join(", ", list.Values.Select(Representation))}]",

        _                => value.ToString(),
    };
}
