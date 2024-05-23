using Enuii.General.Positioning;

namespace Enuii.Syntax.Lexing;

public class Token(string? value, TokenKind kind, Span span)
{
    public string?   Value { get; } = value;
    public TokenKind Kind  { get; } = kind;
    public Span      Span  { get; } = span;

    // Make an EOF token with given `position`
    internal static Token EOF(Position position)
        => new("\0", TokenKind.EOF, new(position));

    // Make an EOL token with given `position`
    internal static Token NL(Position position)
        => new("\n", TokenKind.NewLine, new(position));

    public override string ToString()
        => $"<{Kind}:{Value}> @ {Span}";
}