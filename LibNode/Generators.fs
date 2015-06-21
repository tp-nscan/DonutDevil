namespace LibNode

module Generators =
    open System
    open MathNet.Numerics
    open MathNet.Numerics.Random
    open LibNode.MathUtils
    
    let sharedRng = Random.mersenneTwisterShared

    let SeqIter (s:seq<float32>) =
        let iter = s.GetEnumerator()
        function () ->
                    iter.MoveNext() |> ignore
                    iter.Current

    let RandomName (prefix:string) = 
        let rng = Random.MersenneTwister()
        let suffix = rng.Next()
        sprintf "%s_%i" prefix suffix

     // a random float32 from (-max, max)
    let RandSignedFloat32 (rng:Random) (max:float32) =
        Convert.ToSingle(rng.NextDouble() * 2.0) * max - max

    // a sequence of random float32 from (-max, max)
    let RandF32 (rng:Random) (max:float32) =
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()) * max)

    // a sequence of random float32 from (-max, max)
    let RandSF32 (rng:Random) (max:float32) =
        Seq.initInfinite ( fun i -> RandSignedFloat32 rng max )

    // a sequence of random float32 from (0, max)
    let RandF32s (seed:int) (max:float32) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> RandSignedFloat32 rng max )

    // a sequence of random float32 from (-max, max)
    let RandUF32s (seed:int) (max:float32) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()) * max)

    let RandBools (seed:int) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> rng.Next(2)=1)

     // random draws from {0f, 1f}
    let RandUF32Bits (seed:int) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.Next(2)))

     // random draws from {-1f, 1f}
    let RandF32Bits (seed:int) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.Next(2)) * 2.0f - 1.0f)

    // 0f <=> 1f with probability p
    let FlipUF32WithProb (rng:Random) (p:float) (value:float32) =
        if (rng.NextDouble() < p) then
            1.0f - value
        else
            value

    /// -1f <=> 1f with probability p
    let FlipF32WithProb (rng:Random) (probability:float) (value:float32) =
        if (rng.NextDouble() < probability) then
            value * -1.0f
        else
            value
    
    /// applies FlipUF32WithProb to values:float32[] 
    let FlipUF32A (seed:int) (p:float32) (values:float32[]) =
        let rng = Random.MersenneTwister(seed)
        let dblM = Convert.ToDouble p
        values |> Array.map(fun v -> (FlipUF32WithProb rng dblM v ))

    /// applies FlipF32WithProb to values:float32[] 
    let FlipF32A (seed:int) (p:float32) (values:float32[]) =
        let rng = Random.MersenneTwister(seed)
        let dblM = Convert.ToDouble p
        values |> Array.map(fun v -> (FlipF32WithProb rng dblM v ))


    // one step of random walk in [minVal, maxVal] with stepsize uniformly picked from (0, maxDelta)
    let PerturbInRangeF32 (rng:Random) (minVal:float32) (maxVal:float32) (maxDelta:float32) (value:float32) =
        let newVal = value + (RandSignedFloat32 rng maxDelta)
        if newVal < minVal then
            minVal
        else if newVal > maxVal then
            maxVal
        else
            newVal

    // applies PerturbInRangeF32 to values:float32[] 
    let PerturbInRangeF32A (seed:int) (minVal:float32) (maxVal:float32) (maxDelta:float32) (values:float32[]) =
        let rng = Random.MersenneTwister(seed)
        values |> Array.map(fun v -> (PerturbInRangeF32 rng minVal maxVal maxDelta v))


    let RandFloatsF32 (seed:int) (unsigned:bool) (max:float32) =
        if unsigned then
            RandUF32s seed max
        else
            RandF32s seed max


    let RandomBitsF32 (seed:int) (unsigned:bool) =
        if unsigned then
            RandUF32Bits seed
        else
            RandF32Bits seed


    let RandomFloat32 (seed:int) (justBits:bool) (unsigned:bool) (max:float32) =
        if justBits then
            RandFloatsF32 seed unsigned max
        else
            RandomBitsF32 seed unsigned


    let RandomEuclideanPointsF32 (seed:int) (unsigned:bool) =
        let rng = Random.MersenneTwister(seed)
        if unsigned then
            Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble() ); 
                                                   y = Convert.ToSingle(rng.NextDouble()) })
        else
            Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()) * 2.0f - 1.0f; 
                                                   y = Convert.ToSingle(rng.NextDouble()) * 2.0f - 1.0f })


    let RandDiscPointsF32 (seed:int) (maxRadius:float32) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()); 
                                               y = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.filter(fun p -> (p |> PointF32LengthSquared) < 1.0f)
                            |> Seq.map(fun p -> { PointF32.x = p.x * maxRadius; y = p.y * maxRadius }
                         )

    let RandRingPointsF32 (seed:int) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()); y = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.map(fun p -> (p , (p |> PointF32Length)))
                            |> Seq.filter(fun pd -> (snd pd) < 1.0f  && (snd pd) > 0.0f )
                            |> Seq.map(fun pd -> { PointF32.x = (fst pd).x / (snd pd); y = (fst pd).y / (snd pd) }
                         )


    let RandBallPointsF32 (seed:int) (maxRadius:float32) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> { TripleF32.x = Convert.ToSingle(rng.NextDouble()); 
                                                y = Convert.ToSingle(rng.NextDouble()); 
                                                z = Convert.ToSingle(rng.NextDouble())  })
                            |> Seq.filter(fun p -> (p |> TripleF32LengthSquared) < 1.0f)
                            |> Seq.map(fun p -> { TripleF32.x = p.x * maxRadius; y = p.y * maxRadius; z = p.z * maxRadius; }
                         )


    let RandSpherePointsF32 (seed:int) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> { TripleF32.x = Convert.ToSingle(rng.NextDouble()); 
                                                y = Convert.ToSingle(rng.NextDouble()); 
                                                z = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.map(fun p -> (p , (p |> TripleF32Length)))
                            |> Seq.filter(fun pd -> (snd pd) < 1.0f  && (snd pd) > 0.0f )
                            |> Seq.map(fun pd -> { TripleF32.x = (fst pd).x / (snd pd); 
                                                             y = (fst pd).y / (snd pd); 
                                                             z = (fst pd).z / (snd pd) }
                         )
    
    let RandArray2DUF32 (seed:int) rowCount colCount (absMax:float32) =
        let rng = Random.MersenneTwister(seed)
        Array2D.init rowCount colCount (fun x y -> Convert.ToSingle(rng.NextDouble()) * 2.0f - 1.0f)