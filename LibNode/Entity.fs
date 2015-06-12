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
    abstract member SetEntity: Entity -> RopResult<string, string>
    abstract member SetData: GenResult -> RopResult<DataRecord, string>

module EntityOps =

    let ExGuid wt =
        match wt with
        | GuidId g -> g

    let GetString wt =
        match wt with
        | GuidId g -> g.ToString()

    let ToDataRecord (genResult:GenResult) =
        {
            DataRecord.DataId = GuidId(Guid.NewGuid());
            EntityId = genResult.EntityId;
            Epn = genResult.Epn;
            ArrayData = genResult.ArrayData;
        }

    let MakeEntityData (entityGen:IEntityGen) =
        entityGen.GenResultStates 
            |> List.map(fun r -> entityGen.GetGenResult(snd r))
            |> MergeResultList
    

    let PushGenResultsThroughRepoToEntityData (entityRepo:IEntityRepo) (genResults:GenResult list) =
        try
            genResults |> List.map(fun gr-> entityRepo.SetData) |> Rop.succeed
        with
        | ex -> (sprintf "Error saving GenResults: %s" ex.Message) |> Rop.fail
                
 
    let MakeEntity (entityRepo:IEntityRepo) (entityGen:IEntityGen) (resultData:EntityData list) =
        try

        entityRepo.SetEntity {
                                Entity.EntityId = entityGen.EntityId;
                                GeneratorId = entityGen.GeneratorId;
                                Params = entityGen.Params;
                                Iteration = entityGen.Iteration;
                                SourceData = entityGen.SourceData;
                                ResultData = resultData; 
                             } |> Rop.succeed

        with
        | ex -> (sprintf "Error saving Entity: %s" ex.Message) |> Rop.fail



      
    let SaveEntityGen<'a> (entityRepo:IEntityRepo) (entityGen:IEntityGen) (thing:'a) =
        let makeEntityInRepo = MakeEntity entityRepo entityGen
        let pushDataThrough = PushGenResultsThroughRepoToEntityData entityRepo
        let genResultList = MakeEntityData entityGen
        
        match genResultList with
              | Success (resultList, msgs) -> 
                    resultList |> Rop.succeed
              | Failure errors -> Failure errors

       // Some thing



//    let MakeEntity (entityGen:IEntityGen) (entityRepo:IEntityRepo) =
//        let resultSets = entityGen.GenResultStates 
//                            |> List.map(fun rs -> entityGen.GetGenResult(snd rs)) 
//                            |> Rop.MergeResultList
//
//        match resultSets with
//            | Success (x,_) ->  
//            
//                               {
//                                    Entity.EntityId = entityGen.EntityId;
//                                    GeneratorId = entityGen.GeneratorId;
//                                    Params = entityGen.Params;
//                                    Iteration = entityGen.Iteration;
//                                    SourceData = entityGen.SourceData;
//                                    ResultData = entityGen.SourceData; 
//                                } |> Rop.succeed
//
//            | Failure errors -> errors |> Rop.fail
