using Enuii.Syntax.Lexing;

namespace Enuii.General.Constants;

// Constants need throughout the application
public static class CONSTS
{
    // Symbols
    public const char DOT = '.';
    public const char INF = '∞';

    // Operators
    public const string IN = "in";

    // Quotes
    public const string CharOpen  = "\'‹";
    public const string CharClose = "\'›";
    public const string StrOpen   = "\"«“";
    public const string StrClose  = "\"»”";

    // Constants
    public const string UNKNOWN = "?";
    public const string NULL    = "null";
    public const string NAN     = "NaN";
    public const string NANF    = "NaNf";
    public const string FALSE   = "false";
    public const string MAYBE   = "maybe";
    public const string TRUE    = "true";

    // Keywords
    public const string IF     = "if";
    public const string ELSE   = "else";
    public const string WHILE  = "while";
    public const string FOR    = "for";
    public const string DELETE = "delete";

    // Types
    public const string ANY       = "any";
    public const string VOID      = "void";
    public const string BOOLEAN   = "bool";
    public const string NUMBER    = "number";
    public const string INTEGER   = "int";
    public const string FLOAT     = "float";
    public const string CHAR      = "char";
    public const string STRING    = "string";
    public const string RANGE     = "range";
    public const string LIST      = "list";
    public const string FUNCTION  = "function";
    public const string CLASS     = "class";

    // Others
    public const string EMPTY = "";

    // Groups
    public static readonly string[] TYPES = [ ANY, VOID, NULL, BOOLEAN, NUMBER, INTEGER, FLOAT, CHAR, STRING, RANGE, LIST, FUNCTION, CLASS ];

    // Gets closing pair of an opening quote
    public static (char, TokenKind) GetQuotePair(char q)
    {
        int i;
        if ((i = CharOpen.IndexOf(q)) > -1)
            return (CharClose[i], TokenKind.Char);
        else if ((i = StrOpen.IndexOf(q)) > -1)
            return (StrClose[i], TokenKind.String);
        else
            throw new Exception($"Could not find the quote pair for this char while lexing: {q}");
    }

    public static TokenKind GetIdentifierKind(string value)
    => value switch
    {
        // Operators
        IN    => TokenKind.In,

        // Constants
        NULL  => TokenKind.Null,
        NAN   => TokenKind.Integer,
        NANF  => TokenKind.Float,
        FALSE => TokenKind.Boolean,
        MAYBE => TokenKind.Boolean,
        TRUE  => TokenKind.Boolean,

        // Keywords
        IF     => TokenKind.If,
        ELSE   => TokenKind.Else,
        WHILE  => TokenKind.While,
        FOR    => TokenKind.For,
        DELETE => TokenKind.Delete,

        _ when TYPES.Contains(value) => TokenKind.Type,
        _                            => TokenKind.Identifier,
    };
}
