using System;
using System.Linq;
using MathLib.NumericTypes;

namespace NodeLib
{
     public static class NodeGroupUpdaterTorus
    {
         public static INodeGroupUpdater ForSquareTorus
             (
                 float gain,
                 float step,
                 float range,
                 int squareSize,
                 bool use8Way
             )
         {
             if (use8Way)
             {
                 return new NodeGroupUpdaterImpl
                 (
                    Enumerable.Range(0, squareSize * squareSize)
                              .Select(n2 =>
                                  DualAsymRingFunc
                                      (
                                          torusNbrhdOne: n2.ToTorusNbrs(squareSize, squareSize),
                                          torusNbrhdTwo: n2.ToTorusNbrs(squareSize, squareSize, squareSize * squareSize),
                                          step: step * 15.0f
                                      )
                                  )
                              .ToList()
                 );
             }
             return new NodeGroupUpdaterImpl
             (
                 Enumerable.Range(0, squareSize * squareSize)
                           .Select(n2 =>
                               DualRingFunc
                                   (
                                       torusNbrhdOne: n2.ToTorusNbrs(squareSize,squareSize),
                                       torusNbrhdTwo: n2.ToTorusNbrs(squareSize, squareSize, squareSize * squareSize),
                                       step: step
                                   )
                               )
                           .ToList()
              );
         }


         public static INodeGroupUpdater ForSquareTorusOld
             (
                 float gain, 
                 float step, 
                 float range, 
                 int squareSize, 
                 bool use8Way
             )
         {
             return new NodeGroupUpdaterImpl
             (
                 Enumerable.Range(0, squareSize * squareSize)
                           .Select(n2 =>
                               DualRingFuncOld
                                   (
                                       position: n2,
                                       offset: 0,
                                       step: step,
                                       range: range,
                                       squareSize: squareSize,
                                       use8Way: use8Way
                                   )
                               )
                  .Concat( 
                        Enumerable.Range(0, squareSize * squareSize)
                           .Select(n2 =>
                               DualRingFuncOld
                                   (
                                       position: n2,
                                       offset: squareSize * squareSize,
                                       step: step,
                                       range: range,
                                       squareSize: squareSize,
                                       use8Way: use8Way
                                   )
                               )
                  )
                  .ToList()
              );
         }


         static Func<INodeGroup, INode[]> DualRingFunc(
               TorusNbrhd torusNbrhdOne,
               TorusNbrhd torusNbrhdTwo,
               float step)
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];



