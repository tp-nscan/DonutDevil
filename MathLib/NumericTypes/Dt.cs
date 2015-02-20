using System.Net;

namespace MathLib.NumericTypes
{
    public static class Dt
    {
        public static int[] SidesOnDt(this int index, int width, int height)
        {
            var row = index / width;
            var col = index % width;
            var rowMi = ((row - 1 + height) % height) * width;
            var rowPi = ((row + 1) % height) * width;
            var colM = (col - 1 + width) % width;
            var colP = (col + 1) % width;

            return new[]
            {
                rowMi + col,
                row* width + colP,
                rowPi + col,
                row* width + colM
            };
        }


        public static int[] CornersOnDt(this int index, int width, int height)
        {
            var row = index / width;
            var col = index % width;
            var rowM = ((row - 1 + height)%height) * width;
            var rowP = ((row + 1) % height) * width;
            var colM = (col - 1 + width) % width;
            var colP = (col + 1) % width;

            return new[]
            {
                rowM + colM,
                rowM + colP,
                rowP + colP,
                rowP + colM
            };
        }

        public static int[] PerimeterOnDt(this int index, int width, int height)
        {
            var row = index / width;
            var col = index % width;
            var rowMi = ((row - 1 + height) % height) * width;
            var rowPi = ((row + 1) % height) * width;
            var colM = (col - 1 + width) % width;
            var colP = (col + 1) % width;

            return new[]
            {
                rowMi + colM,
                rowMi + col,
                rowMi + colP,
                row* width + colP,
                rowPi + colP,
                rowPi + col,
                rowPi + colM,
                row* width + colM
            };
        }


        public static string ToDtBracketFormat(this int index, int width, int height)
        {
            var row = index / width;
            var col = index % width;

            return "[" + row + ", " + col + "]";
        }


    }
}
