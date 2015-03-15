using MathLib.Intervals;

namespace WpfUtils
{
    public static class TextFormat
    {
        public static string NumberFormat(this float value)
        {
            if (value < 0.01)
            {
                return "0.00000";
            }

            if (value < 0.1)
            {
                return "0.0000";
            }

            if (value < 1.0)
            {
                return "0.000";
            }

            if (value < 10.0)
            {
                return "0.00";
            }

            return "0.0";
        }

    }
}
