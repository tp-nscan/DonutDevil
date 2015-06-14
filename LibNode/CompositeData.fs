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

type Array2DSize = {rows:int; cols:int}
type Array2DIndex = {row:int; col:int}

type TwoDShape =
    | UT of Array2DSize
    | LT of Array2DSize
    | Full of Array2DSize
    | Sparse of Array2DSize * (Array2DIndex list)

type ArrayShape =
    | OneD of int
    | TwoD of TwoDShape


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

    let FullArrayShape2d (rowCount:int) (colCount:int) = 
        ArrayShape.TwoD(TwoDShape.Full {rows=rowCount; cols=colCount})

    let FullArrayCount (arrayShape:ArrayShape) = 
        match arrayShape with
        | OneD len -> len
        | TwoD td -> match td with
                     | UT ars -> ars.cols * ars.rows
                     | LT ars -> ars.cols * ars.rows
                     | Full ars -> ars.cols * ars.rows
                     | Sparse (ars,l) -> ars.cols * ars.rows