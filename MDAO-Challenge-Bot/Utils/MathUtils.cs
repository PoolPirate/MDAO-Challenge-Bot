using System.Numerics;

namespace MDAO_Challenge_Bot.Utils;
public static class MathUtils
{
    public static double DecimalAdjustAndRoundToSignificantDigits(BigInteger value, int decimals, int digits)
    {
        double d = Math.Exp(BigInteger.Log(value) - BigInteger.Log(BigInteger.Pow(10, decimals)));

        if (d == 0)
        {
            return 0;
        }

        double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
        return Math.Round(scale * Math.Round(d / scale, digits), digits);
    }
}
