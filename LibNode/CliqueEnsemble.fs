namespace LibNode

open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open Rop

type CliqueEnsembleDto = { id:Guid; 
                           unsigned:bool;
                           stepSize:float32;
                           noiseSeed:int;
                           noiseLevel:float32 
                           ensemble: Float32Type * float32[];
                           connections: Float32Type * float32[];
                           }

type CliqueEnsembleGenCpu(prams:Param list, sourceData:EntityData list, 
                          ensemble:Matrix<float32>, connections:Matrix<float32>,
                          iteration:int, stepSize:float32, noiseVals:seq<float32>) =
    let _params = prams
    let _sourceData = sourceData
    let _ensemble = ensemble
    let _connections = connections
    let _iteration = iteration
    let _stepSize = stepSize
    let _noiseVals = noiseVals


 module CliqueEnsembleBuilder =

    let CreateCliqueEnsembleDto
                        (id:Guid)
                        (unsigned:bool)
                        (stepSize:float32)
                        (noiseSeed:int)
                        (noiseLevel:float32) 
                        (ensemble: Float32Type * float32[])
                        (connections: Float32Type * float32[]) =
        

        { id=id; unsigned=unsigned; stepSize=stepSize; noiseSeed=noiseSeed;
          noiseLevel=noiseLevel; ensemble=ensemble; connections=connections }

    let CreateCliqueEnsembleFromParams (entityRepo:IEntityRepo) (ensembleId:DataId) 
                                       (connectionsId:DataId) (prams:Param list) =

        let cliqueEnsembleDto = CreateCliqueEnsembleDto
                                <!> (Parameters.GetGuidParam prams "EntityId")
                                <*> (Parameters.GetBoolParam prams "Unsigned")
                                <*> (Parameters.GetFloat32Param prams "StepSize")
                                <*> (Parameters.GetIntParam prams "NoiseSeed")
                                <*> (Parameters.GetFloat32Param prams "NoiseLevel")
                                <*> (EntityOps.GetFloat32Array entityRepo ensembleId)
                                <*> (EntityOps.GetFloat32Array entityRepo connectionsId)
        None