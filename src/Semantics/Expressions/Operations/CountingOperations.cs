using Enuii.Symbols.Types;
using Enuii.Syntax.Lexing;

namespace Enuii.Semantics.Operations;

public enum CountingKind
{
    INVALID,
    Increment,
    Decrement,
}

public class CountingOperation
{
    private static bool CanCount(TypeSymbol type)
        => type.ID is TypeID.Number or TypeID.Char;

    public static (CountingKind, bool) GetOperation(TokenKind kind, TypeSymbol type) => kind switch
    {
        TokenKind.PlusPlus   => (CountingKind.Increment, CanCount(type)),
        TokenKind.MinusMinus => (CountingKind.Decrement, CanCount(type)),
        _                    => (CountingKind.INVALID,   CanCount(type)),
    };
}
