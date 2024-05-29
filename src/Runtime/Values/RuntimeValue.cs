using Enuii.Symbols.Typing;

namespace Enuii.Runtime.Evaluation;

public abstract class RuntimeValue
{
    public abstract object     Value { get; }
    public abstract TypeSymbol Type  { get; }

    public abstract override string ToString();

    public override bool Equals(object? obj)
        => obj is RuntimeValue rv && GetType() == obj.GetType() && Value == rv.Value;

    public override int GetHashCode()
        => Value is null ? 0 : Value.GetHashCode();

    public static bool operator !=(RuntimeValue left, RuntimeValue right)
        => !left.Equals(right);

    public static bool operator ==(RuntimeValue left, RuntimeValue right)
        =>  left.Equals(right);
}