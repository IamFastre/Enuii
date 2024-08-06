namespace Enuii.Symbols.Types;

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
    Function = 1 << 11,

    Any        = int.MaxValue - Void,
    Nones      = Void    | Unknown | Null,
    Number     = Integer | Float,
    Numable    = Number  | Char,
    Enumerable = String  | Range | List,
    Callable   = Function,
}

public static class TypeIDExtension
{
    public static TypeID Nullify(this TypeID id)
        => id.HasFlag(TypeID.Null)
         ? id
         : id | TypeID.Null;

    public static TypeID Denullify(this TypeID id)
        => id.HasFlag(TypeID.Null) && id is not TypeID.Null
         ? id ^ TypeID.Null
         : id;
}
