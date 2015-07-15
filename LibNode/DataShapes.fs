namespace LibNode
open System
open Rop


type FloatCover =
     | Continuous
     | Discrete

type FloatRange =
     | Signed
     | Unsigned

/// UF: [0, v], SF: [-v, v], UB: [0.0f, 1.0f], SB: [-1.0f, 1.0f]
type Float32Type =
    | UF of float32
    | SF of float32
    | UB
    | SB

type IntType =
    | UI of int
    | SI of int


type BlockSize = {rows:int; cols:int}
type SquareSize = SquareSize of int
type Array2DIndex = {row:int; col:int}
type SparseArrayShape = {rows:int; cols:int; nonZeroCount:int}


type DataShape =
    | Linear of int
    | Block of BlockSize
    | UT of SquareSize
    | Sparse of SparseArrayShape


type NumericType<'a> =
    | U of 'a
    | S of 'a


type DataType =
    | Bool
    | Int of NumericType<int>
    | Float32 of NumericType<float32>


type DataBlock =
    { dataShape:DataShape; dataType:DataType; data:byte[] }


module DataShapeProcs =
    let SquareSizeToInt sqs =
        match sqs with
        | SquareSize v -> v


    let UnsignedToFloatRange (u:bool) =
        match u with
        | true -> FloatRange.Unsigned
        | false -> FloatRange.Signed

    let DiscreteToFloatCover (u:bool) =
        match u with
        | true -> FloatCover.Discrete
        | false -> FloatCover.Continuous

    let GetDataTypeLength (dataType:DataType) = 
        match  (dataType) with
        | Bool -> 1
        | Int _ -> 4
        | Float32 _ -> 4


    let GetDataBlockLength (dataShape:DataShape) (dataType:DataType) = 
        match (dataShape, dataType) with
        | (DataShape.Linear l, dt) -> l * (GetDataTypeLength dt)
        | (DataShape.Block bs, dt) ->  bs.rows * bs.cols * (GetDataTypeLength dt)
        | (DataShape.UT sqs, dt) -> ((SquareSizeToInt sqs) * ((SquareSizeToInt sqs) + 1) * (GetDataTypeLength dt) ) / 2
        | (DataShape.Sparse sas, dt) -> sas.nonZeroCount * ((GetDataTypeLength dt) + 8)

    
    let CopyToFloatArray (dataShape:DataShape, dataType:DataType, data:byte[]) =
        match  (dataType, data) with
        | (DataType.Bool, d) -> "wrong datatype: Bool" |> Rop.fail
        | (DataType.Int nt, d) -> "wrong datatype: Bool" |> Rop.fail
        | (DataType.Float32 ft, d) -> 

                let result = Array.create 6 0.0f
                Buffer.BlockCopy(data, 0, result, 0, GetDataBlockLength dataShape dataType) |> Rop.succeed 

    let GetFloatArrayFromDataBlock(dataBlock:DataBlock) =
        match dataBlock with
        | {dataShape= DataShape.Linear l; dataType=dt; data=data } -> l * (GetDataTypeLength dt)
        | {dataShape= DataShape.Block bs; dataType=dt; data=data } ->  bs.rows * bs.cols * (GetDataTypeLength dt)
        | {dataShape= DataShape.UT sqs; dataType=dt; data=data } -> ((SquareSizeToInt sqs) * ((SquareSizeToInt sqs)+ 1) * (GetDataTypeLength dt) ) / 2
        | {dataShape= DataShape.Sparse sas; dataType=dt; data=data } -> sas.nonZeroCount * ((GetDataTypeLength dt) + 8)