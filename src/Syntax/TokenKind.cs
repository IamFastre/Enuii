namespace Enuii.Syntax.Lexing;

public enum TokenKind
{
    Unknown,
    EOF,
    Integer,
    Float,

    // Operators
    Equal,
    Plus,
    Minus,
    Asterisk,
    ForwardSlash,
    Percent,
    BangMark,
    Tilde,
    Ampersand,
    Pipe,
    Caret,
    Power,
}