namespace LibNode

open System
open Rop

type EntityId = GuidId of Guid
type DataId = GuidId of Guid


///EntityPartName
type Epn = Epn of string

///EntityPartName
type EntityName = EntityName of string


 module EpnConvert =
    let FromString (name:string) =
        Epn(name)
//EntityGenerator name


type GeneratorId = { Name:string; Version:int }


type IsFresh = IsFresh of bool


type DataRecord = 
    {
        DataId: DataId;
        EntityId: EntityId;
        Epn: Epn;
        ArrayData: ArrayData;
    }


type EntityData = 
    {
        DataId: DataId;
        EntityId: EntityId;
        Epn: Epn;
    }


type GenResult = 
    {
        Epn: Epn;
        ArrayData: ArrayData;
    }


type Entity =
    {
        Name:EntityName;
        EntityId: EntityId;
        GeneratorId: GeneratorId;
        Params: Param list;
        Iteration: int;
        SourceData: EntityData list;
        ResultData: EntityData list; 
    }


type IEntityGen =
    abstract member GeneratorId: GeneratorId
    abstract member Iteration: int
    abstract member SourceData: EntityData list
    abstract member Params: Param list
    abstract member GenResultStates: (IsFresh * Epn) list
    abstract member GetGenResult: Epn -> RopResult<GenResult, string>


type IIterativeEntityGen =
    inherit IEntityGen
    abstract member Update: unit -> RopResult<string,string>


type IEntityRepo =
    abstract member GetEntity: EntityId -> RopResult<Entity, string>
    abstract member GetData: DataId -> RopResult<DataRecord, string>
    abstract member SaveEntity: Entity -> RopResult<Entity, string>
    abstract member SaveData: DataRecord -> RopResult<DataRecord, string>