using Enuii.General.Constants;
using Enuii.Syntax.AST;

namespace Enuii.Symbols.Types;

public class TypeSymbol
    : Symbol
{
    public TypeID         ID         { get; }
    public TypeProperties Properties { get; private set; }

    // Metadata
    public bool IsKnown    => ID is not TypeID.Unknown;
    public bool IsCallable => ID is TypeID.Callable;
    public bool IsNull     => ID is TypeID.Null;
    public bool IsNullable => ID is not TypeID.Null && ID.HasFlag(TypeID.Null);
    public bool IsGeneric  => Properties.ArgSize > 0;


    public TypeSymbol(string name, TypeID id, TypeProperties? props = null)
        : base(name)
    {
        ID         = id;
        Properties = props ?? TypeProperties.Blank;
    }

    public TypeSymbol Annul()
    {
        if (Builtins.NULLABLES.TryGetValue(ID, out TypeSymbol? type))
            return type;

        type = new(Name, ID.Annul(), Properties);
        return type;
    }

    public TypeSymbol Denullify()
        => new(Name, ID.Denullify(), Properties);

    // !! TODO: THIS NEEDS TO BE REWORKED
    public TypeSymbol NullSafe()
        => IsNullable ? Denullify() : this;

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

    public override string ToString()
    {
        var str = Name;

        if (Properties.CustomName is not null)
            str = Properties.CustomName;
        else if (IsGeneric)
        {
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
        }

        return str + (IsNullable ? "?" : "");
    }

    /* ====================================================================== */
    /*                                  Types                                 */
    /* ====================================================================== */

    public static readonly TypeSymbol Unknown  = new(CONSTS.UNKNOWN,  TypeID.Unknown);
    public static readonly TypeSymbol Void     = new(CONSTS.VOID,     TypeID.Void);

    public static readonly TypeSymbol Any      = new(CONSTS.ANY,      TypeID.Any);
    public static readonly TypeSymbol Null     = new(CONSTS.NULL,     TypeID.Null);
    public static readonly TypeSymbol Boolean  = new(CONSTS.BOOLEAN,  TypeID.Boolean);
    public static readonly TypeSymbol Number   = new(CONSTS.NUMBER,   TypeID.Number);
    public static readonly TypeSymbol Integer  = new(CONSTS.INTEGER,  TypeID.Integer);
    public static readonly TypeSymbol Float    = new(CONSTS.FLOAT,    TypeID.Float);
    public static readonly TypeSymbol Char     = new(CONSTS.CHAR,     TypeID.Char);
    public static readonly TypeSymbol String   = new(CONSTS.STRING,   TypeID.String,   TypeProperties.String);
    public static readonly TypeSymbol Range    = new(CONSTS.RANGE,    TypeID.Range,    TypeProperties.Range);

    public static TypeSymbol List(TypeSymbol element)
    {
        var list  = new TypeSymbol(CONSTS.LIST, TypeID.List);
        var props = new TypeProperties(
            ArgSize:     1u,
            ElementType: element,
            Parameters:  [element],
            Indexing:    [(Integer, element), (Range, list),],
            CustomName:  $"{element}[]"
        );

        list.Properties = props;
        return list;
    }

    public static TypeSymbol Function(IEnumerable<TypeSymbol> parameters)
    {
        var func  = new TypeSymbol(CONSTS.FUNCTION, TypeID.Function);
        var props = new TypeProperties(
            Parameters: [..parameters],
            CustomName: $"({string.Join(", ", parameters.ToArray()[1..].Select(e => e.ToString()))}) -> {parameters.First()}"
        );

        func.Properties = props;
        return func;
    }


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

        foreach (var t in Builtins.VALUE_TYPES)
            if (t.ID != TypeID.Any && t.HasFlag(type1) && t.HasFlag(type2))
                return t;

        return null;
    }

    internal static bool GetCommonType(TypeSymbol type1, TypeSymbol type2, out TypeSymbol result)
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

        foreach (var t in Builtins.VALUE_TYPES)
            if (t.ID != TypeID.Any && t.HasFlag(type1) && t.HasFlag(type2))
            {
                result = t;
                return true;
            }

        result = null!;
        return false;
    }
}
