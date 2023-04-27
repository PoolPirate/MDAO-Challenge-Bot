namespace MDAO_Challenge_Bot.Utils;
public static class MathUtils
{
    public static double RoundToSignificantDigits(this double d, int digits)
    {
        if (d == 0)
        {
            return 0;
        }

        double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
        return scale * Math.Round(d / scale, digits);
    }
}
