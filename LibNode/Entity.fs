namespace LibNode

open System
open System.Collections.Generic
open MathUtils
open Generators
open NodeGroupBuilders
open Rop

///EntityPartName
type Epn = Epn of string
//EntityGenerator name
type EgName = { Name:string; Version:int }

type EntityId = EntityId of Guid


type DataChunkName = 
    | BoolArray of Epn
    | IntArray of Epn * IntType
    | Float32Array of Epn * Float32Type
    | ListOfBoolArray of Epn
    | ListOfIntArray of Epn * IntType
    | ListOfFloat32Array of Epn * Float32Type


type DataChunk = 
    | BoolArray of Guid * Epn
    | IntArray of Guid * Epn * IntType
    | Float32Array of Guid * Epn * Float32Type
    | ListOfBoolArray of Guid * Epn
    | ListOfIntArray of Guid * Epn * IntType
    | ListOfFloat32Array of Guid * Epn * Float32Type


type DataChunkValue = 
    | BoolArray of Guid * Epn * bool[]
    | IntArray of Guid * Epn * IntType * int[]
    | Float32Array of Guid * Epn * Float32Type * float32[]
    | ListOfBoolArray of Guid * Epn * (bool[] list)
    | ListOfIntArray of Guid * Epn * IntType * (int[] list)
    | ListOfFloat32Array of Guid * Epn * Float32Type * (float32[] list)


type IEntity =
    abstract member EntityId: EntityId
    abstract member EgName: EgName
    abstract member SourceData: (EntityId*Epn*Epn) List
    abstract member Params: Param List
    abstract member Iteration: int
    abstract member DataChunks: DataChunk List

type IEntityGen =
    abstract member EgName: EgName
    abstract member SourceData: (EntityId*Epn*Epn) List
    abstract member Params: Param List
    abstract member DataChunkNames: DataChunkName List
    abstract member GetDataChunkValue: Epn -> RopResult<DataChunkValue, string>

type IIterativeEntityGen =
    inherit IEntityGen
    abstract member Iteration: int
    abstract member Update: unit -> RopResult<string,string>


type RandMatrixDto = { id:Guid; rowCount:int; colCount:int; 
                    seed:int; unsigned:bool; maxValue:float32 }

type RandMatrixGenerator(prams:Param List, rowCount:int, colCount:int, seed:int, 
                         float32Type:Float32Type) =
    let _sourceData = new List<EntityId*Epn*Epn>()
    let _params = prams
    let _seed = seed
    let _float32Type = float32Type
    let _dataChunkNames = new ResizeArray<DataChunkName>([DataChunkName.ListOfFloat32Array(Epn("Matrix"), _float32Type);] |> Seq.cast)

    interface IEntityGen with
        member this.EgName =
            {Name="RandMatrixGenerator"; Version=1}
        member this.SourceData =
            _sourceData
        member this.Params = 
            _params
        member this.DataChunkNames = 
            _dataChunkNames
        member this.GetDataChunkValue(epn:Epn) = 
            match epn with
            | Epn("Matrix") -> DataChunkValue.Float32Array(
                                    Guid.NewGuid(), 
                                    Epn("Matrix"), 
                                    _float32Type, 
                                    (NtGens.RandFloat32Seed _seed float32Type) 
                                        |> Seq.take(rowCount*colCount) 
                                        |> Seq.toArray
                                ) |> Rop.succeed
            | Epn(s) -> sprintf "DataChunkName %s not found" s |> Rop.fail