                 var resOne = cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.UC]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.CR]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.LC]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.CF]) * step;


                 var resTwo = cTwo.MfDeltaAsFloat(ng.Values[torusNbrhdTwo.UC]) * step;
                 resTwo += cTwo.MfDeltaAsFloat(ng.Values[torusNbrhdTwo.CR]) * step;
                 resTwo += cTwo.MfDeltaAsFloat(ng.Values[torusNbrhdTwo.LC]) * step;
                 resTwo += cTwo.MfDeltaAsFloat(ng.Values[torusNbrhdTwo.CF]) * step;


                 return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        Node.Make
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }

         static Func<INodeGroup, INode[]> DualAsymRingFunc(
              TorusNbrhd torusNbrhdOne,
              TorusNbrhd torusNbrhdTwo,
              float step)
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];



                 var resOne = cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.UC]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.CR]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.LC]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.CF]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.CR]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.LC]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhdOne.CF]) * step;





                 var resTwo = cTwo.MfDeltaAsFloat(ng.Values[torusNbrhdTwo.UC]) * step;
                 resTwo += cTwo.MfDeltaAsFloat(ng.Values[torusNbrhdTwo.CR]) * step;
                 resTwo += cTwo.MfDeltaAsFloat(ng.Values[torusNbrhdTwo.LC]) * step;
                 resTwo += cTwo.MfDeltaAsFloat(ng.Values[torusNbrhdTwo.CF]) * step;




                 return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        Node.Make
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }



















         static Func<INodeGroup, INode[]> TorusFunc(
               TorusNbrhd torusNbrhdOne,
               TorusNbrhd torusNbrhdTwo,
               float step)
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];

                 var dUC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UC], ng.Values[torusNbrhdTwo.UC]);
                 var dCR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CR], ng.Values[torusNbrhdTwo.CR]);
                 var dLC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LC], ng.Values[torusNbrhdTwo.LC]);
                 var dCF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CF], ng.Values[torusNbrhdTwo.CF]);


                 var resOne = dUC[0] * step * dUC[2];
                 resOne += dCR[0] * step * dCR[2];
                 resOne += dLC[0] * step * dLC[2];
                 resOne += dCF[0] * step * dCF[2];


                 var resTwo = dUC[1] * step * dUC[2];
                 resTwo += dCR[1] * step * dCR[2];
                 resTwo += dLC[1] * step * dLC[2];
                 resTwo += dCF[1] * step * dCF[2];


                 return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        Node.Make
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }


         static Func<INodeGroup, INode[]> TorusFuncP(
              TorusNbrhd torusNbrhdOne,
              TorusNbrhd torusNbrhdTwo,
              float step)
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];

                 var dUF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UF], ng.Values[torusNbrhdTwo.UF]);
                 var dUC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UC], ng.Values[torusNbrhdTwo.UC]);
                 var dUR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UR], ng.Values[torusNbrhdTwo.UR]);
                 var dCF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CF], ng.Values[torusNbrhdTwo.CF]);
                 var dCR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CR], ng.Values[torusNbrhdTwo.CR]);
                 var dLF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LF], ng.Values[torusNbrhdTwo.LF]);
                 var dLC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LC], ng.Values[torusNbrhdTwo.LC]);
                 var dLR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LR], ng.Values[torusNbrhdTwo.LR]);


                 var resOne = dUF[0] * step * dUF[2];
                 resOne += dUC[0] * step * dUC[2];
                 resOne += dUR[0] * step * dUR[2];
                 resOne += dCF[0] * step * dCF[2];
                 resOne += dCR[0] * step * dCR[2];
                 resOne += dLF[0] * step * dLF[2];
                 resOne += dLC[0] * step * dLC[2];
                 resOne += dLR[0] * step * dLR[2];

                 var resTwo = dUF[1] * step * dUF[2];
                 resTwo += dUC[1] * step * dUC[2];
                 resTwo += dUR[1] * step * dUR[2];
                 resTwo += dCF[1] * step * dCF[2];
                 resTwo += dCR[1] * step * dCR[2];
                 resTwo += dLF[1] * step * dLF[2];
                 resTwo += dLC[1] * step * dLC[2];
                 resTwo += dLR[1] * step * dLR[2];

                 return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        Node.Make
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }



         static Func<INodeGroup, INode[]> TorusFuncP2(
              TorusNbrhd torusNbrhdOne,
              TorusNbrhd torusNbrhdTwo,
              float step)
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];





                 var dUuC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UuC], ng.Values[torusNbrhdTwo.UuC]);
                 var dCRr = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CRr], ng.Values[torusNbrhdTwo.CRr]);
                 var dLlC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LlC], ng.Values[torusNbrhdTwo.LlC]);
                 var dCFf = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CFf], ng.Values[torusNbrhdTwo.CFf]);


                 var dUF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UF], ng.Values[torusNbrhdTwo.UF]);
                 var dUC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UC], ng.Values[torusNbrhdTwo.UC]);
                 var dUR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UR], ng.Values[torusNbrhdTwo.UR]);
                 var dCF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CF], ng.Values[torusNbrhdTwo.CF]);
                 var dCR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CR], ng.Values[torusNbrhdTwo.CR]);
                 var dLF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LF], ng.Values[torusNbrhdTwo.LF]);
                 var dLC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LC], ng.Values[torusNbrhdTwo.LC]);
                 var dLR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LR], ng.Values[torusNbrhdTwo.LR]);


                 var resOne = dUF[0] * step * dUF[2];
                 resOne += dUC[0] * step * dUC[2];
                 resOne += dUR[0] * step * dUR[2];
                 resOne += dCF[0] * step * dCF[2];
                 resOne += dCR[0] * step * dCR[2];
                 resOne += dLF[0] * step * dLF[2];
                 resOne += dLC[0] * step * dLC[2];
                 resOne += dLR[0] * step * dLR[2];
                 resOne += dUuC[0] * step * dUuC[2];
                 resOne += dCRr[0] * step * dCRr[2];
                 resOne += dLlC[0] * step * dLlC[2];
                 resOne += dCFf[0] * step * dCFf[2];



                 var resTwo = dUF[1] * step * dUF[2];
                 resTwo += dUC[1] * step * dUC[2];
                 resTwo += dUR[1] * step * dUR[2];
                 resTwo += dCF[1] * step * dCF[2];
                 resTwo += dCR[1] * step * dCR[2];
                 resTwo += dLF[1] * step * dLF[2];
                 resTwo += dLC[1] * step * dLC[2];
                 resTwo += dLR[1] * step * dLR[2];
                 resTwo += dUuC[1] * step * dUuC[2];
                 resTwo += dCRr[1] * step * dCRr[2];
                 resTwo += dLlC[1] * step * dLlC[2];
                 resTwo += dCFf[1] * step * dCFf[2];





                 return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        Node.Make
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }
         static Func<INodeGroup, INode[]> DualRingFuncOld(
                int position, 
                int offset,
                float step, 
                float range, 
                int squareSize, 
                bool use8Way)
         {
             var s2 =
                 position.Sides2OnDt(squareSize, squareSize)
                     .Select(i => i + offset);

             var p2 =
                position.PerimeterOnDt(squareSize, squareSize)
                     .Select(i => i + offset);


             if (use8Way)
             {
                 return (ng) =>
                 {
                     var orig = ng.Values[position];

                     var res =
                           p2.Select(i => ng.Values[i])
                             .Sum(n => orig.MfDeltaAsFloat(n) * step);

                     return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res).AsMf(),
                                groupIndex: position + offset
                            )
                    };
                 };
             }
             return (ng) =>
             {
                 var orig = ng.Values[position];

                 var res =
                       s2.Select(i => ng.Values[i])
                         .Sum(n => orig.MfDeltaAsFloat(n) * step);

                 return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res).AsMf(),
                                groupIndex: position + offset
                            )
                    };
             };


         }



    }
}
