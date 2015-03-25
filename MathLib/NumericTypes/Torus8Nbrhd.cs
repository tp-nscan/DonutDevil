namespace MathLib.NumericTypes
{
    public static class Torus8NbrhdExt
    {
        public static Torus8Nbrhd ToTorus8Nbrs(this int index, int width, int height, int offset = 0)
        {
            var R = index / width;
            var C = index % width;

            var rowOffsets = new int[17];
            var colOffsets = new int[17];

            for (var i = -8; i < 9; i++)
            {
                rowOffsets[i + 8] = offset + ((R + i + height) % height) * width;
                colOffsets[i + 8] = (C + i + width) % width;
            }

            return new Torus8Nbrhd(rowOffsets, colOffsets);
        }
    }

    public class Torus8Nbrhd
    {
        public Torus8Nbrhd(int[] rowOffsets, int[] colOffsets)
        {
            _rowOffsets = rowOffsets;
            _colOffsets = colOffsets;
        }

        private readonly int[] _rowOffsets;
        public int[] RowOffsets
        {
            get { return _rowOffsets; }
        }

        private readonly int[] _colOffsets;
        public int[] ColOffsets
        {
            get { return _colOffsets; }
        }
    }
}
