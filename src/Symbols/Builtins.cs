using Enuii.General.Constants;
using Enuii.Runtime.Evaluation;
using Enuii.Symbols.Types;
using Enuii.Symbols.Names;

namespace Enuii.Symbols;

public static class Builtins
{
    private static readonly FunctionSymbol PRINT_SYMBOL = new("print", [new("value", TypeSymbol.Any)], TypeSymbol.Void,   true);
    private static readonly FunctionSymbol READ_SYMBOL  = new("read",  [],                             TypeSymbol.String, true);

    /* ====================================================================== */
    /*                                  Types                                 */
    /* ====================================================================== */

    public static readonly TypeSymbol[] VALUE_TYPES =
    [
        TypeSymbol.Any,
        TypeSymbol.Null,
        TypeSymbol.Boolean,
        TypeSymbol.Number,
        TypeSymbol.Integer,
        TypeSymbol.Float,
        TypeSymbol.Char,
        TypeSymbol.String,
        TypeSymbol.Range,
    ];

   public static readonly TypeSymbol[] ALL_TYPES =
   [
        ..VALUE_TYPES,
        TypeSymbol.Void,
        TypeSymbol.Unknown,
   ];

    public static readonly Dictionary<TypeID, TypeSymbol> NULLABLES = [];

    public static Dictionary<string, RuntimeValue> GetBuiltins() => new()
    {
        { CONSTS.NULL,  NullValue.Template },
        { CONSTS.TRUE,  BoolValue.True     },
        { CONSTS.FALSE, BoolValue.False    },

        { PRINT_SYMBOL.Name, new BuiltinFunctionValue(PRINT_SYMBOL) },
        { READ_SYMBOL.Name,  new BuiltinFunctionValue(READ_SYMBOL) },
    };

    public static NameSymbol[] GetBuiltinSemantics() =>
    [
        new(CONSTS.NULL,  TypeSymbol.Null,    true),
        new(CONSTS.TRUE,  TypeSymbol.Boolean, true),
        new(CONSTS.FALSE, TypeSymbol.Boolean, true),

        PRINT_SYMBOL,
        READ_SYMBOL,
    ];

    /* ====================================================================== */
    /*                                Functions                               */
    /* ====================================================================== */

    private static VoidValue PRINT_FUNC(RuntimeValue str)
    {
        Console.WriteLine(str.ToString());
        return VoidValue.Template;
    }

    private static StringValue READ_FUNC()
        => new (Console.ReadLine() ?? "");

    internal static RuntimeValue CallBuiltin(string name, RuntimeValue?[] arguments)
    => name == PRINT_SYMBOL.Name
     ? PRINT_FUNC(arguments[0]!)
     : name == READ_SYMBOL.Name
     ? READ_FUNC()
     : throw new Exception($"Builtin function '{name}' was not found");
}
