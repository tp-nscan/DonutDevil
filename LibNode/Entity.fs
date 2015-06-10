namespace LibNode

open System
open System.Collections.Generic
open MathUtils
open Generators
open NodeGroupBuilders
open Rop

type EntityId = EntityId of Guid
type DataId = DataId of Guid


///EntityPartName
type Epn = Epn of string

 module EpnConvert =
    let FromString (name:string) =
        Epn(name)
//EntityGenerator name

type GeneratorId = { Name:string; Version:int }

type IsFresh = IsFresh of bool

type EntityData = 
    | BoolArray of DataId * Epn
    | IntArray of DataId * Epn * IntType
    | Float32Array of DataId * Epn * Float32Type
    | ListOfBoolArray of DataId * Epn
    | ListOfIntArray of DataId * Epn * IntType
    | ListOfFloat32Array of DataId * Epn * Float32Type


type ArrayData = 
    | BoolArray of bool[]
    | IntArray of IntType * int[]
    | Float32Array of Float32Type * float32[]
    | ListOfBoolArray of (bool[] list)
    | ListOfIntArray of IntType * (int[] list)
    | ListOfFloat32Array of Float32Type * (float32[] list)

type DataRecord = 
    {
        DataId: DataId;
        EntityId: EntityId;
        Epn: Epn;
        ArrayData: ArrayData;
    }

type GenResult = 
    {
        Epn: Epn;
        ArrayData: ArrayData;
    }

type Entity =
    {
        EntityId: EntityId;
        GeneratorId: GeneratorId;
        Params: Param list;
        Iteration: int;
        SourceData: EntityData list;
        ResultData: EntityData list; 
    }

type IEntityGen =
    abstract member GeneratorId: GeneratorId
    abstract member SourceData: EntityData list
    abstract member Params: Param list
    abstract member GenResultStates: (IsFresh * Epn) list
    abstract member GetGenResult: Epn -> RopResult<GenResult, string>


type IIterativeEntityGen =
    inherit IEntityGen
    abstract member Iteration: int
    abstract member Update: unit -> RopResult<string,string>


type IEntityRepo =
    abstract member GetEntity: EntityId -> Entity
    abstract member GetDataRecord: DataId -> DataRecord