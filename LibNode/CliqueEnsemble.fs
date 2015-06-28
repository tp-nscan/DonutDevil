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

type CliqueEnsembleGenCpu(prams:Param list,
                          sourceData: EntityData list,
                          arrayShape:ArrayShape,
                          ensemble:Matrix<float32>,
                          connections:Matrix<float32>,
                          iteration:int,
                          stepSize:float32,
                          seqNoise:seq<float32>) =

    let _sourceData = sourceData
    let _arrayShape = arrayShape
    let _params = prams
    let _ensemble = ensemble
    let _connections = connections
    let _iteration = iteration
    let _stepSize = stepSize
    let _seqNoise = seqNoise

    interface ISym with
        member this.GeneratorId =
            {Name="CliqueEnsembleGenCpu"; Version=1}
        member this.Iteration = 
            _iteration
        member this.SourceData =
            _sourceData
        member this.Params = 
            _params
        member this.GenResultStates = 
            [(IsFresh(true), Epn("Ensemble"))]
        member this.GetGenResult(epn:Epn) = 
            match epn with
            | Epn("Ensemble") -> 
                {
                    GenResult.Epn=Epn("Ensemble"); 
                    GenResult.ArrayData = 
                        ArrayData.Float32Array
                            (
                                _arrayShape, 
                                Float32Type.SF 1.0f,
                                _ensemble.ToArray() |> flattenColumnMajor |> Seq.toArray,
                                Array.empty<int>
                            )
                 } |> Rop.succeed
            
            | Epn(s) -> sprintf "Epn %s not found" s |> Rop.fail
        member this.Update() =
           try
             let randIter = Generators.SeqIter _seqNoise
             let updated = _ensemble.Multiply _connections
             let newStates = _ensemble.Map2((fun x y -> MathUtils.BoundUnitSF32( x + y * _stepSize + randIter())), updated)

             new  CliqueEnsembleGenCpu(
                          prams = _params,
                          sourceData = _sourceData,
                          arrayShape = _arrayShape,
                          ensemble = newStates, 
                          connections = _connections,
                          iteration = _iteration + 1,
                          stepSize = _stepSize, 
                          seqNoise = _seqNoise)
                          :> ISym
                          |> Rop.succeed
           with
            | ex -> (sprintf "Error updating CliqueEnsembleGenCpu: %s" ex.Message) |> Rop.fail


 module CegBuilder =

    type CliqueEnsembleDto = { 
                               stepSize:float32;
                               noiseSeed:int;
                               noiseLevel:float32;
                               arrayShape:ArrayShape;
                               ensemble:Matrix<float32>;
                               connections: Matrix<float32>;
                             }

    let CreateCliqueEnsembleDto
                        (stepSize:float32)
                        (noiseSeed:int)
                        (noiseLevel:float32)
                        (arrayShape:ArrayShape)
                        (ensemble: Matrix<float32>)
                        (connections: Matrix<float32>) =

        { stepSize=stepSize; noiseSeed=noiseSeed;
          noiseLevel=noiseLevel; arrayShape=arrayShape;
          ensemble=ensemble; connections=connections }


    let CreateCliqueEnsembleFromParams (entityRepo:IEntityRepo) (entityData:seq<EntityData>) 
                                       (prams:Param list) =

        let dtoResult = CreateCliqueEnsembleDto
                        <!> (Parameters.GetFloat32Param prams "StepSize")
                        <*> (Parameters.GetIntParam prams "NoiseSeed")
                        <*> (Parameters.GetFloat32Param prams "NoiseLevel")
                        <*> ((GetDataIdForEpn entityData (Epn("Ensemble"))) 
                            |> bindR (GetArrayData entityRepo) |> bindR GetArrayShape)
                        <*> ((GetDataIdForEpn entityData (Epn("Ensemble"))) 
                            |> bindR (GetArrayData entityRepo) |> bindR GetFloat32ArrayData |> bindR MakeDenseMatrix)
                        <*> ((GetDataIdForEpn entityData (Epn("Connections"))) 
                            |> bindR (GetArrayData entityRepo) |> bindR GetFloat32ArrayData |> bindR MakeDenseMatrix)

        match dtoResult with
            | Success (dto, msgs) ->
                new CliqueEnsembleGenCpu(
                        prams = prams, 
                        sourceData = (entityData |> Seq.toList),
                        arrayShape = dto.arrayShape,
                        ensemble = dto.ensemble,
                        connections = dto.connections,
                        iteration = 0,
                        stepSize = dto.stepSize,
                        seqNoise = ArrayDataGen.RandFloat32 
                                        (Float32Type.SF(dto.noiseLevel)) 
                                        0.5f 
                                        (Random.MersenneTwister(dto.noiseSeed))
                    ) |> Rop.succeed

            | Failure errors -> Failure errors


    let ExtractEnsemble (cliqueEnsembleGenCpu:ISym) =
        cliqueEnsembleGenCpu.GetGenResult(Epn("Ensemble")) 
            |> bindR GetArrayDataFromGenResult
            |> bindR GetFloat32ArrayData
            |> bindR GetTuple3of4
            |> ExtractResult


 module CliqueEnsemble =

    let MakeGenForRandomCliqueEnsemble (repo:IEntityRepo) (ensembleCount:int) (nodeCount:int) 
                (seed:int) (maxVal:float32) (noiseLevel:float32) (stepSize:float32) (name:string)=

        let rng = Random.MersenneTwister(seed)
        let ensembleSeed = rng.Next()
        let connectionSeed = rng.Next()
        let noiseSeed = rng.Next()

        let prams = Parameters.CliqueSet false stepSize noiseSeed noiseLevel

        let ensembleResult = RmgUtil.MakeRdmDataRecord repo ensembleCount
                                nodeCount ensembleSeed maxVal (sprintf "%s_%s" name "Ensemble")
                             |> liftR (EntityOps.ToEntityData (Entvert.ToEpn("Ensemble")))

        let connectionsResult = RmgUtil.MakeRdmDataRecord repo nodeCount
                                  nodeCount connectionSeed maxVal (sprintf "%s_%s" name "Connections")
                             |> liftR (EntityOps.ToEntityData (Entvert.ToEpn("Connections")))

        match (MergeResultList [ensembleResult; connectionsResult]) with
                | Success (entityDataSeq, msgs) -> 
                    (CegBuilder.CreateCliqueEnsembleFromParams repo entityDataSeq prams)
                | Failure errors -> Failure errors