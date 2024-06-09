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
    Type,
    Identifier,
    Null,
    Boolean,

    // Keywords
    If,
    Else,
    While,
    For,

    // Operators
    Equal,

    Tilde,
    BangMark,

    Plus,
    Minus,
    Asterisk,
    ForwardSlash,
    Percent,
    Power,

    Ampersand,
    Pipe,
    Caret,

    DoubleAmpersand,
    DoublePipe,
    DoubleQuestionMark,

    // Comparison
    EqualEqual,
    NotEqual,
    Less,
    Greater,
    LessEqual,
    GreaterEqual,
    In,

    // Brackets
    OpenParenthesis,
    CloseParenthesis,
    OpenSquareBracket,
    CloseSquareBracket,
    OpenCurlyBracket,
    CloseCurlyBracket,

    // Others
    Comma,
    Colon,
    QuestionMark,
    SingleArrow,
}


internal static class TokenKindExtension
{
    public static bool IsParserIgnorable(this TokenKind kind) => TokenKind.__IGNORABLE_START__ < kind
                                                              && TokenKind.__IGNORABLE_END__   > kind;

    public static int UnaryPrecedence(this TokenKind kind) => kind switch
    {
        TokenKind.Plus      or
        TokenKind.Minus     or
        TokenKind.Tilde     or
        TokenKind.BangMark => 6,

        _ => 0,
    };

    public static int BinaryPrecedence(this TokenKind kind) => kind switch
    {
        // Multiplicative
        TokenKind.Asterisk or TokenKind.ForwardSlash or TokenKind.Percent or TokenKind.Power
            => 6,
        // Additive
        TokenKind.Plus or TokenKind.Minus
            => 5,
        // Comparative
        TokenKind.EqualEqual or TokenKind.NotEqual or TokenKind.Less or TokenKind.Greater or TokenKind.LessEqual or TokenKind.GreaterEqual or TokenKind.In
            => 4,
        // ANDs
        TokenKind.Ampersand or TokenKind.DoubleAmpersand
            => 3,
        // ORs
        TokenKind.Pipe or TokenKind.Caret or TokenKind.DoublePipe
            => 2,
        // Nullish coalescing
        TokenKind.DoubleQuestionMark
            => 1,

        _ => 0,
    };
}
