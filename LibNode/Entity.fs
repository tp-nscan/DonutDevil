namespace LibNode

open System
open Rop

type EntityId = GuidId of Guid
type DataId = GuidId of Guid


///EntityPartName
type Epn = Epn of string

 module EpnConvert =
    let FromString (name:string) =
        Epn(name)
//EntityGenerator name

type GeneratorId = { Name:string; Version:int }

type IsFresh = IsFresh of bool

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

type EntityData = 
    {
        DataId: DataId;
        EntityId: EntityId;
        Epn: Epn;
    }

type GenResult = 
    {
        EntityId: EntityId;
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
    abstract member EntityId: EntityId
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
    abstract member SaveData: GenResult -> RopResult<DataRecord, string>

module EntityOps =

    let EntityString eg =
        match eg.EntityId with
        | EntityId.GuidId g -> g.ToString()

    let DataString dg =
        match dg.DataId with
        | DataId.GuidId g -> g.ToString()

    let ToDataRecord (genResult:GenResult) =
        {
            DataRecord.DataId = GuidId(Guid.NewGuid());
            EntityId = genResult.EntityId;
            Epn = genResult.Epn;
            ArrayData = genResult.ArrayData;
        }

    let ToEntityData (dataRecord:DataRecord) =
        {
            EntityData.DataId = dataRecord.DataId
            EntityId = dataRecord.EntityId;
            Epn = dataRecord.Epn;
        }

    let MakeEntityData (entityGen:IEntityGen) =
        entityGen.GenResultStates 
            |> List.map(fun r -> entityGen.GetGenResult(snd r))
            |> MergeResultList
    

    let PushThroughRepo (entityRepo:IEntityRepo) (genResults:GenResult list) =
        try
            genResults |> List.map(fun gr-> entityRepo.SaveData gr)
                       |> MergeResultList
        with
        | ex -> (sprintf "Error saving GenResults: %s" ex.Message) |> Rop.fail
                

    let SaveEntity (entityRepo:IEntityRepo) (entity:Entity) =
        try
            entityRepo.SaveEntity entity
        with
        | ex -> (sprintf "Error saving Entity: %s" (EntityString entity)) |> Rop.fail


    let MakeResultData (entityRepo:IEntityRepo) (entityGen:IEntityGen) =
        match MakeEntityData entityGen with
              | Success (resultList, msgs) -> 
                    (PushThroughRepo entityRepo resultList)
              | Failure errors -> Failure errors
      
    let MakeEntityFromGen (entityRepo:IEntityRepo) (entityGen:IEntityGen) =
        match (MakeResultData entityRepo entityGen) with
        | Success (resultData, msgs) -> 
                            {
                                Entity.EntityId = entityGen.EntityId;
                                GeneratorId = entityGen.GeneratorId;
                                Params = entityGen.Params;
                                Iteration = entityGen.Iteration;
                                SourceData = entityGen.SourceData;
                                ResultData = resultData
                                    |> List.map(fun dr -> dr |> ToEntityData)
                            } |> Rop.succeed
        | Failure errors -> Failure errors

    let SaveEntityFromGen (entityRepo:IEntityRepo) (entityGen:IEntityGen) =
        match (MakeEntityFromGen entityRepo entityGen) with
            | Success (resultEntity, msgs) -> 
                               SaveEntity entityRepo resultEntity
            | Failure errors -> Failure errors