using Enuii.General.Constants;
using Enuii.Syntax.AST;

namespace Enuii.Symbols.Typing;

public class TypeSymbol(string name, TypeID id)
    : Symbol(name)
{
    public TypeID ID { get; } = id;

    // Metadata
    public bool IsKnown { get; } = id is not TypeID.Unknown;

    public virtual bool Matches(TypeSymbol other)
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

    public static readonly TypeSymbol[] TYPES = [ Any, Null, Boolean, Number, Integer, Float, Char, String, Range ];


    /* ====================================================================== */
    /*                                 Statics                                */
    /* ====================================================================== */

    internal static TypeSymbol GetLiteralType(NodeKind kind)
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

        _ => throw new Exception($"Unrecognized literal kind while analyzing: {kind}"),
    };

    internal static TypeSymbol GetStringType(string name)
    {
        foreach (var t in TYPES)
            if (t.Name == name)
                return t;
        
        return Unknown;
    }
}
