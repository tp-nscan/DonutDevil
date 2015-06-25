namespace LibNode
open System
open MathNet.Numerics
open MathNet.Numerics.Random    
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix


type ArrayShape =
    | Linear of int
    | Block of BlockSize
    | UT of BlockSize
    | Sparse of SparseArrayShape

type ArrayData = 
    | BoolArray of ArrayShape * bool[] * int[]
    | IntArray of ArrayShape * IntType * int[] * int[]
    | Float32Array of ArrayShape * Float32Type * float32[] * int[]

type ArrayDescr =
    | BoolDescr of ArrayShape
    | IntDescr of ArrayShape * IntType
    | Float32Descr of ArrayShape * Float32Type


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
        | _ -> "Not a 3-tuple or higher" |> Rop.fail


    let GetTuple3of4 tup =
        match tup with
        | (a,b,c,d) -> c |> Rop.succeed
        | _ -> "Not a 3-tuple or higher" |> Rop.fail


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
        ArrayShape.Block {rows=rowCount; cols=colCount}


    let FullArrayCount (arrayShape:ArrayShape) =
       match arrayShape with
            | Linear len -> len
            | Block ars -> ars.cols * ars.rows
            | UT ars -> ars.cols * ars.rows
            | Sparse sas -> sas.cols * sas.rows

    let MakeDenseMatrix ((arrayShape:ArrayShape),(float32Type:Float32Type),(vals:float32[]),(indexes:int[])) =
        match arrayShape with
            | Linear len -> "wrong arrayShape for DenseMatrix" |> Rop.fail
            | Block ars -> DenseMatrix.init ars.rows ars.cols (fun x y -> vals.[x+y*ars.rows]) |> Rop.succeed
            | UT ars -> "wrong arrayShape for DenseMatrix" |> Rop.fail
            | Sparse sas -> "wrong arrayShape for DenseMatrix" |> Rop.fail


module ArrayDataExtr =

  let GetArrayShape (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba, ii) -> (ars) |> Rop.succeed
    | IntArray (ars, it, ia, ii) -> (ars) |> Rop.succeed
    | Float32Array (ars, it, fa, ia) -> (ars) |> Rop.succeed

  let GetBoolArrayData (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba, ii) -> (ars, ba, ii) |> Rop.succeed
    | IntArray (ars, it, ia, ii) -> "IntArray not BoolArray" |> Rop.fail
    | Float32Array (ars, it, fa, ia) -> "Float32Array not BoolArray" |> Rop.fail

  let GetIntArrayData (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba, ii) -> "BoolArray not ListOfIntArray" |> Rop.fail
    | IntArray (ars, it, ia, ii) -> (ars, it, ia, ii) |> Rop.succeed
    | Float32Array (ars, it, fa, ia) -> "Float32Array not IntArray" |> Rop.fail

  let GetFloat32ArrayData (arrayData:ArrayData) =
    match arrayData with 
    | BoolArray (ars, ba, ii) -> "BoolArray not ListOfIntArray" |> Rop.fail
    | IntArray (ars, it, ia, ii) -> "IntArray not Float32Array" |> Rop.fail
    | Float32Array (ars, it, fa, ii) -> (ars, it, fa, ii) |> Rop.succeed

