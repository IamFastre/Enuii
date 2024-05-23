using Enuii.General.Positioning;

namespace Enuii.Syntax.Lexing;

public class Token(string? value, TokenKind kind, Span span)
{
    public string?   Value { get; } = value;
    public TokenKind Kind  { get; } = kind;
    public Span      Span  { get; } = span;

    public static Token EOF(Position? position = null)
        => new(null, TokenKind.EOF, new(position));

    public override string ToString()
        => $"<{Kind}:{Value} @ {Span}>";
}