namespace Enuii.General.Utilities;

public static class Utils
{
    public static readonly Random Random = new();

    public static double RandomDouble()
        => Random.NextDouble();
    
    public static bool CoinFlip()
        => RandomDouble() > 0.5;
}
