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
}
