namespace MathLib.NumericTypes
{
    public static class FixedRange
    {
        public static float ToUnitZ(this float val)
        {
            if (val < -1.0f)
            {
                return -1.0f;
            }

            if (val > 1.0f)
            {
                return 1.0f;
            }
            return val;
        }

        public static float ToUnitR(this float val)
        {
            if (val < -0.0f)
            {
                return -0.0f;
            }

            if (val > 1.0f)
            {
                return 1.0f;
            }
            return val;
        }
    }

}
