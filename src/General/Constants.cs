using Enuii.Syntax.Lexing;

namespace Enuii.General.Constants;

// Constants need throughout the application
public static class CONSTS
{
    // Symbols
    public const char DOT = '.';
    public const char INF = '∞';

    // Quotes
    public const string CharOpen  = "\'‹";
    public const string CharClose = "\'›";
    public const string StrOpen   = "\"«“";
    public const string StrClose  = "\"»”";

    // Keywords & Consts
    public const string UNKNOWN = "?";
    public const string NULL    = "null";
    public const string FALSE   = "false";
    public const string MAYBE   = "maybe";
    public const string TRUE    = "true";

    // Types
    public const string ANY       = "any";
    public const string BOOLEAN   = "bool";
    public const string NUMBER    = "number";
    public const string INTEGER   = "int";
    public const string FLOAT     = "float";
    public const string CHAR      = "char";
    public const string STRING    = "string";

    // Groups
    public static readonly string[] TYPES = [ ANY, NULL, BOOLEAN, NUMBER, INTEGER, FLOAT, CHAR, STRING ];

    // Gets closing pair of an opening quote
    public static (char, TokenKind) GetQuotePair(char q)
    {
        int i;
        if ((i = CharOpen.IndexOf(q)) > -1)
            return (CharClose[i], TokenKind.Char);
        else if ((i = StrOpen.IndexOf(q)) > -1)
            return (StrClose[i], TokenKind.String);
        else
            throw new Exception("Not found");
    }

    public static TokenKind GetIdentifierKind(string value)
    => value switch
    {
        NULL     => TokenKind.Null,
        FALSE    => TokenKind.Boolean,
        MAYBE    => TokenKind.Boolean,
        TRUE     => TokenKind.Boolean,

        _ when TYPES.Contains(value) => TokenKind.Type,

        _ => TokenKind.Identifier,
    };
}
