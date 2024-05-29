using Enuii.Symbols.Typing;

namespace Enuii.Symbols;

public static class Builtins
{
    /* ====================================================================== */
    /*                                  Types                                 */
    /* ====================================================================== */
    public static readonly TypeSymbol[] TYPES =
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
        TypeSymbol.List,
    ];
}