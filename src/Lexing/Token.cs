using Enuii.General.Constants;
using Enuii.General.Positioning;

namespace Enuii.Syntax.Lexing;

public class Token(string value, TokenKind kind, Span span, bool isFabricated = false)
{
    public string    Value        { get; } = value;
    public TokenKind Kind         { get; } = kind;
    public Span      Span         { get; } = span;
    public bool      IsFabricated { get; } = isFabricated;

    // Fabricate a token for matching
    internal static Token Fabricate(Span span)
        => new(CONSTS.UNKNOWN, TokenKind.Unknown, span, true);

    // Make an EOF token with given `position`
    internal static Token EOF(Position position)
        => new("\0", TokenKind.EOF, new(position));

    // Make an EOL token with given `position`
    internal static Token NL(Position position)
        => new("\n", TokenKind.NewLine, new(position));

    public override string ToString()
        => $"<{Kind}:{Value}> @ {Span}";
}
