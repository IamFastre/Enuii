using Enuii.General.Constants;
using Enuii.Syntax.AST;

namespace Enuii.Symbols.Typing;

public class TypeSymbol(string name, TypeID id)
{
    public string Name { get; } = name;
    public TypeID ID   { get; } = id;

    public static readonly TypeSymbol Unknown = new(CONSTS.UNKNOWN, TypeID.Unknown);

    public static readonly TypeSymbol Null    = new(CONSTS.NULL, TypeID.Null);
    public static readonly TypeSymbol Boolean = new(CONSTS.BOOLEAN, TypeID.Boolean);
    public static readonly TypeSymbol Integer = new(CONSTS.INTEGER, TypeID.Integer);
    public static readonly TypeSymbol Float   = new(CONSTS.FLOAT,   TypeID.Float);
    public static readonly TypeSymbol Number  = new(CONSTS.NUMBER,  TypeID.Number);
    public static readonly TypeSymbol Char    = new(CONSTS.CHAR,    TypeID.Char);
    public static readonly TypeSymbol String  = new(CONSTS.STRING,  TypeID.String);

    internal static TypeSymbol GetNodeType(NodeKind kind)
    => kind switch
    {
        NodeKind.Null    => Null,
        NodeKind.Boolean => Boolean,
        NodeKind.Integer => Integer,
        NodeKind.Float   => Float,
        NodeKind.Char    => Char,
        NodeKind.String  => String,

        _ => throw new Exception($"Cannot translate token kind {kind} into a type"),
    };
}
