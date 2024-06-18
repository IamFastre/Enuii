using Enuii.General.Constants;
using Enuii.Runtime.Evaluation;
using Enuii.Symbols.Types;
using Enuii.Symbols.Names;

namespace Enuii.Symbols;

public static class Builtins
{
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
    };

    public static NameSymbol[] GetBuiltinSemantics() =>
    [
        new(CONSTS.NULL,  TypeSymbol.Null,    true),
        new(CONSTS.TRUE,  TypeSymbol.Boolean, true),
        new(CONSTS.FALSE, TypeSymbol.Boolean, true),
    ];
}
