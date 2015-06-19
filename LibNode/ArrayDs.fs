namespace LibNode
open System
open MathNet.Numerics
open MathNet.Numerics.Random    
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix


type TwoDShape =
    | UT of BlockSize
    | LT of BlockSize
    | Full of BlockSize
    | Sparse of BlockSize * (Array2DIndex list)

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

type ArrayDescr =
    | Bool of BlockSize
    | Int of IntType*BlockSize
    | Float32 of Float32Type*BlockSize



module ArrayDataGen =
     
    let Float32TypeDescr (float32Type:Float32Type) =
        match float32Type with
        | UF m -> sprintf "(0, %f) float" m
        | SF m -> sprintf "(-%f, %f) float" m m


    let IntTypeDescr (intType:IntType) =
        match intType with
        | UI m -> sprintf "(0, %i) int" m
        | SI m -> sprintf "(-%i, %i) int" m m


    let GetTuple3of3 tup =
        match tup with
        | (a,b,c) -> c |> Rop.succeed
        | _ -> "Not a 3-tuple" |> Rop.fail

    let ToFloat32Type isUnsigned maxVal =
        match isUnsigned with
        | true -> UF(maxVal)
        | false -> SF(maxVal)

    let RandFloat32 (rng:Random) (float32Type:Float32Type) =
        match float32Type with
        | UF max -> Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()) * max)
        | SF max -> Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble() - 0.5) * max * 2.0f)

    let RandFloat32Seed (seed:int) (float32Type:Float32Type) =
        let rng = Random.MersenneTwister(seed)
        match float32Type with
        | UF max -> Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble()) * max)
        | SF max -> Seq.initInfinite ( fun i -> Convert.ToSingle(rng.NextDouble() - 0.5) * max * 2.0f)

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

    let MakeDenseMatrix ((arrayShape:ArrayShape),(float32Type:Float32Type),(vals:float32[])) =
        match arrayShape with
        | OneD _ -> "wrong arrayShape for DenseMatrix" |> Rop.fail
        | TwoD tds -> match tds with
                      | UT ars -> "wrong arrayShape for DenseMatrix" |> Rop.fail
                      | LT ars -> "wrong arrayShape for DenseMatrix" |> Rop.fail
                      | Full ars -> DenseMatrix.init ars.rows ars.cols (fun x y -> vals.[x+y*ars.rows]) |> Rop.succeed
                      | Sparse (ars,l) -> "wrong arrayShape for DenseMatrix" |> Rop.fail




module ArrayDataExtr =

  let GetArrayShape (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> (ars) |> Rop.succeed
    | IntArray (ars, it, ia) -> (ars) |> Rop.succeed
    | Float32Array (ars, it, fa) -> (ars) |> Rop.succeed
    | ListOfBoolArray (ars, lba) -> (ars) |> Rop.succeed
    | ListOfIntArray (ars, it, lia) -> (ars) |> Rop.succeed
    | ListOfFloat32Array (ars, ft, lfa) -> (ars) |> Rop.succeed

  let GetBoolArrayData (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> (ars, ba) |> Rop.succeed
    | IntArray (ars, it, ia) -> "IntArray not BoolArray" |> Rop.fail
    | Float32Array (ars, it, fa) -> "Float32Array not BoolArray" |> Rop.fail
    | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not BoolArray" |> Rop.fail
    | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not BoolArray" |> Rop.fail
    | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not BoolArray" |> Rop.fail


  let GetIntArrayData (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
    | IntArray (ars, it, ia) -> (ars, it, ia) |> Rop.succeed
    | Float32Array (ars, it, fa) -> "Float32Array not IntArray" |> Rop.fail
    | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not IntArray" |> Rop.fail
    | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not IntArray" |> Rop.fail
    | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not IntArray" |> Rop.fail


  let GetFloat32ArrayData (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
    | IntArray (ars, it, ia) -> "IntArray not Float32Array" |> Rop.fail
    | Float32Array (ars, it, fa) -> (ars, it, fa) |> Rop.succeed
    | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not Float32Array" |> Rop.fail
    | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not Float32Array" |> Rop.fail
    | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not Float32Array" |> Rop.fail


  let GetListOfBoolArrayData (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
    | IntArray (ars, it, ia) -> "IntArray not ListBoolArray" |> Rop.fail
    | Float32Array (ars, it, fa) -> "Float32Array not ListBoolArray" |> Rop.fail
    | ListOfBoolArray (ars, lba) -> (ars, lba) |> Rop.succeed
    | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not ListBoolArray" |> Rop.fail
    | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not ListBoolArray" |> Rop.fail


  let GetListOfIntArrayData (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
    | IntArray (ars, it, ia) -> "IntArray not ListOfIntArray" |> Rop.fail
    | Float32Array (ars, it, fa) -> "Float32Array not ListOfIntArray" |> Rop.fail
    | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not ListOfIntArray" |> Rop.fail
    | ListOfIntArray (ars, it, lia) -> (ars, it, lia) |> Rop.succeed
    | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not ListOfIntArray" |> Rop.fail


  let GetListOfFloat32ArrayData (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba) -> "BoolArray not ListOfFloat32Array"|> Rop.fail
    | IntArray (ars, it, ia) -> "IntArray not ListOfFloat32Array" |> Rop.fail
    | Float32Array (ars, it, fa) -> "Float32Array not ListOfFloat32Array" |> Rop.fail
    | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not ListOfFloat32Array" |> Rop.fail
    | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not ListOfFloat32Array" |> Rop.fail
    | ListOfFloat32Array (ars, ft, lfa) -> (ars, ft, lfa) |> Rop.succeed

