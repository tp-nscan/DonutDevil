namespace LibNode
open System
open Rop

type EntityId = GuidId of Guid
type DataId = GuidId of Guid
type IsFresh = IsFresh of bool
type Epn = Epn of string
type EntityName = EntityName of string
type GeneratorId = { Name:string; Version:int }

 module Entvert =

    let ToEntityId (id:Guid) =
        EntityId.GuidId id

    let ToDataId (id:Guid) =
        DataId.GuidId id

    let ToEntityName (name:string) =
        EntityName(name)

    let EntityNameToString (entityName:EntityName) =
        match entityName with
        | EntityName nm -> nm

    let ToEpn (name:string) =
        Epn(name)

    let EpnToString (epn:Epn) =
        match epn with
        | Epn nm -> nm

    let ParseGeneratorId (generatorId:string) =
        try
            let pcs = generatorId.Split [|'_'|]
            {Name=pcs.[0]; Version=System.Int32.Parse pcs.[1]} |> Rop.succeed
        with
        | ex -> (sprintf "Error: %s" ex.Message) |> Rop.fail

    let GeneratorIdToString (generatorId:GeneratorId) =
        sprintf "%s_%i" generatorId.Name generatorId.Version


type DataRecord = 
    {
        DataId: DataId;
        EntityId: EntityId;
        DrData: ArrayData;
    }

type EntityData = 
    {
        ArrayDescr: ArrayDescr;
        DataId: DataId;
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


type ISym =
    inherit IEntityGen
    abstract member Update: unit -> RopResult<ISym,string>


type IEntityRepo =
    abstract member GetEntity: EntityId -> RopResult<Entity, string>
    abstract member GetData: DataId -> RopResult<DataRecord, string>
    abstract member SaveEntity: Entity -> RopResult<Entity, string>
    abstract member SaveData: DataRecord -> RopResult<DataRecord, string>