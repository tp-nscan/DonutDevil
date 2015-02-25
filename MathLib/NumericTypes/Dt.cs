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
                row * width + colP,
                rowPi + col,
                row * width + colM
            };
        }

        public static int[] Sides2OnDt(this int index, int width, int height)
        {
            var row = index / width;
            var col = index % width; 

            var rowMi = ((row - 1 + height) % height) * width;
            var rowZ = row * width;
            var rowPi = ((row + 1) % height) * width; 

            var colM = (col - 1 + width) % width;
            var colP = (col + 1) % width;

            var rowMi2 = ((row - 2 + height) % height) * width;
            var rowPi2 = ((row + 2) % height) * width;

            var colM2 = (col - 2 + width) % width;
            var colP2 = (col + 2) % width;

            return new[]
            {
                rowMi + col,
                rowMi2 + col,

                rowZ + colP,
                rowZ + colP2,

                rowPi + col,
                rowPi2 + col,

                rowZ  + colM,
                rowZ + colM2
            };
        }

        public static int[] Sides3OnDt(this int index, int width, int height)
        {
            var row = index / width;
            var col = index % width;

            var rowM1 = ((row - 1 + height) % height) * width;
            var rowZ = row * width;
            var rowP1 = ((row + 1) % height) * width;

            var colM1 = (col - 1 + width) % width;
            var colP1 = (col + 1) % width;

            var rowM2 = ((row - 2 + height) % height) * width;
            var rowP2 = ((row + 2) % height) * width;
            var colM2 = (col - 2 + width) % width;
            var colP2 = (col + 2) % width;

            var rowM3 = ((row - 3 + height) % height) * width;
            var rowP3 = ((row + 3) % height) * width;
            var colM3 = (col - 3 + width) % width;
            var colP3 = (col + 3) % width;

            return new[]
            {
                rowM1 + col,
                rowM2 + col,
                rowM3 + col,

                rowZ + colP1,
                rowZ + colP2,
                rowZ + colP3,

                rowP1 + col,
                rowP2 + col,
                rowP3 + col,

                rowZ + colM1,
                rowZ + colM2,
                rowZ + colM3
            };
        }

        public static int[] Star3OnDt(this int index, int width, int height)
        {
            var row = index / width;
            var col = index % width;

            var rowM1 = ((row - 1 + height) % height) * width;
            var rowZ = row * width;
            var rowP1 = ((row + 1) % height) * width;

            var colM1 = (col - 1 + width) % width;
            var colP1 = (col + 1) % width;

            var rowM2 = ((row - 2 + height) % height) * width;
            var rowP2 = ((row + 2) % height) * width;
            var colM2 = (col - 2 + width) % width;
            var colP2 = (col + 2) % width;

            var rowM3 = ((row - 3 + height) % height) * width;
            var rowP3 = ((row + 3) % height) * width;
            var colM3 = (col - 3 + width) % width;
            var colP3 = (col + 3) % width;

            return new[]
            {
                rowM1 + col,
                rowM2 + col,
                rowM3 + col,

                rowZ + colP1,
                rowZ + colP2,
                rowZ + colP3,

                rowP1 + col,
                rowP2 + col,
                rowP3 + col,

                rowZ + colM1,
                rowZ + colM2,
                rowZ + colM3,

                rowM1 + colM1,
                rowM1 + colP1,
                rowP1 + colM1,
                rowP1 + colP1
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
