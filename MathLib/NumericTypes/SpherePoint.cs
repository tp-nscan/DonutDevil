using System;

namespace MathLib.NumericTypes
{
    public static class SpherePoint
    {
        public static float[] Random(Random random)
        {
            while (true)
            {
                var randX = (float)random.NextDouble() * 2 - 1.0f;
                var randY = (float)random.NextDouble() * 2 - 1.0f;
                var randZ = (float)random.NextDouble() * 2 - 1.0f;

                var lengthSq = randX*randX + randY*randY + randZ*randZ;

                if (lengthSq < 1)
                {
                    var length = (float)Math.Sqrt(lengthSq);
                    return new[] {randX/length, randY/length, randZ/length};
                }
            }
        }

        public static float[] Make(float x, float y, float z)
        {
            var lengthSq = x * x + y * y + z * z;
            var length = (float)Math.Sqrt(lengthSq);
            if (float.IsNaN(length))
            {
                return new[] { 1.0f, 0.0f, 0.0f };
            }
            return new[] { x / length, y / length, z / length };
            
        }

        public static float[] Thresh(float x, float y, float z, float thresh)
        {
            var lengthSq = x * x + y * y + z * z;
            if (lengthSq > thresh)
            {
                return new[] {0.0f, 0.0f, 0.0f};
            }
            var tented = lengthSq.Tent(thresh / 2.0f);
            return new[] { x * tented, y * tented, z * tented };

        }

    }
}
