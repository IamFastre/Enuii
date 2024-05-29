using System.Text;
using Enuii.General.Constants;
using Enuii.Syntax.AST;

namespace Enuii.Symbols.Typing;

public class TypeSymbol(string name, TypeID id, int paramsSize = 0)
    : Symbol(name)
{
    public TypeID       ID         { get; } = id;
    public int          ParamsSize { get; } = paramsSize;
    public TypeSymbol[] Parameters { get; } = new TypeSymbol[paramsSize]; // TODO: PARAMETERS SHOULD HAVE NAMES LIKE `Generic<T1, T2>`

    // Metadata
    public bool IsKnown   { get; } = id is not TypeID.Unknown;
    public bool IsGeneric { get; } = paramsSize > 0;

    public bool HasFlag(TypeSymbol other)
    {
        // if parameter size don't match
        // or if the base ID don't match
        // return false otherwise keep looking
        if (ParamsSize != other.ParamsSize || !ID.HasFlag(other.ID))
            return false;

        // check if each of the parameters do match
        // with the `self` parameters being the parent
        // so that `number[]` matches `int[]` but not the other way
        for (int i = 0; i < Parameters.Length; i++)
            if (!Parameters[i].HasFlag(other.Parameters[i]))
                return false;

        // nothing seems out of order
        return true;
    }

    public TypeSymbol SetParameters(params TypeSymbol[] paramz)
    {
        if (Parameters is null)
            throw new Exception("Type is not generic to give parameters");

        if (paramz.Length != ParamsSize)
            throw new Exception("Incorrect number of parameters given to generic type");

        var self = new TypeSymbol(Name, ID, ParamsSize);

        for (int i = 0; i < paramz.Length; i++)
            self.Parameters[i] = paramz[i];

        return self;
    }

    public override string ToString()
    {
        if (!IsGeneric)
            return Name;

        var str = new StringBuilder(Name);

        str.Append('<');
        for (int i = 0; i < Parameters?.Length; i++)
        {
            var p = Parameters.ElementAt(i);
            if (i == Parameters.Length - 1)
                str.Append($"{p}");
            else
                str.Append($"{p}, ");
        }
        str.Append('>');

        return str.ToString();
    }


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
    public static readonly TypeSymbol List    = new(CONSTS.LIST,    TypeID.List, 1);

    public static readonly TypeSymbol[] CONCRETES = [ Any, Null, Boolean, Number, Integer, Float, Char, String, Range, List ];


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
