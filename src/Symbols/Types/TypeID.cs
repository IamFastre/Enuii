namespace Enuii.Symbols.Typing;

[Flags]
public enum TypeID
{
    Void     = 1 << 1,
    Unknown  = 1 << 2,
    Null     = 1 << 3,

    Boolean  = 1 << 4,
    Integer  = 1 << 5,
    Float    = 1 << 6,
    Char     = 1 << 7,
    String   = 1 << 8,
    Range    = 1 << 9,
    List     = 1 << 10,

    Any        = int.MaxValue - Void,
    Nones      = Void    | Unknown | Null,
    Number     = Integer | Float,
    Enumerable = String  | Range | List,
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
