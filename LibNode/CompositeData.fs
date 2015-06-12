namespace LibNode
    open System
    open MathNet.Numerics
    open MathNet.Numerics.Random

    type Float32Type =
        | Unsigned of float32
        | Signed of float32

    type IntType =
        | Unbounded
        | Bounded of int*int
        | UnsignedBit
        | SignedBit


    module NtGens =
     
        let ToFloat32Type isUnsigned maxVal =
            match isUnsigned with
            | true -> Unsigned(maxVal)
            | false -> Signed(maxVal)

        let RandFloat32 (rng:Random) (float32Type:Float32Type) =
            match float32Type with
            | Unsigned max -> Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()) * max)
            | Signed max -> Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()-0.5) * max * 2.0f)

        let RandFloat32Seed (seed:int) (float32Type:Float32Type) =
            let rng = Random.MersenneTwister(seed)
            match float32Type with
            | Unsigned max -> Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()) * max)
            | Signed max -> Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()-0.5) * max * 2.0f)