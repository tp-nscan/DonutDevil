namespace LibNode

open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open Rop
open ArrayDataGen
open ArrayDataExtr
open EntityOps
open MathUtils

type CliqueEnsembleDto = { 
                           unsigned:bool;
                           stepSize:float32;
                           noiseSeed:int;
                           noiseLevel:float32;
                           arrayShape:ArrayShape;
                           ensemble:Matrix<float32>;
                           connections: Matrix<float32>;
                         }

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
            {Name="RandMatrixGenerator"; Version=1}
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
                                _ensemble.ToArray() |> FlattenColumnMajor,
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


 module CliqueEnsembleBuilder =

    let CreateCliqueEnsembleDto
                        (unsigned:bool)
                        (stepSize:float32)
                        (noiseSeed:int)
                        (noiseLevel:float32)
                        (arrayShape:ArrayShape)
                        (ensemble: Matrix<float32>)
                        (connections: Matrix<float32>) =

        { unsigned=unsigned; stepSize=stepSize; noiseSeed=noiseSeed;
          noiseLevel=noiseLevel; arrayShape=arrayShape;
          ensemble=ensemble; connections=connections }

    let CreateCliqueEnsembleFromParams (entityRepo:IEntityRepo) (ensembleId:DataId) 
                                       (connectionsId:DataId) (entityData:EntityData[]) 
                                       (prams:Param list) =

        let dtoResult = CreateCliqueEnsembleDto
                        <!> (Parameters.GetBoolParam prams "Unsigned")
                        <*> (Parameters.GetFloat32Param prams "StepSize")
                        <*> (Parameters.GetIntParam prams "NoiseSeed")
                        <*> (Parameters.GetFloat32Param prams "NoiseLevel")
                        <*> ((GetArrayData entityRepo ensembleId) |> bindR GetArrayShape)
                        <*> ((GetArrayData entityRepo ensembleId) |> bindR GetFloat32ArrayData |> bindR MakeDenseMatrix)
                        <*> ((GetArrayData entityRepo connectionsId) |> bindR GetFloat32ArrayData |> bindR MakeDenseMatrix)


        match dtoResult with
            | Success (dto, msgs) ->
                new CliqueEnsembleGenCpu(
                        prams = prams, 
                        sourceData = (entityData |> Array.toList),
                        arrayShape = dto.arrayShape,
                        ensemble = dto.ensemble,
                        connections = dto.connections,
                        iteration = 0,
                        stepSize = dto.stepSize,
                        seqNoise = Generators.RandFloatsF32 dto.noiseSeed dto.unsigned dto.noiseLevel 
                    ) |> Rop.succeed

            | Failure errors -> Failure errors

    let ExtractEnsemble (cliqueEnsembleGenCpu:ISym) =
        cliqueEnsembleGenCpu.GetGenResult(Epn("Ensemble")) 
            |> bindR GetArrayDataFromGenResult
            |> bindR GetFloat32ArrayData
            |> bindR GetTuple3of4
            |> ExtractResult