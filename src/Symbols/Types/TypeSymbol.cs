using Enuii.General.Constants;
using Enuii.Syntax.AST;

namespace Enuii.Symbols.Typing;

public class TypeSymbol(string name, TypeID id, IEnumerable<TypeSymbol>? parameters = null)
    : Symbol(name)
{
    public TypeID        ID         { get; } = id;
    public TypeSymbol[]? Parameters { get; } = parameters?.ToArray();

    // Metadata
    public bool IsKnown   { get; } = id is not TypeID.Unknown;
    public bool IsGeneric { get; } = parameters is not null;

    public virtual bool HasFlag(TypeSymbol other)
        => ID.HasFlag(other.ID);


    /* ====================================================================== */
    /*                                  Types                                 */
    /* ====================================================================== */

    public static readonly TypeSymbol Any     = new(CONSTS.ANY,     TypeID.Any);
    public static readonly TypeSymbol Unknown = new(CONSTS.UNKNOWN, TypeID.Unknown);

    public static readonly TypeSymbol Null    = new(CONSTS.NULL,    TypeID.Null);
    public static readonly TypeSymbol Boolean = new(CONSTS.BOOLEAN, TypeID.Boolean);
    public static readonly TypeSymbol Number  = new(CONSTS.NUMBER,  TypeID.Number);
    public static readonly TypeSymbol Integer = new(CONSTS.INTEGER, TypeID.Integer);
    public static readonly TypeSymbol Float   = new(CONSTS.FLOAT,   TypeID.Float);
    public static readonly TypeSymbol Char    = new(CONSTS.CHAR,    TypeID.Char);
    public static readonly TypeSymbol String  = new(CONSTS.STRING,  TypeID.String);
    public static readonly TypeSymbol Range   = new(CONSTS.RANGE,   TypeID.Range);

    public static readonly TypeSymbol[] CONCRETES = [ Any, Null, Boolean, Number, Integer, Float, Char, String, Range ];

    /* ====================================================================== */

    internal static TypeSymbol List(IEnumerable<TypeSymbol> element)
        => new(CONSTS.LIST, TypeID.List, element);

    public static readonly Func<IEnumerable<TypeSymbol>, TypeSymbol>[] GENERICS = [ List ];


    /* ====================================================================== */
    /*                                 Statics                                */
    /* ====================================================================== */

    internal static TypeSymbol GetConstantType(NodeKind kind)
    => kind switch
    {
        NodeKind.Unknown => Unknown,
        NodeKind.Null    => Null,
        NodeKind.Boolean => Boolean,
        NodeKind.Integer => Integer,
        NodeKind.Float   => Float,
        NodeKind.Char    => Char,
        NodeKind.String  => String,
        NodeKind.Range   => Range,

        _ => throw new Exception($"Unrecognized constant literal kind while analyzing: {kind}"),
    };

    internal static TypeSymbol GetStringType(string name)
    {
        foreach (var t in CONCRETES)
            if (t.Name == name)
                return t;

        return Unknown;
    }

    internal static bool GetCommonType(TypeSymbol type1, TypeSymbol type2, ref TypeSymbol result)
    {
        if (type1.HasFlag(type2))
        {
            result = type1;
            return true;
        }

        if (type2.HasFlag(type1))
        {
            result = type2;
            return true;
        }

        foreach (var t in CONCRETES)
            if (t.ID != TypeID.Any && t.HasFlag(type1) && t.HasFlag(type2))
            {
                result = t;
                return true;
            }

        return false;
    }
}
