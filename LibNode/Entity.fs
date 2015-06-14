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

type ArrayData = 
    | BoolArray of ArrayShape * bool[]
    | IntArray of ArrayShape * IntType * int[]
    | Float32Array of ArrayShape * Float32Type * float32[]
    | ListOfBoolArray of ArrayShape * (bool[] list)
    | ListOfIntArray of ArrayShape * IntType * (int[] list)
    | ListOfFloat32Array of ArrayShape * Float32Type * (float32[] list)

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
        Name:EntityName;
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

    let EpnString epn =
        match epn with
        | Epn s -> s

    let EntityString eg =
        match eg.EntityId with
        | EntityId.GuidId g -> g.ToString()

    let DataString dg =
        match dg.DataId with
        | DataId.GuidId g -> g.ToString()


    let GetBoolArray (entityRepo:IEntityRepo) (dataId:DataId) =
        match entityRepo.GetData dataId with
        | Success (entityData, msgs) -> 
            match entityData.ArrayData with 
            | BoolArray (ars, ba) -> (ars, ba) |> Rop.succeed
            | IntArray (ars, it, ia) -> "IntArray not BoolArray" |> Rop.fail
            | Float32Array (ars, it, fa) -> "Float32Array not BoolArray" |> Rop.fail
            | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not BoolArray" |> Rop.fail
            | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not BoolArray" |> Rop.fail
            | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not BoolArray" |> Rop.fail
        | Failure errors -> Failure errors


    let GetIntArray (entityRepo:IEntityRepo) (dataId:DataId) =
        match entityRepo.GetData dataId with
        | Success (entityData, msgs) -> 
            match entityData.ArrayData with 
            | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
            | IntArray (ars, it, ia) -> (it, ia) |> Rop.succeed
            | Float32Array (ars, it, fa) -> "Float32Array not IntArray" |> Rop.fail
            | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not IntArray" |> Rop.fail
            | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not IntArray" |> Rop.fail
            | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not IntArray" |> Rop.fail
        | Failure errors -> Failure errors


    let GetFloat32Array (entityRepo:IEntityRepo) (dataId:DataId) =
        match entityRepo.GetData dataId with
        | Success (entityData, msgs) -> 
            match entityData.ArrayData with 
            | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
            | IntArray (ars, it, ia) -> "IntArray not Float32Array" |> Rop.fail
            | Float32Array (ars, it, fa) -> (it, fa) |> Rop.succeed
            | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not Float32Array" |> Rop.fail
            | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not Float32Array" |> Rop.fail
            | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not Float32Array" |> Rop.fail
        | Failure errors -> Failure errors


    let GetListOfBoolArray (entityRepo:IEntityRepo) (dataId:DataId) =
        match entityRepo.GetData dataId with
        | Success (entityData, msgs) -> 
            match entityData.ArrayData with 
            | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
            | IntArray (ars, it, ia) -> "IntArray not ListBoolArray" |> Rop.fail
            | Float32Array (ars, it, fa) -> "Float32Array not ListBoolArray" |> Rop.fail
            | ListOfBoolArray (ars, lba) -> lba |> Rop.succeed
            | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not ListBoolArray" |> Rop.fail
            | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not ListBoolArray" |> Rop.fail
        | Failure errors -> Failure errors


    let GetListOfIntArray (entityRepo:IEntityRepo) (dataId:DataId) =
        match entityRepo.GetData dataId with
        | Success (entityData, msgs) -> 
            match entityData.ArrayData with 
            | BoolArray (ars, ba) -> "BoolArray not ListOfIntArray" |> Rop.fail
            | IntArray (ars, it, ia) -> "IntArray not ListOfIntArray" |> Rop.fail
            | Float32Array (ars, it, fa) -> "Float32Array not ListOfIntArray" |> Rop.fail
            | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not ListOfIntArray" |> Rop.fail
            | ListOfIntArray (ars, it, lia) -> (it, lia) |> Rop.succeed
            | ListOfFloat32Array (ars, ft, lfa) -> "ListOfFloat32Array not ListOfIntArray" |> Rop.fail
        | Failure errors -> Failure errors


    let GetListOfFloat32Array (entityRepo:IEntityRepo) (dataId:DataId) =
        match entityRepo.GetData dataId with
        | Success (entityData, msgs) -> 
            match entityData.ArrayData with 
            | BoolArray (ars, ba) -> "BoolArray not ListOfFloat32Array"|> Rop.fail
            | IntArray (ars, it, ia) -> "IntArray not ListOfFloat32Array" |> Rop.fail
            | Float32Array (ars, it, fa) -> "Float32Array not ListOfFloat32Array" |> Rop.fail
            | ListOfBoolArray (ars, lba) -> "ListOfBoolArray not ListOfFloat32Array" |> Rop.fail
            | ListOfIntArray (ars, it, lia) -> "ListOfIntArray not ListOfFloat32Array" |> Rop.fail
            | ListOfFloat32Array (ars, ft, lfa) -> (ft, lfa) |> Rop.succeed
        | Failure errors -> Failure errors


    let GetSourceEntityData (entity:Entity) (epn:Epn) =
       try
        entity.SourceData |> List.find(fun e-> e.Epn = epn) |> Rop.succeed
       with
        | ex -> (sprintf "Source data not found: %s" (EpnString epn)) |> Rop.fail

    let GetResultEntityData (entity:Entity) (epn:Epn) =
       try
        entity.ResultData |> List.find(fun e-> e.Epn = epn) |> Rop.succeed
       with
        | ex -> (sprintf "Source data not found: %s" (EpnString epn)) |> Rop.fail

    let GetSourceDataRecord (entityRepo:IEntityRepo) (entity:Entity) (epn:Epn) =
        match GetSourceEntityData entity epn with
        | Success (entityData, msgs) -> 
            entityRepo.GetData entityData.DataId
        | Failure errors -> Failure errors

    let GetResultDataRecord (entityRepo:IEntityRepo) (entity:Entity) (epn:Epn) =
        match GetResultEntityData entity epn with
        | Success (entityData, msgs) -> 
            entityRepo.GetData entityData.DataId
        | Failure errors -> Failure errors

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
      
    let MakeEntityFromGen (entityRepo:IEntityRepo) (entityGen:IEntityGen) (name:string) =
        match (MakeResultData entityRepo entityGen) with
        | Success (resultData, msgs) -> 
                            {
                                Entity.Name = EntityName(name);
                                Entity.EntityId = entityGen.EntityId;
                                GeneratorId = entityGen.GeneratorId;
                                Params = entityGen.Params;
                                Iteration = entityGen.Iteration;
                                SourceData = entityGen.SourceData;
                                ResultData = resultData
                                    |> List.map(fun dr -> dr |> ToEntityData)
                            } |> Rop.succeed
        | Failure errors -> Failure errors

    let SaveEntityFromGen (entityRepo:IEntityRepo) (entityGen:IEntityGen) (name:string) =
        match (MakeEntityFromGen entityRepo entityGen name) with
        | Success (resultEntity, msgs) -> 
                            SaveEntity entityRepo resultEntity 
        | Failure errors -> Failure errors