namespace LibNode
open System
open MathNet.Numerics
open MathNet.Numerics.Random
open MathNet.Numerics.Distributions
open LibNode.MathUtils

module Generators =
    let sharedRng = Random.mersenneTwisterShared

    let SeqIter (s:seq<float32>) =
        let iter = s.GetEnumerator()
        function () ->
                    iter.MoveNext() |> ignore
                    iter.Current

    let NormalSF32 rnd stddev = 
        Normal.Samples(rnd=rnd, mean=0.0, stddev=stddev)
            |> Seq.map(fun v->(FloatToSF32 v))

    let RandomName (prefix:string) = 
        let rng = Random.MersenneTwister()
        let suffix = rng.Next()
        sprintf "%s_%i" prefix suffix

     // a random float32 from (-max, max)
    let RandSF32 (max:float32) (rng:Random) =
        Convert.ToSingle(rng.NextDouble() * 2.0) * max - max

    // a sequence of random float32 from (0, max)
    let SeqOfRandUF32 (max:float32) (rng:Random) =
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()) * max)

    // a sequence of random float32 from (-max, max)
    let SeqOfRandSF32 (max:float32) (rng:Random) =
        Seq.initInfinite ( fun i -> RandSF32 max rng)

    let SeqOfRandBools (rng:Random) (trueProb:float) =
        Seq.initInfinite ( fun i -> rng.NextDouble() < trueProb )

     // random draws from {0f, 1f}
    let SeqOfRandUF32Bits (pOfOne:float32) (rng:Random) =
        let myCollapser = MathUtils.AorB 0.0f 1.0f pOfOne
        Seq.initInfinite ( fun i -> myCollapser (System.Convert.ToSingle(rng.NextDouble())))

     // random draws from {-1f, 1f}
    let SeqOfRandSF32Bits (pOfOne:float32) (rng:Random) =
        let myCollapser = MathUtils.AorB -1.0f 1.0f pOfOne
        Seq.initInfinite ( fun i -> myCollapser (System.Convert.ToSingle(rng.NextDouble())))

    // 0f <=> 1f with probability p
    let FlipUF32WithProb (rng:Random) (p:float) (value:float32) =
        if (rng.NextDouble() < p) then
            1.0f - value
        else
            value

    /// -1f <=> 1f with probability p
    let FlipF32WithProb (probability:float) (rng:Random) (value:float32) =
        if (rng.NextDouble() < probability) then
            value * -1.0f
        else
            value

    /// applies FlipUF32WithProb to values:float32[] 
    let FlipUF32A (p:float32) (rng:Random) (values:float32[]) =
        let dblM = Convert.ToDouble p
        values |> Array.map(fun v -> (FlipUF32WithProb rng dblM v ))


    /// applies FlipF32WithProb to values:float32[] 
    let FlipF32A (p:float32) (rng:Random) (values:float32[]) =
        let dblM = Convert.ToDouble p
        values |> Array.map(fun v -> (FlipF32WithProb dblM rng v ))


    // applies PerturbInRangeF32 to values:float32[] 
    let PerturbInRangeF32A (minVal:float32) (maxVal:float32) 
                           (maxDelta:float32) (rng:Random)
                           (values:float32[]) =
        let rangeAdder = AddInRange minVal maxVal
        values |> Array.map(fun v -> (rangeAdder (RandSF32 maxDelta rng) v))


    let MesForArraySF32 (noiseLevel:float32) (rng:Random) copies (array:float32[][]) = 
        MesForArray array copies (PerturbInRangeF32A -1.0f 1.0f noiseLevel rng)


    let RandomEuclideanPointsF32 (unsigned:bool) (rng:Random) =
        match unsigned with
        | true ->
            Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble() ); 
                                                   y = Convert.ToSingle(rng.NextDouble()) })
        | false ->
            Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()) * 2.0f - 1.0f; 
                                                   y = Convert.ToSingle(rng.NextDouble()) * 2.0f - 1.0f })


    let RandDiscPointsF32 (maxRadius:float32) (rng:Random) =
        Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()); 
                                               y = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.filter(fun p -> (p |> PointF32LengthSquared) < 1.0f)
                            |> Seq.map(fun p -> { PointF32.x = p.x * maxRadius; y = p.y * maxRadius }
                         )

    let RandRingPointsF32 (rng:Random) =
        Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()); y = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.map(fun p -> (p , (p |> PointF32Length)))
                            |> Seq.filter(fun pd -> (snd pd) < 1.0f  && (snd pd) > 0.0f )
                            |> Seq.map(fun pd -> { PointF32.x = (fst pd).x / (snd pd); y = (fst pd).y / (snd pd) }
                         )


    let RandBallPointsF32 (maxRadius:float32) (rng:Random) =
        Seq.initInfinite ( fun i -> { TripleF32.x = Convert.ToSingle(rng.NextDouble()); 
                                                y = Convert.ToSingle(rng.NextDouble()); 
                                                z = Convert.ToSingle(rng.NextDouble())  })
                            |> Seq.filter(fun p -> (p |> TripleF32LengthSquared) < 1.0f)
                            |> Seq.map(fun p -> { TripleF32.x = p.x * maxRadius; y = p.y * maxRadius; z = p.z * maxRadius; }
                         )


    let RandSpherePointsF32 (rng:Random) =
        Seq.initInfinite ( fun i -> { TripleF32.x = Convert.ToSingle(rng.NextDouble()); 
                                                y = Convert.ToSingle(rng.NextDouble()); 
                                                z = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.map(fun p -> (p , (p |> TripleF32Length)))
                            |> Seq.filter(fun pd -> (snd pd) < 1.0f  && (snd pd) > 0.0f )
                            |> Seq.map(fun pd -> { TripleF32.x = (fst pd).x / (snd pd); 
                                                             y = (fst pd).y / (snd pd); 
                                                             z = (fst pd).z / (snd pd) }
                         )
    
    let RandArray2DUF32 rowCount colCount (absMax:float32) (rng:Random) =
        Array2D.init rowCount colCount (
            fun x y -> Convert.ToSingle(rng.NextDouble()) * 2.0f - 1.0f)