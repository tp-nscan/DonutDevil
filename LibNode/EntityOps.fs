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
                | (Linear l, t) -> sprintf "%sint[%i]" (t |> Float32TypeDescr) l
                | (Block ars, t)  -> sprintf "%s[%i][%i]" (t |> Float32TypeDescr) ars.rows ars.cols
                | (UT ars, t) -> sprintf "Upper Tr %s[%i][%i]" (t |> Float32TypeDescr) ars.rows ars.cols
                | (Sparse srs, t)  -> sprintf "Sparse %s[%i][%i]" (t |> Float32TypeDescr) srs.rows srs.cols

    
    let ArrayDataString (arrayData:ArrayData) =
        match arrayData with
            | BoolArray (ars, ba, ii) -> ArrayDescrString (ArrayDescr.BoolDescr ars)
            | IntArray (ars, it, ia, ii) -> ArrayDescrString (ArrayDescr.IntDescr (ars, it))
            | Float32Array (ars, ft, fa, ia) -> ArrayDescrString (ArrayDescr.Float32Descr (ars, ft))


    let ArrayDataToArrayDescr (arrayData:ArrayData) =
        match arrayData with
            | BoolArray (ars, ba, ii) -> ArrayDescr.BoolDescr ars
            | IntArray (ars, it, ia, ii) -> ArrayDescr.IntDescr (ars, it)
            | Float32Array (ars, ft, fa, ia) -> ArrayDescr.Float32Descr (ars, ft)


    let DataRecordToArrayDescr (dataRecord:DataRecord) =
        ArrayDataToArrayDescr dataRecord.DrData


    let GenResultToArrayDescr (genResult:GenResult) =
        ArrayDataToArrayDescr genResult.ArrayData


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


    let GetDataIdForEpn (entityData:seq<EntityData>) (epn:Epn) =
       try
          entityData |> Seq.find(fun e-> e.Epn = epn) |> (fun t->t.DataId) |> Rop.succeed
       with
        | ex -> (sprintf "Source data not found: %s" (EpnString epn)) |> Rop.fail


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

    let ToEntityData (epn:Epn) (dataRecord:DataRecord) =
        {
            EntityData.ArrayDescr = dataRecord |> DataRecordToArrayDescr;
            EntityData.DataId = dataRecord.DataId;
            EntityData.Epn = epn
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
                        (
                            gr.Epn,
                            {
                                DataRecord.DataId=DataId.GuidId(Guid.NewGuid());
                                EntityId = entityId;
                                DrData = gr.ArrayData;
                            }
                        )) |> Rop.succeed
            | Failure errors -> Failure errors


    let SaveDataRecords (entityRepo:IEntityRepo) (dataRecords:DataRecord list) =
        try
            dataRecords |> List.map(fun gr -> gr |> entityRepo.SaveData)
                           |> Rop.succeed
        with
        | ex -> (sprintf "Error saving DataRecords") |> Rop.fail


    let SaveEntityInRepo (entityRepo:IEntityRepo) (entityId:EntityId) 
                         (name:string) (entityGen:IEntityGen) 
                         (dataRecords:(Epn*DataRecord) list) =
        try
            match (SaveDataRecords entityRepo (dataRecords |> List.map snd)) with
                | Success (drs, msgs) -> 
                    entityRepo.SaveEntity     
                        {
                            Name=EntityName(name);
                            EntityId=entityId;
                            GeneratorId=entityGen.GeneratorId;
                            Params=entityGen.Params;
                            Iteration=entityGen.Iteration;
                            SourceData=entityGen.SourceData;
                            ResultData=dataRecords |> List.map (fun dr -> ToEntityData (fst dr) (snd dr)); 
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