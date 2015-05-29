namespace LibNode

module Generators =

    open System
    open MathNet.Numerics
    open MathNet.Numerics.Distributions
    open MathNet.Numerics.LinearAlgebra
    open MathNet.Numerics.LinearAlgebra.Matrix
    open MathNet.Numerics.Random
    open LibNode.Dict
    open LibNode.MathUtils
    
    let sharedRng = Random.mersenneTwisterShared

    // selects a random float32 from (-max, max)
    let RandUnsignedFloatsF32 (seed:int) (max:float32) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()) * max)

    let RandSignedFloat32 (rng:Random) (max:float32) =
        Convert.ToSingle(rng.NextDouble() * 2.0) * max - max
        
    let RandBools (seed:int) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> rng.Next(2)=1)

    let RandSignedFloatsF32 (seed:int) (max:float32) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> RandSignedFloat32 rng max )

    let RandBitsUF32 (seed:int) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.Next(2)))

    let RandBitsF32 (seed:int) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.Next(2)) * TwoF32 - OneF32)

    let FlipUF32WithProb (rng:Random) (probability:float) (value:float32) =
        if (rng.NextDouble() < probability) then
            OneF32 - value
        else
            value

    let FlipF32WithProb (rng:Random) (probability:float) (value:float32) =
        if (rng.NextDouble() < probability) then
            value * NOneF32
        else
            value

    let PerturbInRangeF32 (rng:Random) (minVal:float32) (maxVal:float32) (maxDelta:float32) (value:float32) =
        let newVal = value + (RandSignedFloat32 rng maxDelta)
        if newVal < minVal then
            minVal
        else if newVal > maxVal then
            maxVal
        else
            newVal

    let PerturbInRangeF32A (seed:int) (minVal:float32) (maxVal:float32) (maxDelta:float32) (values:float32[]) =
        let rng = Random.MersenneTwister(seed)
        values |> Array.map(fun v -> (PerturbInRangeF32 rng minVal maxVal maxDelta v))


    let FlipUF32A (seed:int) (mutationRate:float32) (values:float32[]) =
        let rng = Random.MersenneTwister(seed)
        let dblM = Convert.ToDouble mutationRate
        values |> Array.map(fun v -> (FlipUF32WithProb rng dblM v ))


    let FlipF32A (seed:int) (mutationRate:float32) (values:float32[]) =
        let rng = Random.MersenneTwister(seed)
        let dblM = Convert.ToDouble mutationRate
        values |> Array.map(fun v -> (FlipF32WithProb rng dblM v ))


    let RandFloatsF32 (seed:int) (unsigned:bool) (max:float32) =
        if unsigned then
            RandUnsignedFloatsF32 seed max
        else
            RandSignedFloatsF32 seed max


    let RandomBitsF32 (seed:int) (unsigned:bool) =
        if unsigned then
            RandBitsUF32 seed
        else
            RandBitsF32 seed


    let RandomFloat32 (seed:int) (justBits:bool) (unsigned:bool) (max:float32) =
        if justBits then
            RandFloatsF32 seed unsigned max
        else
            RandomBitsF32 seed unsigned


    let RandomEuclideanPointsF32 (seed:int) (unsigned:bool) =
        let rng = Random.MersenneTwister(seed)
        if unsigned then
            Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble() ); y = Convert.ToSingle(rng.NextDouble()) })
        else
            Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()) * TwoF32 - OneF32; y = Convert.ToSingle(rng.NextDouble())* TwoF32 - OneF32 })


    let RandDiscPointsF32 (seed:int) (maxRadius:float32) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()); y = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.filter(fun p -> (p |> PointF32LengthSquared) < OneF32)
                            |> Seq.map(fun p -> { PointF32.x = p.x * maxRadius; y = p.y * maxRadius }
                         )

    let RandRingPointsF32 (seed:int) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()); y = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.map(fun p -> (p , (p |> PointF32Length)))
                            |> Seq.filter(fun pd -> (snd pd) < OneF32  && (snd pd) > ZeroF32 )
                            |> Seq.map(fun pd -> { PointF32.x = (fst pd).x / (snd pd); y = (fst pd).y / (snd pd) }
                         )


    let RandBallPointsF32 (seed:int) (maxRadius:float32) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> { TripleF32.x = Convert.ToSingle(rng.NextDouble()); y = Convert.ToSingle(rng.NextDouble()); z = Convert.ToSingle(rng.NextDouble())  })
                            |> Seq.filter(fun p -> (p |> TripleF32LengthSquared) < OneF32)
                            |> Seq.map(fun p -> { TripleF32.x = p.x * maxRadius; y = p.y * maxRadius; z = p.z * maxRadius; }
                         )


    let RandSpherePointsF32 (seed:int) =
        let rng = Random.MersenneTwister(seed)
        Seq.initInfinite ( fun i -> { TripleF32.x = Convert.ToSingle(rng.NextDouble()); y = Convert.ToSingle(rng.NextDouble()); z = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.map(fun p -> (p , (p |> TripleF32Length)))
                            |> Seq.filter(fun pd -> (snd pd) < OneF32  && (snd pd) > ZeroF32 )
                            |> Seq.map(fun pd -> { TripleF32.x = (fst pd).x / (snd pd); y = (fst pd).y / (snd pd); z = (fst pd).z / (snd pd) }
                         )
    
    let RandArray2DUF32 (seed:int) rowCount colCount (absMax:float32) =
        let rng = Random.MersenneTwister(seed)
        Array2D.init rowCount colCount (fun x y -> Convert.ToSingle(rng.NextDouble()) * TwoF32 - OneF32)