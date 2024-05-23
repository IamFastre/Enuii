namespace Enuii.Symbols.Typing;

[Flags]
public enum TypeID
{
    Unknown  = 1 << 1,
    Null     = 1 << 2,
    Boolean  = 1 << 3,
    Integer  = 1 << 4,
    Float    = 1 << 5,
    Char     = 1 << 6,
    String   = 1 << 7,

    Any       = int.MaxValue,
    Number    = Integer | Float,
}

public static class TypeIDExtension
{
    public static bool Match(this (TypeID L, TypeID R) Ns, TypeID left, TypeID? right = null, bool interchangeable = false)
    {
       if (right is null)
            return left.HasFlag(Ns.L) && left.HasFlag(Ns.R);

        if (interchangeable)
            return left.HasFlag(Ns.L) && right.Value.HasFlag(Ns.R) || left.HasFlag(Ns.R) && right.Value.HasFlag(Ns.L);
        return left.HasFlag(Ns.L) && right.Value.HasFlag(Ns.R);
    }

    public static bool Known(this (TypeID L, TypeID R) Ns)
        => Ns.L.Known() && Ns.R.Known();

    public static bool Known(this TypeID type)
        => type is not TypeID.Unknown;
}
