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

type PerturbGen(prams:Param list, 
                sourceData: EntityData list,
                states:float32[,],
                mutationRate:float32,
                seed:int,
                replicationRate:int,
                float32Type:Float32Type) =

    let _params = prams
    let _sourceData = []
    let _states = states
    let _mutationRate = mutationRate
    let _seed = seed
    let _replicationRate = replicationRate
    let _arrayShape = ArrayShape.Block {rows=_states.GetLength(1)*replicationRate; 
                                        cols=_states.GetLength(0)}
    let _float32Type = float32Type
    let _perturbed = (Generators.MesForArraySF32 _mutationRate (Random.MersenneTwister(_seed))  
                            _replicationRate (GetRowsForArray2D _states))
                        |> Array.concat

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
            [(IsFresh(true), Epn("Perturbed"))]
        member this.GetGenResult(epn:Epn) = 
            match epn with
            | Epn("Perturbed") -> 
                {
                    GenResult.Epn=Epn("Perturbed"); 
                    GenResult.ArrayData = 
                        Float32Array
                            ( 
                                _arrayShape,
                                _float32Type, 
                                _perturbed,
                                Array.empty<int>
                            );
                } |> Rop.succeed

            | Epn(s) -> sprintf "DataChunkName %s not found" s |> Rop.fail


module PerturbBuilder =

    type PerturbDto = { 
                    seed:int;
                    mutationRate:float32;
                    unsigned:bool;
                    replicationRate:int;
                    states:float32[,];
                  }

    let CreatePerturbDto (seed:int) (mutationRate:float32) 
                         (replicationRate:int) 
                         (unsigned:bool) 
                         (states:float32[,]) =
        { seed=seed; mutationRate=mutationRate; replicationRate=replicationRate; 
            unsigned=unsigned; states=states }

    let CreatePerturbGenFromParams (entityRepo:IEntityRepo) (statesData:EntityData) 
                                   (prams:Param list) =

        let dtoResult = CreatePerturbDto
                    <!> (Parameters.GetIntParam prams "Seed")
                    <*> (Parameters.GetFloat32Param prams "MutationRate")
                    <*> (Parameters.GetIntParam prams "ReplicationRate")
                    <*> (Parameters.GetBoolParam prams "Unsigned")
                    <*> ((GetArrayData entityRepo statesData.DataId) 
                                |> bindR GetFloat32ArrayData |> bindR MakeNestedArrays)

        match dtoResult with
            | Success (dto, msgs) ->
                new PerturbGen(
                        prams = prams, 
                        sourceData = [statesData],
                        states = dto.states,
                        mutationRate  = dto.mutationRate,
                        seed = dto.seed,
                        replicationRate = dto.replicationRate,
                        float32Type = (ArrayDataGen.ToFloat32Type 
                                        (dto.unsigned |> DataShapeProcs.UnsignedToFloatRange)
                                        FloatCover.Continuous 
                                        1.0f)
                    ) |> Rop.succeed

            | Failure errors -> Failure errors


    let MakePerturberEntity(repo:IEntityRepo) (statesData:EntityData)
                           (seed:int) (mutationRate:float32) (replicationRate:int) 
                           (unsigned:bool) (entityName:string) =

        let prams = Parameters.Peturber seed mutationRate replicationRate unsigned

        match (CreatePerturbGenFromParams repo statesData prams) with
        | Success (gen, msgs) -> EntityOps.SaveEntityGen repo gen entityName
        | Failure errors -> Failure errors


    let MakePerturberEntityData(repo:IEntityRepo) (statesData:EntityData)
                                (seed:int) (mutationRate:float32) (replicationRate:int) 
                                (unsigned:bool) (entityName:string) =

        match (MakePerturberEntity repo statesData seed mutationRate replicationRate unsigned entityName) with
        | Success (entity, msgs) ->
                        let epn = Entvert.ToEpn("Perturbed")
                        EntityOps.GetResultEntityData entity epn
        | Failure errors -> Failure errors


    let MakePerturberEntityDataRecord(repo:IEntityRepo) (statesData:EntityData)
                                       (seed:int) (mutationRate:float32) (replicationRate:int) 
                                       (unsigned:bool) (entityName:string) =

        match (MakePerturberEntity repo statesData seed mutationRate replicationRate unsigned entityName) with
        | Success (entity, msgs) ->
                        let epn = Entvert.ToEpn("Perturbed")
                        EntityOps.GetResultDataRecord repo entity epn
        | Failure errors -> Failure errors
