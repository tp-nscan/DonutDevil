using System;
using System.Collections.Generic;
using System.Linq;
using LibNode;
using MathLib;
using MathLib.NumericTypes;
using NodeLib.Common;
using NodeLib.Params;

namespace NodeLib.NgUpdaters
{
    public static class NgUpdaterTorus
    {
         public static INgUpdater Standard(
                 string name,
                 int squareSize,
                 int arrayOffset,
                 float stepSize,
                 DualInteractionType dualInteractionType,
                 IReadOnlyDictionary<string, IParameter> otherParams
             )
         {
             switch (dualInteractionType)
             {
                 case DualInteractionType.None:
                     return StandardNone
                         (
                             name: name,
                             squareSize: squareSize,
                             arrayOffset: arrayOffset,
                             stepSize: stepSize,
                             otherParams: otherParams
                         );
                 case DualInteractionType.Direct:
                     return StandardDirect
                         (
                             name: name,
                             squareSize: squareSize,
                             arrayOffset: arrayOffset,
                             stepSize: stepSize,
                             otherParams: otherParams
                         );
                 case DualInteractionType.Euclidean:
                     return null;
                         //StandardEuclidean
                         //(
                         //    name: name,
                         //    squareSize: squareSize,
                         //    arrayOffset: arrayOffset,
                         //    stepSize: stepSize,
                         //    otherParams: otherParams
                         //);
                 case DualInteractionType.RotationalBias:
                     return StandardRotationalBias
                         (
                             name: name,
                             squareSize: squareSize,
                             arrayOffset: arrayOffset,
                             stepSize: stepSize,
                             otherParams: otherParams
                         );
                 default:
                     throw new Exception(" DualInteractionType not handled");
             }
         }

         public static INgUpdater StandardNone(
            string name,
            int squareSize,
            int arrayOffset,
            float stepSize,
            IReadOnlyDictionary<string, IParameter> otherParams
         )
         {
             var noise = (float)otherParams["Noise"].Value;
                return new NgUpdaterImpl
                  (
                   name: "ForSquareTorus",
                   updateFunctions: Enumerable.Range(arrayOffset, squareSize * squareSize)
                               .Select(n2 =>
                                   Ring2UsingPerimeterSaw
                                       (
                                           torusNbrhdOne: n2.ToTorus3Nbrs(squareSize, squareSize, arrayOffset),
                                           torusNbrhdTwo: n2.ToTorus3Nbrs(squareSize, squareSize, squareSize * squareSize + arrayOffset),
                                           step: stepSize * 1.0f,
                                           saw: 0 / 10.0f,
                                           noise: noise
                                       )
                                   )
                               .ToList()
                   );
            }

         public static INgUpdater StandardDirect(
            string name,
            int squareSize,
            int arrayOffset,
            float stepSize,
            IReadOnlyDictionary<string, IParameter> otherParams
        )
         {
             return null;
         }

        // public static INgUpdater StandardEuclidean(
        //   string name,
        //   int squareSize,
        //   int arrayOffset,
        //   float stepSize,
        //   IReadOnlyDictionary<string, IParameter> otherParams
        //)
        // {
        //     var noise = (float)otherParams["Noise"].Value;
        //     return new NgUpdaterImpl
        //       (
        //        name: "ForSquareTorus",
        //        updateFunctions: Enumerable.Range(arrayOffset, squareSize * squareSize)
        //                    .Select(n2 =>
        //                        EuclidPerimeter
        //                            (
        //                                torusNbrhdOne: n2.ToTorus3Nbrs(squareSize, squareSize, arrayOffset),
        //                                torusNbrhdTwo: n2.ToTorus3Nbrs(squareSize, squareSize, squareSize * squareSize + arrayOffset),
        //                                step: stepSize * 10.0f,
        //                                tent: 0.25f,
        //                                saw: 0 / 10.0f,
        //                                noise: noise
        //                            )
        //                        )
        //                    .ToList()
        //        );
        // }


