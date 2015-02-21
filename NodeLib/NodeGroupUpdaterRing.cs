using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeLib
{
    public static class NodeGroupUpdaterRing
    {

        public static INodeGroupUpdater ForSquareTorus(double gain, double range, int squareSize, bool use8Way)
        {
            //return new NodeGroupUpdaterImpl<int>(
            //    Enumerable.Range(0, squareSize * squareSize)
            //              .Select(n2 => UpdateFuncForSquareTorusWithNoise(n2, (int)(range * M10.Mod), squareSize, use8Way))
            //              .ToList()
            //    );
            return null;
        }
    }
}
