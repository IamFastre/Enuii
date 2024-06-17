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
    public static (CountingKind, bool) GetOperation(TokenKind kind, TypeSymbol type) => kind switch
    {
        TokenKind.PlusPlus   => (CountingKind.Increment, TypeSymbol.Number.HasFlag(type)),
        TokenKind.MinusMinus => (CountingKind.Decrement, TypeSymbol.Number.HasFlag(type)),
        _                    => (CountingKind.INVALID,   TypeSymbol.Number.HasFlag(type)),
    };
}