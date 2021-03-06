﻿namespace LibNode
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

type CorrelatorGen(prams:Param list,
                   sourceData: EntityData list,
                   states:Matrix<float32>,
                   clipFrac:float32) =

    let _sourceData = sourceData
    let _params = prams
    let _clipFrac = clipFrac
    let _states = states
    let _connections = _states.Transpose().Multiply(_states).ToArray() 
                        |> ZeroTheDiagonalF32
                        |> flattenColumnMajor 
                        |> Seq.toArray
                        |> ClipFractionSF32 _clipFrac

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
                                ArrayShape.Block {rows=_states.ColumnCount;cols=_states.ColumnCount},
                                Float32Type.SF 1.0f, 
                                _connections,
                                Array.empty<int>
                            );
                } |> Rop.succeed

            | Epn(s) -> sprintf "DataChunkName %s not found" s |> Rop.fail


 module CgBuilder =

    type CorrelatorDto = { 
                            clipFrac:float32;
                            states:Matrix<float32>;
                         }

    let CreateCorrelatorDto (clipFrac:float32) (states:Matrix<float32>) =
        { clipFrac=clipFrac; states=states; }

    let CreateCorrelatorGenFromParams (entityRepo:IEntityRepo) (statesData:EntityData) 
                                   (prams:Param list) =

        let dtoResult = CreateCorrelatorDto
                        <!> (Parameters.GetFloat32Param prams "ClipFrac")
                        <*> ((GetArrayData entityRepo statesData.DataId) 
                                |> bindR GetFloat32ArrayData |> bindR MakeDenseMatrix)

        match dtoResult with
            | Success (dto, msgs) ->
                new CorrelatorGen(
                        prams = prams, 
                        sourceData = [statesData],
                        states = dto.states,
                        clipFrac = dto.clipFrac
                    ) |> Rop.succeed

            | Failure errors -> Failure errors



    let MakeCorrelatorEntity(repo:IEntityRepo) (statesData:EntityData)
                            (clipFrac:float32) (entityName:string) =

        let prams = Parameters.Correlator clipFrac

        match (CreateCorrelatorGenFromParams repo statesData prams) with
        | Success (gen, msgs) -> EntityOps.SaveEntityGen repo gen entityName
        | Failure errors -> Failure errors


    let MakeCorrelatorEntityData(repo:IEntityRepo) (statesData:EntityData)
                            (clipFrac:float32) (entityName:string) =

        match (MakeCorrelatorEntity repo statesData clipFrac entityName) with
        | Success (entity, msgs) ->
                        let epn = Entvert.ToEpn("Connections")
                        EntityOps.GetResultEntityData entity epn
        | Failure errors -> Failure errors


    let MakeCorrelatorDataRecord(repo:IEntityRepo) (statesData:EntityData)
                            (clipFrac:float32) (entityName:string) =

        match (MakeCorrelatorEntity repo statesData clipFrac entityName) with
        | Success (entity, msgs) ->
                        let epn = Entvert.ToEpn("Connections")
                        EntityOps.GetResultDataRecord repo entity epn
        | Failure errors -> Failure errors