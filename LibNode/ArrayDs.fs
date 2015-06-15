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

type ArrayData = 
    | BoolArray of ArrayShape * bool[]
    | IntArray of ArrayShape * IntType * int[]
    | Float32Array of ArrayShape * Float32Type * float32[]
    | ListOfBoolArray of ArrayShape * (bool[] list)
    | ListOfIntArray of ArrayShape * IntType * (int[] list)
    | ListOfFloat32Array of ArrayShape * Float32Type * (float32[] list)


module ArrayDataGen =
     
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


module ArrayDataExtr =

  let GetBoolArray (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> (ars, ba) |> Rop.succeed
    | IntArray (ars, it, ia) -> "IntArray not BoolArray" |> Rop.fail
    | Float32Array (ars, it, fa) -> "Float32Array not BoolArray" |> Rop.fail
    | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not BoolArray" |> Rop.fail
    | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not BoolArray" |> Rop.fail
    | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not BoolArray" |> Rop.fail


  let GetIntArray (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
    | IntArray (ars, it, ia) -> (it, ia) |> Rop.succeed
    | Float32Array (ars, it, fa) -> "Float32Array not IntArray" |> Rop.fail
    | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not IntArray" |> Rop.fail
    | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not IntArray" |> Rop.fail
    | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not IntArray" |> Rop.fail


  let GetFloat32Array (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
    | IntArray (ars, it, ia) -> "IntArray not Float32Array" |> Rop.fail
    | Float32Array (ars, it, fa) -> (it, fa) |> Rop.succeed
    | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not Float32Array" |> Rop.fail
    | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not Float32Array" |> Rop.fail
    | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not Float32Array" |> Rop.fail


  let GetListOfBoolArray (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
    | IntArray (ars, it, ia) -> "IntArray not ListBoolArray" |> Rop.fail
    | Float32Array (ars, it, fa) -> "Float32Array not ListBoolArray" |> Rop.fail
    | ListOfBoolArray (ars, lba) -> lba |> Rop.succeed
    | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not ListBoolArray" |> Rop.fail
    | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not ListBoolArray" |> Rop.fail


  let GetListOfIntArray (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
    | IntArray (ars, it, ia) -> "IntArray not ListOfIntArray" |> Rop.fail
    | Float32Array (ars, it, fa) -> "Float32Array not ListOfIntArray" |> Rop.fail
    | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not ListOfIntArray" |> Rop.fail
    | ListOfIntArray (ars, it, lia) -> (it, lia) |> Rop.succeed
    | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not ListOfIntArray" |> Rop.fail


  let GetListOfFloat32Array (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> "BoolArray not ListOfFloat32Array"|> Rop.fail
    | IntArray (ars, it, ia) -> "IntArray not ListOfFloat32Array" |> Rop.fail
    | Float32Array (ars, it, fa) -> "Float32Array not ListOfFloat32Array" |> Rop.fail
    | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not ListOfFloat32Array" |> Rop.fail
    | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not ListOfFloat32Array" |> Rop.fail
    | ListOfFloat32Array (ars, ft, lfa) -> (ft, lfa) |> Rop.succeed

