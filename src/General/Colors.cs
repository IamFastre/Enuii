namespace Enuii.General.Colors;

internal class C
{
    public static string END        = "\u001b[0m";
    public static string BOLD       = "\u001b[1m";
    public static string DIM        = "\u001b[2m";
    public static string ITALIC     = "\u001b[3m";
    public static string UNDERLINE  = "\u001b[4m";
    public static string BLINK      = "\u001b[5m";
    public static string BLINK2     = "\u001b[6m";
    public static string SELECTED   = "\u001b[7m";
    public static string INVISIBLE  = "\u001b[8m";
    public static string STRIKE     = "\u001b[9m";
    public static string UNDERLINE2 = "\u001b[21m";
    public static string ENDULINE   = "\u001b[24m";

    public static string BLACK      = "\u001b[30m";
    public static string RED        = "\u001b[31m";
    public static string GREEN      = "\u001b[32m";
    public static string YELLOW     = "\u001b[33m";
    public static string BLUE       = "\u001b[34m";
    public static string MAGENTA    = "\u001b[35m";
    public static string CYAN       = "\u001b[36m";
    public static string WHITE      = "\u001b[37m";
    public static string BLACK2     = "\u001b[90m";
    public static string RED2       = "\u001b[91m";
    public static string GREEN2     = "\u001b[92m";
    public static string YELLOW2    = "\u001b[93m";
    public static string BLUE2      = "\u001b[94m";
    public static string MAGENTA2   = "\u001b[95m";
    public static string CYAN2      = "\u001b[96m";
    public static string WHITE2     = "\u001b[97m";

    public static bool IsMonochrome = RED.Length == 0;

    public static string RGB(int R, int G, int B, bool isBackground = false)
    {
        var BG = isBackground ? 48 : 38;
        return $"\u001b[{BG};2;{R};{G};{B}m";
    }

    public static void ToPoly()
    {
        WHITE      = "\u001b[37m";
        BLACK      = "\u001b[30m";
        RED        = "\u001b[31m";
        RED2       = "\u001b[91m";
        YELLOW     = "\u001b[33m";
        YELLOW2    = "\u001b[93m";
        GREEN      = "\u001b[32m";
        GREEN2     = "\u001b[92m";
        BLUE       = "\u001b[34m";
        BLUE2      = "\u001b[94m";
        MAGENTA    = "\u001b[35m";
        MAGENTA2   = "\u001b[95m";
    }

    public static void ToMono()
    {
        WHITE      = "";
        BLACK      = "";
        RED        = "";
        RED2       = "";
        YELLOW     = "";
        YELLOW2    = "";
        GREEN      = "";
        GREEN2     = "";
        BLUE       = "";
        BLUE2      = "";
        MAGENTA    = "";
        MAGENTA2   = "";
    }
}
