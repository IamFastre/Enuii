namespace Enuii.Syntax.Lexing;

public enum TokenKind
{
    Unknown,
    Error,
    EOF,

    // Literals
    Integer,
    Float,
    Char,
    String,

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