namespace Enuii.Syntax.Lexing;

public enum TokenKind
{
    Error,
    EOF,

    // Parser Ignorables
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
    // Nots
    Tilde,
    BangMark,
    // Binary
    Plus,
    Minus,
    Asterisk,
    ForwardSlash,
    Percent,
    Power,
    // Inty
    Ampersand,
    Pipe,
    Caret,
    LessLess,
    GreaterGreater,
    // Boolean
    DoubleAmpersand,
    DoublePipe,
    DoubleQuestionMark,

    // Assignments
    __ASSIGNMENT_START__,
    Equal,
    PlusEqual,
    MinusEqual,
    AsteriskEqual,
    ForwardSlashEqual,
    PercentEqual,
    AmpersandEqual,
    PipeEqual,
    CaretEqual,
    PowerEqual,
    DoubleAmpersandEqual,
    DoublePipEqual,
    DoubleQuestionMarkEqual,
    __ASSIGNMENT_END__,
    PlusPlus,
    MinusMinus,

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
    Hash,
    Comma,
    Colon,
    QuestionMark,
    DashArrow,
}


internal static class TokenKindExtension
{
    public static bool IsParserIgnorable(this TokenKind kind) => TokenKind.__IGNORABLE_START__ < kind
                                                              && TokenKind.__IGNORABLE_END__   > kind;

    public static bool IsAssignment(this TokenKind kind)      => TokenKind.__ASSIGNMENT_START__ < kind
                                                              && TokenKind.__ASSIGNMENT_END__   > kind;

    public static int UnaryPrecedence(this TokenKind kind) => kind switch
    {
        TokenKind.Plus      or
        TokenKind.Minus     or
        TokenKind.Tilde     or
        TokenKind.BangMark => 8,

        _ => 0,
    };

    public static int BinaryPrecedence(this TokenKind kind) => kind switch
    {
        // Multiplicative
        TokenKind.Asterisk or TokenKind.ForwardSlash or TokenKind.Percent or TokenKind.Power
            => 7,
        // Additive
        TokenKind.Plus or TokenKind.Minus
            => 6,
        // Shifts
        TokenKind.GreaterGreater or TokenKind.LessLess
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