         public static INgUpdater StandardRotationalBias(
           string name,
           int squareSize,
           int arrayOffset,
           float stepSize,
           IReadOnlyDictionary<string, IParameter> otherParams
        )
         {
             var noise = (float)otherParams["Noise"].Value;
             return new NgUpdaterImpl
               (
                name: "ForSquareTorus",
                updateFunctions: Enumerable.Range(arrayOffset, squareSize * squareSize)
                            .Select(n2 =>
                                Ring2UsingPerimeterWithRotationalBias
                                    (
                                        torusNbrhdOne: n2.ToTorus3Nbrs(squareSize, squareSize, arrayOffset),
                                        torusNbrhdTwo: n2.ToTorus3Nbrs(squareSize, squareSize, squareSize * squareSize + arrayOffset),
                                        step: stepSize * 1.0f,
                                        bias: 0.3f,
                                        noise: noise
                                    )
                                )
                            .ToList()
                );
         }


         /// <summary>
         ///  2-ring metric with sides nbhd
         /// </summary>
         static Func<NodeGroup, Node[]> Ring2UsingSides(
               Torus3NbrhdIndexer torusNbrhdOne,
               Torus3NbrhdIndexer torusNbrhdTwo,
               float step
             )
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];



                 var resOne = cOne.MfDelta(ng.Values[torusNbrhdOne.UC]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CR]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LC]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CF]) * step;


                 var resTwo = cTwo.MfDelta(ng.Values[torusNbrhdTwo.UC]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CR]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LC]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CF]) * step;


                 return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }


         /// <summary>
         ///  2-ring metric with perimeter nbhd and Saw distance
         /// </summary>
         static Func<NodeGroup, Node[]> Ring2UsingPerimeterSaw(
                Torus3NbrhdIndexer torusNbrhdOne,
                Torus3NbrhdIndexer torusNbrhdTwo,
                float step,
                float saw,
                float noise
             )
         {
             return (ng) =>
             {
                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];


                 var 
                 resOne =  cOne.MfDeltaSaw(ng.Values[torusNbrhdOne.UF], saw) * step;
                 resOne += cOne.MfDeltaSaw(ng.Values[torusNbrhdOne.UC], saw) * step;
                 resOne += cOne.MfDeltaSaw(ng.Values[torusNbrhdOne.UR], saw) * step;
                 resOne += cOne.MfDeltaSaw(ng.Values[torusNbrhdOne.CF], saw) * step;
                 resOne += cOne.MfDeltaSaw(ng.Values[torusNbrhdOne.CR], saw) * step;
                 resOne += cOne.MfDeltaSaw(ng.Values[torusNbrhdOne.LF], saw) * step;
                 resOne += cOne.MfDeltaSaw(ng.Values[torusNbrhdOne.LC], saw) * step;
                 resOne += cOne.MfDeltaSaw(ng.Values[torusNbrhdOne.LR], saw) * step;
                 resOne += (float)((SafeRandom.NextDouble() - 0.5f) * noise);

                 var
                 resTwo  = cTwo.MfDeltaSaw(ng.Values[torusNbrhdTwo.UF], saw) * step;
                 resTwo += cTwo.MfDeltaSaw(ng.Values[torusNbrhdTwo.UC], saw) * step;
                 resTwo += cTwo.MfDeltaSaw(ng.Values[torusNbrhdTwo.UR], saw) * step;
                 resTwo += cTwo.MfDeltaSaw(ng.Values[torusNbrhdTwo.CF], saw) * step;
                 resTwo += cTwo.MfDeltaSaw(ng.Values[torusNbrhdTwo.CR], saw) * step;
                 resTwo += cTwo.MfDeltaSaw(ng.Values[torusNbrhdTwo.LF], saw) * step;
                 resTwo += cTwo.MfDeltaSaw(ng.Values[torusNbrhdTwo.LC], saw) * step;
                 resTwo += cTwo.MfDeltaSaw(ng.Values[torusNbrhdTwo.LR], saw) * step;

                 resTwo += (float)((SafeRandom.NextDouble() - 0.5f) * noise);

                 return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }

         /// <summary>
         ///  2-ring metric with perimeter nbhd
         /// </summary>
         static Func<NodeGroup, Node[]> Ring2UsingPerimeter(
              Torus3NbrhdIndexer torusNbrhdOne,
              Torus3NbrhdIndexer torusNbrhdTwo,
              float step
             )
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];



                 var resOne = cOne.MfDelta(ng.Values[torusNbrhdOne.UF]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UC]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UR]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CF]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CR]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LF]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LC]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LR]) * step;



                 var resTwo = cTwo.MfDelta(ng.Values[torusNbrhdTwo.UF]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UC]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UR]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CF]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CR]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LF]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LC]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LR]) * step;


                 return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }


         /// <summary>
         ///  2-ring metric with perimeter nbhd and rotational bias
         /// </summary>
         static Func<NodeGroup, Node[]> Ring1UsingPerimeterWithRotationalBias(
              Torus3NbrhdIndexer torusNbrhdOne,
              int squareSize,
              float step,
              float alpha,
              float beta
             )
         {
             return (ng) =>
             {

                 var bias = torusNbrhdOne.CC/(float) (squareSize*squareSize);

                 var cOne = ng.Values[torusNbrhdOne.CC];

                 var biasesOne = UpdateUtils.RingRadialCosBiases(step: step, rBias: alpha, aBias: cOne);


                 var resOne = cOne.MfDelta(ng.Values[torusNbrhdOne.UF]) * biasesOne[7];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UC]) * biasesOne[0];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UR]) * biasesOne[1];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CF]) * biasesOne[6];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CR]) * biasesOne[2];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LF]) * biasesOne[5];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LC]) * biasesOne[4];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LR]) * biasesOne[3];

                 resOne += (float)((SafeRandom.NextDouble() - 0.5f) * beta);

                 return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            )
                    };
             };

         }



         /// <summary>
         ///  2-ring metric with perimeter nbhd and rotational bias
         /// </summary>
         static Func<NodeGroup, Node[]> Ring2UsingPerimeterWithRotationalBias(
              Torus3NbrhdIndexer torusNbrhdOne,
              Torus3NbrhdIndexer torusNbrhdTwo,
              float step,
              float bias,
              float noise
             )
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];


                 var biasesOne = UpdateUtils.RingRadialCosBiases(step: step, rBias: bias, aBias: cTwo);


                 var resOne = cOne.MfDelta(ng.Values[torusNbrhdOne.UF]) * biasesOne[7];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UC]) * biasesOne[0];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UR]) * biasesOne[1];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CF]) * biasesOne[6];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CR]) * biasesOne[2];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LF]) * biasesOne[5];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LC]) * biasesOne[4];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LR]) * biasesOne[3];

                 resOne += (float)(( SafeRandom.NextDouble()- 0.5f) * noise);


                 var biasesTwo = UpdateUtils.RingRadialSinBiases(step: step, rBias: bias, aBias: cOne);

                 var resTwo = cTwo.MfDelta(ng.Values[torusNbrhdTwo.UF]) * biasesTwo[7];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UC]) * biasesTwo[0];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UR]) * biasesTwo[1];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CF]) * biasesTwo[6];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CR]) * biasesTwo[2];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LF]) * biasesTwo[5];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LC]) * biasesTwo[4];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LR]) * biasesTwo[3];


                 resTwo += (float)((SafeRandom.NextDouble() - 0.5f) * noise);

                 return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }


         /// <summary>
         ///  3-ring metric with perimeter nbhd and rotational bias
         /// </summary>
         static Func<NodeGroup, Node[]> Ring3UsingPerimeterWithRotationalBias(
              Torus3NbrhdIndexer torusNbrhdOne,
              Torus3NbrhdIndexer torusNbrhdTwo,
              Torus3NbrhdIndexer torusNbrhdThree,
              float step,
              float alpha,
              float beta
             )
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];
                 var cThree = ng.Values[torusNbrhdThree.CC];


                 var biasesOne = UpdateUtils.RingRadialCosBiases(step: step, rBias: alpha, aBias: cTwo);


                 var resOne = cOne.MfDelta(ng.Values[torusNbrhdOne.UF]) * biasesOne[7];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UC]) * biasesOne[0];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UR]) * biasesOne[1];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CF]) * biasesOne[6];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CR]) * biasesOne[2];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LF]) * biasesOne[5];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LC]) * biasesOne[4];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LR]) * biasesOne[3];

                 resOne += (float)((SafeRandom.NextDouble() - 0.5f) * beta);


                 var biasesTwo = UpdateUtils.RingRadialSinBiases(step: step, rBias: alpha, aBias: cThree);

                 var resTwo = cTwo.MfDelta(ng.Values[torusNbrhdTwo.UF]) * biasesTwo[7];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UC]) * biasesTwo[0];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UR]) * biasesTwo[1];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CF]) * biasesTwo[6];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CR]) * biasesTwo[2];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LF]) * biasesTwo[5];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LC]) * biasesTwo[4];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LR]) * biasesTwo[3];


                 resTwo += (float)((SafeRandom.NextDouble() - 0.5f) * beta);



                 var biasesThree = UpdateUtils.RingRadialSinBiases(step: step, rBias: alpha, aBias: cOne);

                 var resThree = cThree.MfDelta(ng.Values[torusNbrhdThree.UF]) * biasesThree[7];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.UC]) * biasesThree[0];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.UR]) * biasesThree[1];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.CF]) * biasesThree[6];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.CR]) * biasesThree[2];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.LF]) * biasesThree[5];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.LC]) * biasesThree[4];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.LR]) * biasesThree[3];


                 resThree += (float)((SafeRandom.NextDouble() - 0.5f) * beta);


                 return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            ),

                       new Node
                            (
                                value: (cThree + resThree).AsMf(),
                                groupIndex: torusNbrhdThree.CC
                            )
                    };
             };

         }


         /// <summary>
         ///  4-ring metric with perimeter nbhd and rotational bias
         /// </summary>
         static Func<NodeGroup, Node[]> Ring4UsingPerimeterWithRotationalBias(
              Torus3NbrhdIndexer torusNbrhdOne,
              Torus3NbrhdIndexer torusNbrhdTwo,
              Torus3NbrhdIndexer torusNbrhdThree,
              Torus3NbrhdIndexer torusNbrhdFour,
              float step,
              float alpha,
              float beta
             )
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];
                 var cThree = ng.Values[torusNbrhdThree.CC];
                 var cFour = ng.Values[torusNbrhdFour.CC];


                 var biasesOne = UpdateUtils.RingRadialCosBiases(step: step, rBias: alpha, aBias: cTwo);


                 var resOne = cOne.MfDelta(ng.Values[torusNbrhdOne.UF]) * biasesOne[7];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UC]) * biasesOne[0];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UR]) * biasesOne[1];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CF]) * biasesOne[6];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CR]) * biasesOne[2];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LF]) * biasesOne[5];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LC]) * biasesOne[4];
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LR]) * biasesOne[3];

                 resOne += (float)((SafeRandom.NextDouble() - 0.5f) * beta);


                 var biasesTwo = UpdateUtils.RingRadialSinBiases(step: step, rBias: alpha, aBias: cThree);

                 var resTwo = cTwo.MfDelta(ng.Values[torusNbrhdTwo.UF]) * biasesTwo[7];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UC]) * biasesTwo[0];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UR]) * biasesTwo[1];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CF]) * biasesTwo[6];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CR]) * biasesTwo[2];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LF]) * biasesTwo[5];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LC]) * biasesTwo[4];
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LR]) * biasesTwo[3];


                 resTwo += (float)((SafeRandom.NextDouble() - 0.5f) * beta);



                 var biasesThree = UpdateUtils.RingRadialSinBiases(step: step, rBias: alpha, aBias: cFour);

                 var resThree = cThree.MfDelta(ng.Values[torusNbrhdThree.UF]) * biasesThree[7];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.UC]) * biasesThree[0];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.UR]) * biasesThree[1];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.CF]) * biasesThree[6];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.CR]) * biasesThree[2];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.LF]) * biasesThree[5];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.LC]) * biasesThree[4];
                 resThree += cThree.MfDelta(ng.Values[torusNbrhdThree.LR]) * biasesThree[3];


                 resThree += (float)((SafeRandom.NextDouble() - 0.5f) * beta);


                 var biasesFour = UpdateUtils.RingRadialSinBiases(step: step, rBias: alpha, aBias: cOne);

                 var resFour = cFour.MfDelta(ng.Values[torusNbrhdFour.UF]) * biasesFour[7];
                 resFour += cFour.MfDelta(ng.Values[torusNbrhdFour.UC]) * biasesFour[0];
                 resFour += cFour.MfDelta(ng.Values[torusNbrhdFour.UR]) * biasesFour[1];
                 resFour += cFour.MfDelta(ng.Values[torusNbrhdFour.CF]) * biasesFour[6];
                 resFour += cFour.MfDelta(ng.Values[torusNbrhdFour.CR]) * biasesFour[2];
                 resFour += cFour.MfDelta(ng.Values[torusNbrhdFour.LF]) * biasesFour[5];
                 resFour += cFour.MfDelta(ng.Values[torusNbrhdFour.LC]) * biasesFour[4];
                 resFour += cFour.MfDelta(ng.Values[torusNbrhdFour.LR]) * biasesFour[3];

                 resFour += (float)((SafeRandom.NextDouble() - 0.5f) * beta);



                 return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            ),

                       new Node
                            (
                                value: (cThree + resThree).AsMf(),
                                groupIndex: torusNbrhdThree.CC
                            ),

                       new Node
                            (
                                value: (cFour + resFour).AsMf(),
                                groupIndex: torusNbrhdFour.CC
                            )
                    };
             };

         }




         /// <summary>
         ///  Torus euclidean metric with sides nbhd
         /// </summary>

         static Func<NodeGroup, Node[]> EuclidSides(
              Torus3NbrhdIndexer torusNbrhdOne,
              Torus3NbrhdIndexer torusNbrhdTwo,
              float step,
              float tent,
              float saw
         )
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];
                 var dUC = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.UC], ng.Values[torusNbrhdTwo.UC], tent, saw);
                 var dCF = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.CF], ng.Values[torusNbrhdTwo.CF], tent, saw);
                 var dCR = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.CR], ng.Values[torusNbrhdTwo.CR], tent, saw);
                 var dLC = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.LC], ng.Values[torusNbrhdTwo.LC], tent, saw);


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
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }


         /// <summary>
         ///  Torus euclidean metric with perimeter nbhd
         /// </summary>
         //static Func<NodeGroup, Node[]> EuclidPerimeter(
         //     Torus3Nbrhd torusNbrhdOne,
         //     Torus3Nbrhd torusNbrhdTwo,
         //     float step,
         //     float tent,
         //     float saw,
         //     double noise
         //)
         //{
         //    return (ng) =>
         //    {


         //        var cOne = ng.Values[torusNbrhdOne.CC];
         //        var cTwo = ng.Values[torusNbrhdTwo.CC];

         //        var dUF = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.UF], ng.Values[torusNbrhdTwo.UF], tent, saw);
         //        var dUC = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.UC], ng.Values[torusNbrhdTwo.UC], tent, saw);
         //        var dUR = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.UR], ng.Values[torusNbrhdTwo.UR], tent, saw);
         //        var dCF = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.CF], ng.Values[torusNbrhdTwo.CF], tent, saw);
         //        var dCR = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.CR], ng.Values[torusNbrhdTwo.CR], tent, saw);
         //        var dLF = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.LF], ng.Values[torusNbrhdTwo.LF], tent, saw);
         //        var dLC = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.LC], ng.Values[torusNbrhdTwo.LC], tent, saw);
         //        var dLR = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.LR], ng.Values[torusNbrhdTwo.LR], tent, saw);


         //        var resOne = dUF[0] * step * dUF[2];
         //        resOne += dUC[0] * step * dUC[2];
         //        resOne += dUR[0] * step * dUR[2];
         //        resOne += dCF[0] * step * dCF[2];
         //        resOne += dCR[0] * step * dCR[2];
         //        resOne += dLF[0] * step * dLF[2];
         //        resOne += dLC[0] * step * dLC[2];
         //        resOne += dLR[0] * step * dLR[2];

         //        resOne += (float)((SafeRandom.NextDouble() - 0.5f) * noise);


         //        var resTwo = dUF[1] * step * dUF[2];
         //        resTwo += dUC[1] * step * dUC[2];
         //        resTwo += dUR[1] * step * dUR[2];
         //        resTwo += dCF[1] * step * dCF[2];
         //        resTwo += dCR[1] * step * dCR[2];
         //        resTwo += dLF[1] * step * dLF[2];
         //        resTwo += dLC[1] * step * dLC[2];
         //        resTwo += dLR[1] * step * dLR[2];

         //        resTwo += (float)((SafeRandom.NextDouble() - 0.5f) * noise);

         //        return new[]
         //           {
         //               SNode.Make
         //                   (
         //                       value: (cOne + resOne).AsMf(),
         //                       groupIndex: torusNbrhdOne.CC
         //                   ),

         //               SNode.Make
         //                   (
         //                       value: (cTwo + resTwo).AsMf(),
         //                       groupIndex: torusNbrhdTwo.CC
         //                   )
         //           };
         //    };

         //}


         /// <summary>
         ///  Torus euclidean metric with star nbhd
         /// </summary>
         static Func<NodeGroup, Node[]> EuclidStar(
              Torus3NbrhdIndexer torusNbrhdOne,
              Torus3NbrhdIndexer torusNbrhdTwo,
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
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }



         /// <summary>
         ///  Torus euclidean metric with star nbhd
         /// </summary>
         static Func<NodeGroup, Node[]> EuclidFuncRadius2Perimeter(
              Torus3NbrhdIndexer torusNbrhdOne,
              Torus3NbrhdIndexer torusNbrhdTwo,
              float step)
         {
             return (ng) =>
             {
                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];


                 var dUuFf = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UuFf], ng.Values[torusNbrhdTwo.UuFf]);
                 var dUuF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UuF], ng.Values[torusNbrhdTwo.UuF]);
                 var dUuC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UuC], ng.Values[torusNbrhdTwo.UuC]);
                 var dUuR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UuR], ng.Values[torusNbrhdTwo.UuR]);
                 var dUuRr = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UuRr], ng.Values[torusNbrhdTwo.UuRr]);

                 var dLlFf = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LlFf], ng.Values[torusNbrhdTwo.LlFf]);
                 var dLlF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LlF], ng.Values[torusNbrhdTwo.LlF]);
                 var dLlC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LlC], ng.Values[torusNbrhdTwo.LlC]);
                 var dLlR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LlR], ng.Values[torusNbrhdTwo.LlR]);
                 var dLlRr = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LlRr], ng.Values[torusNbrhdTwo.LlRr]);


                 var dUFf = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UFf], ng.Values[torusNbrhdTwo.UFf]);
                 var dCFf = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CFf], ng.Values[torusNbrhdTwo.CFf]);
                 var dLFf = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LFf], ng.Values[torusNbrhdTwo.LFf]);

                 var dURr = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.URr], ng.Values[torusNbrhdTwo.URr]);
                 var dCRr = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CRr], ng.Values[torusNbrhdTwo.CRr]);
                 var dLRr = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LRr], ng.Values[torusNbrhdTwo.LRr]);




                 var dUF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UF], ng.Values[torusNbrhdTwo.UF]);
                 var dUC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UC], ng.Values[torusNbrhdTwo.UC]);
                 var dUR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.UR], ng.Values[torusNbrhdTwo.UR]);
                 var dCF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CF], ng.Values[torusNbrhdTwo.CF]);
                 var dCR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.CR], ng.Values[torusNbrhdTwo.CR]);
                 var dLF = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LF], ng.Values[torusNbrhdTwo.LF]);
                 var dLC = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LC], ng.Values[torusNbrhdTwo.LC]);
                 var dLR = Mf2.VDiff(cOne, cTwo, ng.Values[torusNbrhdOne.LR], ng.Values[torusNbrhdTwo.LR]);


                 var 
                     resOne =  dUF[0] * step * dUF[2];
                     resOne += dUC[0] * step * dUC[2];
                     resOne += dUR[0] * step * dUR[2];
                     resOne += dCF[0] * step * dCF[2];
                     resOne += dCR[0] * step * dCR[2];
                     resOne += dLF[0] * step * dLF[2];
                     resOne += dLC[0] * step * dLC[2];
                     resOne += dLR[0] * step * dLR[2];

                     resOne += dUuFf[0] * step * dUuFf[2];
                     resOne += dUuF[0] * step * dUuF[2];
                     resOne += dUuC[0] * step * dUuC[2];
                     resOne += dUuR[0] * step * dUuR[2];
                     resOne += dUuRr[0] * step * dUuRr[2];

                     resOne += dLlFf[0] * step * dLlFf[2];
                     resOne += dLlF[0] * step * dLlF[2];
                     resOne += dLlC[0] * step * dLlC[2];
                     resOne += dLlR[0] * step * dLlR[2];
                     resOne += dLlRr[0] * step * dLlRr[2];

                     resOne += dUFf[0] * step * dUFf[2];
                     resOne += dCFf[0] * step * dCFf[2];
                     resOne += dLFf[0] * step * dLFf[2];

                     resOne += dURr[0] * step * dURr[2];
                     resOne += dCRr[0] * step * dCRr[2];
                     resOne += dLRr[0] * step * dLRr[2];



                 var 
                     resTwo =  dUF[1] * step * dUF[2];
                     resTwo += dUC[1] * step * dUC[2];
                     resTwo += dUR[1] * step * dUR[2];
                     resTwo += dCF[1] * step * dCF[2];
                     resTwo += dCR[1] * step * dCR[2];
                     resTwo += dLF[1] * step * dLF[2];
                     resTwo += dLC[1] * step * dLC[2];
                     resTwo += dLR[1] * step * dLR[2];

                     resTwo += dUuFf[1] * step * dUuFf[2];
                     resTwo += dUuF[1] * step * dUuF[2];
                     resTwo += dUuC[1] * step * dUuC[2];
                     resTwo += dUuR[1] * step * dUuR[2];
                     resTwo += dUuRr[1] * step * dUuRr[2];

                     resTwo += dLlFf[1] * step * dLlFf[2];
                     resTwo += dLlF[1] * step * dLlF[2];
                     resTwo += dLlC[1] * step * dLlC[2];
                     resTwo += dLlR[1] * step * dLlR[2];
                     resTwo += dLlRr[1] * step * dLlRr[2];

                     resTwo += dUFf[1] * step * dUFf[2];
                     resTwo += dCFf[1] * step * dCFf[2];
                     resTwo += dLFf[1] * step * dLFf[2];

                     resTwo += dURr[1] * step * dURr[2];
                     resTwo += dCRr[1] * step * dCRr[2];
                     resTwo += dLRr[1] * step * dLRr[2];


                 return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }



         /// <summary>
         ///  Torus euclidean metric with perimeter nbhd
         /// </summary>
         static Func<NodeGroup, Node[]> EuclidFuncPerimeterTv(
              int squareSize,
              Torus3NbrhdIndexer torusNbrhdOne,
              Torus3NbrhdIndexer torusNbrhdTwo,
              float alpha,
              float step
             )
         {
             const float nbrTentMax = 0.15f;
             const float tvTentMax = 0.020f;
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];

                 var tvXcoord = (int)(cOne * squareSize);
                 var tvYcord = (int)(cTwo * squareSize);
                 var tvCord = tvXcoord*squareSize + tvYcord;

                 var tvXVal = ng.Values[tvCord];
                 var tvYVal = ng.Values[tvCord + squareSize*squareSize];


                 var dTv = Mf2.VDiffTent(cOne, cTwo, tvXVal, tvYVal, tvTentMax);

                 var dUF = Mf2.VDiffTent(cOne, cTwo, ng.Values[torusNbrhdOne.UF], ng.Values[torusNbrhdTwo.UF], nbrTentMax);
                 var dUC = Mf2.VDiffTent(cOne, cTwo, ng.Values[torusNbrhdOne.UC], ng.Values[torusNbrhdTwo.UC], nbrTentMax);
                 var dUR = Mf2.VDiffTent(cOne, cTwo, ng.Values[torusNbrhdOne.UR], ng.Values[torusNbrhdTwo.UR], nbrTentMax);
                 var dCF = Mf2.VDiffTent(cOne, cTwo, ng.Values[torusNbrhdOne.CF], ng.Values[torusNbrhdTwo.CF], nbrTentMax);
                 var dCR = Mf2.VDiffTent(cOne, cTwo, ng.Values[torusNbrhdOne.CR], ng.Values[torusNbrhdTwo.CR], nbrTentMax);
                 var dLF = Mf2.VDiffTent(cOne, cTwo, ng.Values[torusNbrhdOne.LF], ng.Values[torusNbrhdTwo.LF], nbrTentMax);
                 var dLC = Mf2.VDiffTent(cOne, cTwo, ng.Values[torusNbrhdOne.LC], ng.Values[torusNbrhdTwo.LC], nbrTentMax);
                 var dLR = Mf2.VDiffTent(cOne, cTwo, ng.Values[torusNbrhdOne.LR], ng.Values[torusNbrhdTwo.LR], nbrTentMax);


                 var resOne = dUF[0] * step * dUF[2];
                 resOne += dUC[0] * step * dUC[2];
                 resOne += dUR[0] * step * dUR[2];
                 resOne += dCF[0] * step * dCF[2];
                 resOne += dCR[0] * step * dCR[2];
                 resOne += dLF[0] * step * dLF[2];
                 resOne += dLC[0] * step * dLC[2];
                 resOne += dLR[0] * step * dLR[2];

                 resOne += dTv[0] * step * dTv[2] * alpha;




                 var resTwo = dUF[1] * step * dUF[2];
                 resTwo += dUC[1] * step * dUC[2];
                 resTwo += dUR[1] * step * dUR[2];
                 resTwo += dCF[1] * step * dCF[2];
                 resTwo += dCR[1] * step * dCR[2];
                 resTwo += dLF[1] * step * dLF[2];
                 resTwo += dLC[1] * step * dLC[2];
                 resTwo += dLR[1] * step * dLR[2];

                 resTwo += dTv[1] * step * dTv[2] * alpha;


                 return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }



         /// <summary>
         ///  2-ring metric with perimeter nbhd
         /// </summary>
         static Func<NodeGroup, Node[]> Ring2UsingPerimeterTv(
              int squareSize,
              Torus3NbrhdIndexer torusNbrhdOne,
              Torus3NbrhdIndexer torusNbrhdTwo,
              float alpha,
              float step
             )
         {
             return (ng) =>
             {


                 var cOne = ng.Values[torusNbrhdOne.CC];
                 var cTwo = ng.Values[torusNbrhdTwo.CC];

                 var tvXcoord = (int)(cOne * squareSize);
                 var tvYcord = (int)(cTwo * squareSize);
                 var tvCord = tvXcoord * squareSize + tvYcord;

                 var tvXVal = ng.Values[tvCord];
                 var tvYVal = ng.Values[tvCord + squareSize * squareSize];


                 var resOne = cOne.MfDelta(ng.Values[torusNbrhdOne.UF]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UC]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UR]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CF]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CR]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LF]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LC]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LR]) * step;

                 resOne += cOne.MfDelta(tvXVal) * step * alpha;

                 var resTwo = cTwo.MfDelta(ng.Values[torusNbrhdTwo.UF]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UC]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UR]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CF]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CR]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LF]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LC]) * step;
                 resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LR]) * step;

                 resTwo += cTwo.MfDelta(tvYVal) * step * alpha;


                 return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
             };

         }
    }
}


