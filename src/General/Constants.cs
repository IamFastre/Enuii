using Enuii.Syntax.Lexing;

namespace Enuii.General.Constants;

// Constants need throughout the application
public static class Constants
{
    public const char DOT = '.';
    public const char INF = '∞';

    public const string CharOpen  = "\'‹";
    public const string CharClose = "\'›";
    public const string StrOpen   = "\"«“";
    public const string StrClose  = "\"»”";

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
}