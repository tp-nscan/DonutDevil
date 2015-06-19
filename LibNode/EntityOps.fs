namespace LibNode

open System
open Rop
open ArrayDataGen

module EntityOps =

    let ArrayDataDescr (arrayData:ArrayData) =
        match arrayData with
        | BoolArray (ars, ba) -> 
            match ars with
            | OneD l -> sprintf "bool[%i]" l
            | TwoD tds -> match tds with
                        | UT ars -> sprintf "Upper Tr bool[%i][%i]" ars.rows ars.cols
                        | LT ars -> sprintf "Lower Tr bool[%i][%i]" ars.rows ars.cols
                        | Full ars -> sprintf "bool[%i][%i]" ars.rows ars.cols
                        | TwoDShape.Sparse (ars, l) -> sprintf "Sparse bool[%i][%i]" ars.rows ars.cols

        | IntArray (ars, it, ia) -> 
            match ars with
            | OneD l -> sprintf "int[%i]" l
            | TwoD tds -> match tds with
                        | UT ars -> sprintf "Upper Tr %s[%i][%i]" (it |>IntTypeDescr) ars.rows ars.cols
                        | LT ars -> sprintf "Lower Tr %s[%i][%i]" (it |>IntTypeDescr) ars.rows ars.cols
                        | Full ars -> sprintf "%s[%i][%i]" (it |>IntTypeDescr) ars.rows ars.cols
                        | TwoDShape.Sparse (ars, l) -> sprintf "Sparse %s[%i][%i]" (it |>IntTypeDescr) ars.rows ars.cols

        | Float32Array (ars, ft, fa) -> 
            match ars with
            | OneD l -> sprintf "float[%i]" l
            | TwoD tds -> match tds with
                        | UT ars -> sprintf "Upper Tr %s[%i][%i]" (ft |>Float32TypeDescr) ars.rows ars.cols
                        | LT ars -> sprintf "Lower Tr %s[%i][%i]" (ft |>Float32TypeDescr) ars.rows ars.cols
                        | Full ars -> sprintf "%s[%i][%i]" (ft |>Float32TypeDescr) ars.rows ars.cols
                        | TwoDShape.Sparse (ars, l) -> sprintf "Sparse %s[%i][%i]" (ft |>Float32TypeDescr) ars.rows ars.cols


        | ListOfBoolArray (ars, lba) -> match ars with
                                     | OneD l -> sprintf ""
                                     | TwoD tds -> sprintf ""
        | ListOfIntArray (ars, it, lia) -> match ars with
                                     | OneD l -> sprintf ""
                                     | TwoD tds -> sprintf ""
        | ListOfFloat32Array (ars, ft, lfa) -> match ars with
                                     | OneD l -> sprintf ""
                                     | TwoD tds -> sprintf ""

    let EpnString epn =
        match epn with
        | Epn s -> s

    let EntityString eg =
        match eg.EntityId with
        | EntityId.GuidId g -> g.ToString()

    let DataString dg =
        match dg.DataId with
        | DataId.GuidId g -> g.ToString()

    let GetArrayData (entityRepo:IEntityRepo) (dataId:DataId) =
        match (entityRepo.GetData dataId) with
        | Success (dr, msgs) -> 
            dr.ArrayData |> Rop.succeed
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

    let ToDataRecord (entity:Entity) (genResult:GenResult) =
        match (GetResultEntityData entity genResult.Epn) with
        | Success (entityData, msgs) -> 
            {
                DataRecord.DataId = entityData.DataId
                EntityId = entity.EntityId;
                Epn = genResult.Epn;
                ArrayData = genResult.ArrayData;
            } |> Rop.succeed
        | Failure errors -> Failure errors
    

    let ToEntityData (dataRecord:DataRecord) =
        {
            EntityData.DataId = dataRecord.DataId;
            EntityId = dataRecord.EntityId;
            Epn = dataRecord.Epn;
        }


    let MakeEntityData (entityId:EntityId) (epn:Epn) =
        {
            EntityData.DataId = DataId.GuidId(Guid.NewGuid());
            EntityId = entityId;
            Epn = epn;
        }

    let MakeAllGenResults (entityGen:IEntityGen) =
        entityGen.GenResultStates 
            |> List.map(fun r -> entityGen.GetGenResult(snd r))
            |> MergeResultList
    

    let MakeAllDataRecords (entityGen:IEntityGen) (entity:Entity) =
        match (MakeAllGenResults entityGen) with
        | Success (genResults, msgs) -> 
                genResults |> List.map(fun gr -> gr |> ToDataRecord entity)
                           |> MergeResultList
        | Failure errors -> Failure errors


    let SaveAllDataRecords (entityRepo:IEntityRepo) (entityGen:IEntityGen) (entity:Entity) =
        match (MakeAllDataRecords entityGen entity) with
        | Success (dataRecords, msgs) -> 
               dataRecords |> List.map(fun gr -> gr |> entityRepo.SaveData)
                           |> Rop.succeed
        | Failure errors -> Failure errors


    let SaveEntity (entityRepo:IEntityRepo) (entity:Entity) =
        try
            entityRepo.SaveEntity entity
        with
        | ex -> (sprintf "Error saving Entity: %s" (EntityString entity)) |> Rop.fail


    let MakeEntity (entityGen:IEntityGen) (name:string)  =        
        let entityId = EntityId.GuidId(Guid.NewGuid())
        {
            Entity.Name = EntityName.EntityName(name);
            EntityId = entityId;
            GeneratorId = entityGen.GeneratorId;
            Params = entityGen.Params;
            Iteration = entityGen.Iteration;
            SourceData = entityGen.SourceData;
            ResultData = entityGen.GenResultStates
                            |> List.map(fun grs-> (snd grs) |> MakeEntityData entityId); 
        }

    let SaveEntityGen (entityRepo:IEntityRepo) (entityGen:IEntityGen) (name:string) =

        let entity = MakeEntity entityGen name
        try
            match (SaveAllDataRecords entityRepo entityGen entity) with
            | Success (resultRecprds, msgs) -> SaveEntity entityRepo entity
            | Failure errors -> Failure errors

        with
        | ex -> (sprintf "Error saving Entity: %s" (EntityString entity)) |> Rop.fail

    let GetArrayDataFromGenResult (genResult:GenResult) =
        genResult.ArrayData |> Rop.succeed 