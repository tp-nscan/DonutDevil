namespace LibNode

open System
open Rop
open ArrayDataGen

module EntityOps =

    let ArrayDescrString (arrayDescr:ArrayDescr) =
        match arrayDescr with
        | BoolDescr ars -> 
            match ars with
                | Linear l -> sprintf "bool[%i]" l
                | Block ars -> sprintf "bool[%i][%i]" ars.rows ars.cols
                | UT ars -> sprintf "Upper Tr bool[%i][%i]" ars.rows ars.cols
                | Sparse srs -> sprintf "Sparse bool[%i][%i]" srs.rows srs.cols

        | IntDescr (ars, it) -> 
            match (ars, it) with
                | (Linear l, it) -> sprintf "%sint[%i]" (it |> IntTypeDescr) l
                | (Block ars, it)  -> sprintf "%s[%i][%i]" (it |> IntTypeDescr) ars.rows ars.cols
                | (UT ars, it) -> sprintf "Upper Tr %s[%i][%i]" (it |> IntTypeDescr) ars.rows ars.cols
                | (Sparse srs, it)  -> sprintf "Sparse %s[%i][%i]" (it |> IntTypeDescr) srs.rows srs.cols

        | Float32Descr (ars, ft)-> 
            match (ars, ft) with
                | (Linear l, it) -> sprintf "%sint[%i]" (ft |> Float32TypeDescr) l
                | (Block ars, it)  -> sprintf "%s[%i][%i]" (ft |> Float32TypeDescr) ars.rows ars.cols
                | (UT ars, it) -> sprintf "Upper Tr %s[%i][%i]" (ft |> Float32TypeDescr) ars.rows ars.cols
                | (Sparse srs, it)  -> sprintf "Sparse %s[%i][%i]" (ft |> Float32TypeDescr) srs.rows srs.cols

    
    let ArrayDataString (arrayData:ArrayData) =
        match arrayData with
            | BoolArray (ars, ba, ii) -> ArrayDescrString (ArrayDescr.BoolDescr ars)
            | IntArray (ars, it, ia, ii) -> ArrayDescrString (ArrayDescr.IntDescr (ars, it))
            | Float32Array (ars, ft, fa, ia) -> ArrayDescrString (ArrayDescr.Float32Descr (ars, ft))


    let DataRecordToArrayDescr (dataRecord:DataRecord) =
        match dataRecord.DrData with
        | BoolArray (ars, ba, ii) -> ArrayDescr.BoolDescr ars
        | IntArray (ars, it, ia, ii) -> ArrayDescr.BoolDescr ars
        | Float32Array (ars, ft, fa, ii) -> ArrayDescr.BoolDescr ars


    let GenResultToArrayDescr (genResult:GenResult) =
        match genResult.ArrayData with
        | BoolArray (ars, ba, ii) -> ArrayDescr.BoolDescr ars
        | IntArray (ars, it, ia, ii) -> ArrayDescr.BoolDescr ars
        | Float32Array (ars, ft, fa, ii) -> ArrayDescr.BoolDescr ars

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
            dr.DrData |> Rop.succeed
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


    let ToEntityData (dataRecord:DataRecord) =
        {
            EntityData.ArrayDescr = dataRecord |> DataRecordToArrayDescr;
            EntityData.DataId = dataRecord.DataId;
            EntityId = dataRecord.EntityId;
            EntityData.Epn = dataRecord.Epn
        }

    let MakeEntityData (entityId:EntityId) (arrayDescr:ArrayDescr) (epn:Epn)  =
        {
            EntityData.ArrayDescr = arrayDescr;
            EntityData.DataId = DataId.GuidId(Guid.NewGuid());
            EntityId = entityId;
            Epn = epn;
        }

    let MakeAllGenResults (entityGen:IEntityGen) =
        entityGen.GenResultStates 
            |> List.map(fun r -> (entityGen.GetGenResult (snd r)))
            |> MergeResultList
    
    
    let GenResultToDataRecords (entityId:EntityId) (entityGen:IEntityGen) =
        match (MakeAllGenResults entityGen) with
            | Success (genResults, msgs) -> 
                  genResults 
                  |>  List.map(fun gr -> 
                        {
                            DataRecord.DataId=DataId.GuidId(Guid.NewGuid());
                            EntityId = entityId;
                            DrData = gr.ArrayData;
                            Epn=gr.Epn
                        }) |> Rop.succeed
            | Failure errors -> Failure errors


    let SaveDataRecords (entityRepo:IEntityRepo) (dataRecords:DataRecord list) =
        try
            dataRecords |> List.map(fun gr -> gr |> entityRepo.SaveData)
                           |> Rop.succeed
        with
        | ex -> (sprintf "Error saving DataRecords") |> Rop.fail


    let SaveEntityInRepo (entityRepo:IEntityRepo) (entityId:EntityId) 
                         (name:string) (entityGen:IEntityGen) 
                         (dataRecords:DataRecord list) =
        try
            match (SaveDataRecords entityRepo dataRecords) with
                | Success (drs, msgs) -> 
                    entityRepo.SaveEntity     
                        {
                            Name=EntityName(name);
                            EntityId=entityId;
                            GeneratorId=entityGen.GeneratorId;
                            Params=entityGen.Params;
                            Iteration=entityGen.Iteration;
                            SourceData=entityGen.SourceData;
                            ResultData=dataRecords |> List.map (fun dr -> dr |> ToEntityData); 
                        }
                | Failure errors -> Failure errors
        with
        | ex -> (sprintf "Error saving Entity: %s" name) |> Rop.fail


    let SaveEntityGen (entityRepo:IEntityRepo) (entityGen:IEntityGen) (name:string) =

        let entityId = EntityId.GuidId(Guid.NewGuid())
        try
            match (GenResultToDataRecords entityId entityGen) with
            | Success (dataRecords, msgs) -> SaveEntityInRepo entityRepo entityId 
                                                              name entityGen dataRecords
            | Failure errors -> Failure errors

        with
        | ex -> (sprintf "Error saving Entity") |> Rop.fail



    let GetArrayDataFromGenResult (genResult:GenResult) =
        genResult.ArrayData |> Rop.succeed 