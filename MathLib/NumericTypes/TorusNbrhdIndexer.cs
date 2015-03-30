namespace MathLib.NumericTypes
{
    public static class Torus8NbrhdExt
    {
        public static TorusNbrhdIndexer ToTorus8Nbrs(this int index, int width, int height, int radius, int offset = 0)
        {
            var R = index / width;
            var C = index % width;

            var rowOffsets = new int[radius * 2 + 1];
            var colOffsets = new int[radius * 2 + 1];

            for (var i = -8; i < 9; i++)
            {
                rowOffsets[i + 8] = offset + ((R + i + height) % height) * width;
                colOffsets[i + 8] = (C + i + width) % width;
            }

            return new TorusNbrhdIndexer(rowOffsets, colOffsets, radius);
        }
    }

    public class TorusNbrhdIndexer
    {
        public TorusNbrhdIndexer(int[] rowOffsets, int[] colOffsets, int radius)
        {
            _rowOffsets = rowOffsets;
            _colOffsets = colOffsets;
            _radius = radius;
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
        
        private readonly int _radius;
        public int Radius
        {
            get { return _radius; }
        }
    }
}
