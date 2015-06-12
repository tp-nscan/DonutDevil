namespace LibNode

open System
open Rop

type RandMatrixDto = { id:Guid; rowCount:int; colCount:int; 
                        seed:int; unsigned:bool; maxValue:float32 }

type RandMatrixGen(prams:Param list, entityId:EntityId, rowCount:int, 
                   colCount:int, seed:int, iteration:int, 
                   float32Type:Float32Type) =
    let _sourceData = []
    let _params = prams
    let _entityId = entityId
    let _seed = seed
    let _iteration = iteration
    let _rowCount = rowCount
    let _colCount = colCount
    let _float32Type = float32Type

    interface IEntityGen with
        member this.EntityId = _entityId
        member this.GeneratorId =
            { Name="RandMatrixGenerator"; Version=1 }
        member this.Iteration = 
            _iteration
        member this.SourceData =
            _sourceData
        member this.Params = 
            _params
        member this.GenResultStates = 
            [(IsFresh(true), Epn("Matrix"))]
        member this.GetGenResult(epn:Epn) = 
            match epn with
            | Epn("Matrix") -> 
                { 
                    GenResult.EntityId = _entityId;
                    GenResult.Epn=Epn("Matrix"); 
                    GenResult.ArrayData = 
                        Float32Array( _float32Type, 
                                    (NtGens.RandFloat32Seed _seed float32Type) 
                                    |> Seq.take(_rowCount*_colCount) 
                        |> Seq.toArray);
                } |> Rop.succeed

            | Epn(s) -> sprintf "DataChunkName %s not found" s |> Rop.fail


 module RmgBuilder =

    type RandomCesrDto = {id:Guid; noiseSeed:int; cnxnSeed:int; ensembleCount:int; nodeCount:int; 
                          stepSize:float32; noiseLevel:float32}


    let CreateRandomDto (id:Guid)
                        (nodeCount:int)
                        (ensembleCount:int)
                        (stepSize:float32)
                        (noiseSeed:int)
                        (noiseLevel:float32) 
                        (cnxnSeed:int)  =
        { id=id; noiseSeed=noiseSeed; cnxnSeed=cnxnSeed; ensembleCount=ensembleCount;
        nodeCount=nodeCount; stepSize=stepSize; noiseLevel=noiseLevel }


    let CreateRandMatrixDto (id:Guid) (rowCount:int) (colCount:int) (seed:int)
                            (unsigned:bool) (maxValue:float32) =
        { id=id; rowCount=rowCount; colCount=colCount; seed=seed;
            unsigned=unsigned; maxValue=maxValue; }


    let CreateRandMatrixFromParams (prams:Param list) =

        let dtoResult = CreateRandMatrixDto
                    <!> (Parameters.GetGuidParam prams "EntityId")
                    <*> (Parameters.GetIntParam prams "RowCount")
                    <*> (Parameters.GetIntParam prams "ColCount")
                    <*> (Parameters.GetIntParam prams "Seed")
                    <*> (Parameters.GetBoolParam prams "Unsigned")
                    <*> (Parameters.GetFloat32Param prams "MaxValue")

        match dtoResult with
            | Success (dto, msgs) -> 
                    new RandMatrixGen(prams=prams, 
                                      entityId=EntityId.GuidId(dto.id), 
                                      rowCount=dto.rowCount, 
                                      colCount=dto.colCount, 
                                      seed=dto.seed, 
                                      iteration=0,
                                      float32Type=(NtGens.ToFloat32Type dto.unsigned dto.maxValue)) 
                                     |> Rop.succeed
            | Failure errors -> Failure errors
