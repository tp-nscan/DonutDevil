namespace LibNode
open System
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open Rop
open MathUtils
open ArrayDataGen
open ArrayDataExtr
open EntityOps

type CorrelatorDto = { 
                        trimScale:float32;
                        states:Matrix<float32>;
                     }

type CorrelatorGen(prams:Param list,
                   sourceData: EntityData list,
                   states:Matrix<float32>,
                   trimScale:float32) =

    let _sourceData = sourceData
    let _params = prams
    let _trimScale = trimScale
    let _states = states
    let _arrayShape = ArrayShape.Block {rows=_states.ColumnCount;cols=_states.ColumnCount}
    let _connections = _states.Transpose().Multiply(_states).ToArray() 
                        |> ZeroTheDiagonalF32
                        |> flattenColumnMajor 
                        |> Seq.toArray
                        |> TrimByFraction _trimScale

    interface IEntityGen with
        member this.GeneratorId =
            { Name="Correlator"; Version=1 }
        member this.Iteration = 
            0
        member this.SourceData =
            _sourceData
        member this.Params = 
            _params
        member this.GenResultStates = 
            [(IsFresh(true), Epn("Connections"))]
        member this.GetGenResult(epn:Epn) = 
            match epn with
            | Epn("Connections") -> 
                {
                    GenResult.Epn=Epn("Connections"); 
                    GenResult.ArrayData = 
                        Float32Array
                            ( 
                                _arrayShape,
                                Float32Type.SF 1.0f, 
                                _connections,
                                Array.empty<int>
                            );
                } |> Rop.succeed

            | Epn(s) -> sprintf "DataChunkName %s not found" s |> Rop.fail


 module CgBuilder =

    let CreateCorrelatorDto (trimScale:float32) (states:Matrix<float32>) =
        { trimScale=trimScale; states=states; }

    let CreateCorrelatorFromParams (entityRepo:IEntityRepo) (statesData:EntityData) 
                                   (prams:Param list) =

        let dtoResult = CreateCorrelatorDto
                        <!> (Parameters.GetFloat32Param prams "TrimScale")
                        <*> ((GetArrayData entityRepo statesData.DataId) 
                                |> bindR GetFloat32ArrayData |> bindR MakeDenseMatrix)

        match dtoResult with
            | Success (dto, msgs) ->
                new CorrelatorGen(
                        prams = prams, 
                        sourceData = [statesData],
                        states = dto.states,
                        trimScale = dto.trimScale
                    ) |> Rop.succeed

            | Failure errors -> Failure errors



    let MakeCorrelatorEntity(repo:IEntityRepo) (statesData:EntityData)
                            (trimScale:float32) (entityName:string) =

        let prams = Parameters.Correlator trimScale

        match (CreateCorrelatorFromParams repo statesData prams) with
        | Success (gen, msgs) -> EntityOps.SaveEntityGen repo gen entityName
        | Failure errors -> Failure errors


    let MakeCorrelatorEntityData(repo:IEntityRepo) (statesData:EntityData)
                            (trimScale:float32) (entityName:string) =

        match (MakeCorrelatorEntity repo statesData trimScale entityName) with
        | Success (entity, msgs) ->
                        let epn = Entvert.ToEpn("Connections")
                        EntityOps.GetResultEntityData entity epn
        | Failure errors -> Failure errors


    let MakeCorrelatorDataRecord(repo:IEntityRepo) (statesData:EntityData)
                            (trimScale:float32) (entityName:string) =

        match (MakeCorrelatorEntity repo statesData trimScale entityName) with
        | Success (entity, msgs) ->
                        let epn = Entvert.ToEpn("Connections")
                        EntityOps.GetResultDataRecord repo entity epn
        | Failure errors -> Failure errors