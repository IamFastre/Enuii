using Enuii.General.Constants;
using Enuii.Runtime.Evaluation;
using Enuii.Symbols.Types;
using Enuii.Symbols.Names;

namespace Enuii.Symbols;

public static class Builtins
{
    private static readonly NameSymbol PRINT_NAME = new("print", TypeSymbol.Function([TypeSymbol.Void, TypeSymbol.Any]), true);
    private static readonly NameSymbol READ_NAME  = new("read", TypeSymbol.Function([TypeSymbol.String]), true);

    /* ====================================================================== */
    /*                                  Types                                 */
    /* ====================================================================== */


    public static readonly TypeSymbol[] USABLE_TYPES =
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

    public static Dictionary<string, RuntimeValue> GetBuiltins() => new()
    {
        { CONSTS.NULL,  NullValue.Template },
        { CONSTS.TRUE,  BoolValue.True     },
        { CONSTS.FALSE, BoolValue.False    },

        { PRINT_NAME.Name, new BuiltinFunctionValue(PRINT_NAME.Name, PRINT_NAME.Type, [new("value", TypeSymbol.Any, new StringValue("SEX"))]) },
        { READ_NAME.Name,  new BuiltinFunctionValue(READ_NAME.Name,  READ_NAME.Type,  []) },
    };

    public static NameSymbol[] GetBuiltinSemantics() =>
    [
        new(CONSTS.NULL,  TypeSymbol.Null,    true),
        new(CONSTS.TRUE,  TypeSymbol.Boolean, true),
        new(CONSTS.FALSE, TypeSymbol.Boolean, true),

        PRINT_NAME,
        READ_NAME,
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
    => name == PRINT_NAME.Name
     ? PRINT_FUNC(arguments[0]!)
     : name == READ_NAME.Name
     ? READ_FUNC()
     : throw new Exception($"Builtin function '{name}' was not found");
}
