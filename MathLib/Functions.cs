using System;

namespace MathLib
{
    public static class Functions
    {
        static Functions()
        {
            scaffold = new float[TrigFuncSteps, 2];

            for (var i = 0; i < TrigFuncSteps; i++)
            {
                var angle = (i / (double)TrigFuncSteps) * Math.PI * 2.0;
                scaffold[i, 0] = (float)Math.Cos(angle);
                scaffold[i, 1] = (float)Math.Sin(angle);
            }
        }

        public static double SawTooth(double period)
        {
            return 1.0;
        }

        public static float Tent(this float lhs, float max)
        {
            if (lhs < max)
            {
                return lhs;
            }
            if (lhs < 2*max)
            {
                return 2 * max - lhs;
            }
            return 0.0f;
        }

        public static float SinLu(float arg)
        {
            return scaffold[(int) (arg*TrigFuncSteps), 1];
        }

        public static float CosLu(float arg)
        {
            return scaffold[(int) (arg*TrigFuncSteps), 0];
        }

        private static readonly float[,] scaffold;

        public const int TrigFuncSteps = 2000;
    }
}
