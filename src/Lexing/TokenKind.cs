namespace Enuii.Syntax.Lexing;

public enum TokenKind
{
    Error,
    EOF,

    __IGNORABLE_START__,
    Unknown,
    NewLine,
    WhiteSpace,
    BigWhiteSpace,
    __IGNORABLE_END__,

    // Literals
    Integer,
    Float,
    Char,
    String,

    // Identifiers
    Identifier,
    Null,
    Boolean,

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


internal static class TokenKindExtension
{
    public static bool IsParserIgnorable(this TokenKind kind) => TokenKind.__IGNORABLE_START__ < kind
                                                              && TokenKind.__IGNORABLE_END__   > kind;
}