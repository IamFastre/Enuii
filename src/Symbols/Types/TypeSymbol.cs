using Enuii.General.Constants;
using Enuii.Syntax.AST;

namespace Enuii.Symbols.Types;

public class TypeSymbol
    : Symbol
{
    public TypeID         ID         { get; }
    public TypeProperties Properties { get; private set; }

    // Metadata
    public bool IsNull    => ID is TypeID.Null;
    public bool IsKnown   => ID is not TypeID.Unknown;
    public bool IsGeneric => Properties.ArgSize > 0;

    public TypeSymbol(string name, TypeID id, TypeProperties? props = null, Action<TypeSymbol>? onCreate = null)
        : base(name)
    {
        ID         = id;
        Properties = props ?? TypeProperties.Blank;

        onCreate?.Invoke(this);
    }

    public bool HasFlag(TypeSymbol other)
    {
        // if parameter size don't match
        // or if the base ID don't match
        // return false otherwise keep looking
        if (Properties.ArgSize != other.Properties.ArgSize || !ID.HasFlag(other.ID))
            return false;

        // check if each of the parameters do match
        // with the `self` parameters being the parent
        // so that for example `number[]` matches `int[]`
        // but not the other way around
        for (int i = 0; i < Properties.Parameters.Length; i++)
            if (!Properties.Parameters[i].HasFlag(other.Properties.Parameters[i]))
                return false;

        // nothing seems out of order
        return true;
    }

    public TypeSymbol SetParameters(params TypeSymbol[] paramz)
    {
        if (Properties.Parameters is null)
            throw new Exception("Type is not generic to give parameters");

        if (paramz.Length != Properties.ArgSize)
            throw new Exception($"Incorrect number of parameters given to generic type. (given: {paramz.Length}) (required: {Properties.ArgSize})");

        switch (ID)
        {
            case TypeID.List:
                var list = new TypeSymbol(Name, ID, Properties);
                list.Properties = TypeProperties.TypedList(list, paramz[0]);
                return list;

            default:
                var self = new TypeSymbol(Name, ID, Properties);

                for (int i = 0; i < paramz.Length; i++)
                    self.Properties.Parameters[i] = paramz[i];

                return self;
        }
    }

    public override string ToString()
    {
        if (Properties.CustomName is not null)
            return Properties.CustomName.Invoke(this);

        if (!IsGeneric)
            return Name;

        var str = Name;

        str += '<';
        for (int i = 0; i < Properties.Parameters?.Length; i++)
        {
            var p = Properties.Parameters[i];
            if (i == Properties.Parameters.Length - 1)
                str += $"{p}";
            else
                str += $"{p}, ";
        }
        str += '>';

        return str.ToString();
    }

    /* ====================================================================== */
    /*                                  Types                                 */
    /* ====================================================================== */

    public static readonly TypeSymbol Unknown = new(CONSTS.UNKNOWN, TypeID.Unknown);
    public static readonly TypeSymbol Void    = new(CONSTS.VOID,    TypeID.Void);

    public static readonly TypeSymbol Any     = new(CONSTS.ANY,     TypeID.Any);
    public static readonly TypeSymbol Null    = new(CONSTS.NULL,    TypeID.Null);
    public static readonly TypeSymbol Boolean = new(CONSTS.BOOLEAN, TypeID.Boolean);
    public static readonly TypeSymbol Number  = new(CONSTS.NUMBER,  TypeID.Number);
    public static readonly TypeSymbol Integer = new(CONSTS.INTEGER, TypeID.Integer);
    public static readonly TypeSymbol Float   = new(CONSTS.FLOAT,   TypeID.Float);
    public static readonly TypeSymbol Char    = new(CONSTS.CHAR,    TypeID.Char);
    public static readonly TypeSymbol String  = new(CONSTS.STRING,  TypeID.String, TypeProperties.String);
    public static readonly TypeSymbol Range   = new(CONSTS.RANGE,   TypeID.Range,  TypeProperties.Range);
    public static readonly TypeSymbol List    = new(CONSTS.LIST,    TypeID.List,   TypeProperties.List);


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

    internal static TypeSymbol? GetCommonType(TypeSymbol type1, TypeSymbol type2)
    {
        if (type1.HasFlag(type2))
            return type1;

        if (type2.HasFlag(type1))
            return type2;

        foreach (var t in Builtins.USABLE_TYPES)
            if (t.ID != TypeID.Any && t.HasFlag(type1) && t.HasFlag(type2))
                return t;

        return null;
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

        foreach (var t in Builtins.USABLE_TYPES)
            if (t.ID != TypeID.Any && t.HasFlag(type1) && t.HasFlag(type2))
            {
                result = t;
                return true;
            }

        return false;
    }
}
