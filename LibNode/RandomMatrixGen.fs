namespace LibNode

open System
open Rop


type RandMatrixGen(prams:Param list, 
                   arrayShape:ArrayShape, 
                   seed:int,
                   float32Type:Float32Type) =

    let _sourceData = []
    let _params = prams
    let _arrayShape = arrayShape
    let _seed = seed
    let _float32Type = float32Type

    interface IEntityGen with
        member this.GeneratorId =
            { Name="RandMatrixGenerator"; Version=1 }
        member this.Iteration = 
            0
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
                    GenResult.Epn=Epn("Matrix"); 
                    GenResult.ArrayData = 
                        Float32Array
                            ( 
                                _arrayShape,
                                _float32Type, 
                                (ArrayDataGen.RandFloat32Seed _seed float32Type) 
                                |> Seq.take(ArrayDataGen.FullArrayCount _arrayShape) 
                                |> Seq.toArray,
                                Array.empty<int>
                            );
                } |> Rop.succeed

            | Epn(s) -> sprintf "DataChunkName %s not found" s |> Rop.fail


 module RmgBuilder =

    type RandMatrixDto = { rowCount:int; colCount:int; 
                       seed:int; unsigned:bool; maxValue:float32 }

    let CreateRandMatrixDto (rowCount:int) (colCount:int) (seed:int)
                            (unsigned:bool) (maxVal:float32) =
        { rowCount=rowCount; colCount=colCount; seed=seed;
            unsigned=unsigned; maxValue=maxVal; }


    let RandMatrixGenFromParams (prams:Param list) =

        let dtoResult = CreateRandMatrixDto
                    <!> (Parameters.GetIntParam prams "RowCount")
                    <*> (Parameters.GetIntParam prams "ColCount")
                    <*> (Parameters.GetIntParam prams "Seed")
                    <*> (Parameters.GetBoolParam prams "Unsigned")
                    <*> (Parameters.GetFloat32Param prams "MaxValue")

        match dtoResult with
            | Success (dto, msgs) -> 
                    new RandMatrixGen(
                        prams=prams, 
                        arrayShape = (ArrayDataGen.FullArrayShape2d dto.rowCount dto.colCount),
                        seed=dto.seed,
                        float32Type = (ArrayDataGen.ToFloat32Type dto.unsigned 1.0f)) 
                        |> Rop.succeed
            | Failure errors -> Failure errors



  module RmgUtil =

    let MakeGenForRandomDenseMatrix (rowCount:int) (colCount:int) (seed:int) (maxVal:float32) =
        RmgBuilder.RandMatrixGenFromParams (Parameters.RandomMatrixSet rowCount colCount seed maxVal)


    let MakeRandomDenseMatrixEntity (repo:IEntityRepo) (rowCount:int) (colCount:int) 
                                        (seed:int) (maxVal:float32) (entityName:string) =
        match (MakeGenForRandomDenseMatrix rowCount colCount seed maxVal) with
        | Success (gen, msgs) -> EntityOps.SaveEntityGen repo gen entityName
        | Failure errors -> Failure errors


    let MakeRandomDenseMatrixDataRecord (repo:IEntityRepo) (rowCount:int) (colCount:int) 
                                        (seed:int) (maxVal:float32) (entityName:string) =
        match (MakeRandomDenseMatrixEntity repo rowCount colCount seed maxVal entityName) with
        | Success (entity, msgs) ->
                        let epn = Entvert.ToEpn("Matrix")
                        EntityOps.GetResultDataRecord repo entity epn
        | Failure errors -> Failure errors
