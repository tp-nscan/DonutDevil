namespace LibNode

module Generators =

    open MathNet.Numerics
    open MathNet.Numerics.Distributions
    open MathNet.Numerics.LinearAlgebra
    open MathNet.Numerics.LinearAlgebra.Matrix
    open MathNet.Numerics.Random
    open MathUtils
    open System
    
    let rng = Random.mersenneTwisterShared

    // selects a random float32 from (-max, max)
    let RandUnsignedIntervalF32 (max:float32) =
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()) * max)

        
    let RandUsFloat32 (max:float32) =
        Convert.ToSingle(rng.NextDouble() * 2.0) * max - max
        
    let RandBools =
        Seq.initInfinite ( fun i -> rng.Next(2)=1)

    let RandSignedIntervalF32 (max:float32) =
        Seq.initInfinite ( fun i -> RandUsFloat32 max )


    let RandBitsUF32  =
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.Next(2)))

    let RandBitsF32 =
        Seq.initInfinite ( fun i -> Convert.ToSingle(rng.Next(2)) * TwoF32 - OneF32)

    let FlipUF32WithProb (probability:float) (value:float32) =
        if (rng.NextDouble() < probability) then
            OneF32 - value
        else
            value


    let FlipF32WithProb (probability:float) (value:float32) =
        if (rng.NextDouble() < probability) then
            value * NOneF32
        else
            value


    let PerturbInRangeF32 (minVal:float32) (maxVal:float32) (maxDelta:float32) (value:float32) =
        let newVal = value + (RandUsFloat32 maxDelta)
        if newVal < minVal then
            minVal
        else if newVal > maxVal then
            maxVal
        else
            newVal


    let PerturbInRangeF32A (minVal:float32) (maxVal:float32) (maxDelta:float32) (values:float32[]) =
        values |> Array.map(fun v -> (PerturbInRangeF32 minVal maxVal maxDelta v))


    let FlipUF32A (mutationRate:float32) (values:float32[]) =
        let dblM = Convert.ToDouble mutationRate
        values |> Array.map(fun v -> (FlipUF32WithProb dblM v ))


    let FlipF32A (mutationRate:float32) (values:float32[]) =
        let dblM = Convert.ToDouble mutationRate
        values |> Array.map(fun v -> (FlipF32WithProb dblM v ))


    let RandomIntervalF32 (unsigned:bool) (max:float32) =
        if unsigned then
            RandUnsignedIntervalF32 max
        else
            RandSignedIntervalF32 max


    let RandomBitsF32 (unsigned:bool) =
        if unsigned then
            RandBitsUF32
        else
            RandBitsF32


    let RandomFloat32 (justBits:bool) (unsigned:bool) (max:float32) =
        if justBits then
            RandomIntervalF32 unsigned max
        else
            RandomBitsF32 unsigned


    let RandomEuclideanPointsF32 (unsigned:bool) =
        if unsigned then
            Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble() ); y = Convert.ToSingle(rng.NextDouble()) })
        else
            Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()) * TwoF32 - OneF32; y = Convert.ToSingle(rng.NextDouble())* TwoF32 - OneF32 })


    let RandDiscPointsF32 (maxRadius:float32) =
        Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()); y = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.filter(fun p -> (p |> PointF32LengthSquared) < OneF32)
                            |> Seq.map(fun p -> { PointF32.x = p.x * maxRadius; y = p.y * maxRadius }
                         )

    let RandRingPointsF32 =
        Seq.initInfinite ( fun i -> { PointF32.x = Convert.ToSingle(rng.NextDouble()); y = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.map(fun p -> (p , (p |> PointF32Length)))
                            |> Seq.filter(fun pd -> (snd pd) < OneF32  && (snd pd) > ZeroF32 )
                            |> Seq.map(fun pd -> { PointF32.x = (fst pd).x / (snd pd); y = (fst pd).y / (snd pd) }
                         )


    let RandBallPointsF32 (maxRadius:float32) =
        Seq.initInfinite ( fun i -> { TripleF32.x = Convert.ToSingle(rng.NextDouble()); y = Convert.ToSingle(rng.NextDouble()); z = Convert.ToSingle(rng.NextDouble())  })
                            |> Seq.filter(fun p -> (p |> TripleF32LengthSquared) < OneF32)
                            |> Seq.map(fun p -> { TripleF32.x = p.x * maxRadius; y = p.y * maxRadius; z = p.z * maxRadius; }
                         )


    let RandSpherePointsF32 =
        Seq.initInfinite ( fun i -> { TripleF32.x = Convert.ToSingle(rng.NextDouble()); y = Convert.ToSingle(rng.NextDouble()); z = Convert.ToSingle(rng.NextDouble()) })
                            |> Seq.map(fun p -> (p , (p |> TripleF32Length)))
                            |> Seq.filter(fun pd -> (snd pd) < OneF32  && (snd pd) > ZeroF32 )
                            |> Seq.map(fun pd -> { TripleF32.x = (fst pd).x / (snd pd); y = (fst pd).y / (snd pd); z = (fst pd).z / (snd pd) }
                         )
    
    let RandArray2DUF32 rowCount colCount (absMax:float32) =
        Array2D.init rowCount colCount (fun x y -> Convert.ToSingle(rng.NextDouble()) * TwoF32 - OneF32)