using Enuii.General.Colors;

namespace Enuii.Main;

public static class Enuii
{
    public static class Defaults
    {
        public static string Chevron { get; } = $"{C.BLACK2}<{GetSymbol()}{C.BLACK2}>{C.END} ";

        // Special events
        private static bool   IsValentine => DateTime.Now.Month == 2 && DateTime.Now.Day == 14;
        private static bool   IsSpecial   => DateTime.Now.Month == 4 && DateTime.Now.Day == 28;
        private static bool   IsMoony     => DateTime.Now.Month == 8 && DateTime.Now.Day == 31;
        private static bool   IsMusicDay  => DateTime.Now.Month == 6 && DateTime.Now.Day == 21;
        // Special colors
        private static string PINK => C.RGB(255,  70, 130) + C.BLINK;
        private static string GOLD => C.RGB(247, 186,   0) + C.BLINK;
        private static string PURP => C.RGB(132,   0, 255) + C.BLINK;
        private static string GREN => C.RGB( 30, 215,  96) + C.BLINK;

        private static string GetSymbol()
        {
            string sym;
            if (IsValentine)
                sym = $"{PINK}♥";
            else if (IsSpecial)
                sym = $"{GOLD}★";
            else if (IsMoony)
                sym = $"{PURP}☾";
            else if (IsMusicDay)
                sym = $"{GREN}♪";
            else
                sym = $"{C.GREEN2}+";

            return sym + C.END;
        }
    }
}
