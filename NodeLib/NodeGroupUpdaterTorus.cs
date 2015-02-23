using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathLib.NumericTypes;

namespace NodeLib
{
     public static class NodeGroupUpdaterTorus
    {
         public static INodeGroupUpdater ForSquareTorus(float gain, float step, float range, int squareSize, bool use8Way)
         {
             return new NodeGroupUpdaterImpl(
                 Enumerable.Range(0, squareSize * squareSize)
                           .Select(n2 =>
                               DualRingFunc
                                   (
                                       position: n2,
                                       step: step,
                                       range: range,
                                       squareSize: squareSize,
                                       use8Way: use8Way
                                   )
                               )
                           .ToList()
                 );
         }

         static Func<INodeGroup, INode[]> DualRingFunc(
                int position, float step, float range, int squareSize, bool use8Way)
         {
             if (use8Way)
             {
                 return (ng) =>
                 {
                     var orig = ng.Values[position];

                     var res =
                         position.PerimeterOnDt(squareSize, squareSize)
                             .Select(i => ng.Values[i])
                             .Sum(n => orig.MfDeltaAsFloat(n) * step);

                     return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res).AsMf(),
                                groupIndex: position
                            )
                    };
                 };
             }
             return (ng) =>
             {
                 var orig = ng.Values[position];

                 var res =
                     position.SidesOnDt(squareSize, squareSize)
                         .Select(i => ng.Values[i])
                         .Sum(n => orig.MfDeltaAsFloat(n) * step);

                 return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res).AsMf(),
                                groupIndex: position
                            )
                    };
             };


         }

    }
}
