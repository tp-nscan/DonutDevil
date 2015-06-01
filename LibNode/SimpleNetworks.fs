namespace LibNode
open System
open System.Collections.Generic
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open MathUtils
open Generators
open NodeGroupBuilders
open Rop

    type CliqueEnsemble = { States: Matrix<float32>; Connections: Matrix<float32> }

    type CesrDto ={startSeed:int; ensembleCount:int; nodeCount:int; stepSize:float32; noise:float32}
    

module SimpleNetwork =

    let CreateRandCesr (seed:int) (stateCount:int) (nodeCount:int) =
        let randIter = Generators.SeqPopper (Generators.RandF32s seed OneF32)
        { 
          CliqueEnsemble.States = DenseMatrix.init stateCount nodeCount (fun i j -> randIter()); 
          Connections=DenseMatrix.init nodeCount nodeCount (fun i j -> randIter()) 
        }

    let Step (cliqueEnsemble:CliqueEnsemble) (step:float32) =
        let updated = cliqueEnsemble.States.Multiply cliqueEnsemble.Connections
        cliqueEnsemble.States.Map2((fun x y -> BoundUnitSF32(x + y*step)), updated)

    let StepWithNoise (seed:int) (noise:float32) (cliqueEnsemble:CliqueEnsemble) (step:float32) =
        let randIter = Generators.SeqPopper (Generators.RandF32s seed noise)

        let updated = cliqueEnsemble.States.Multiply cliqueEnsemble.Connections
        { 
            CliqueEnsemble.States = cliqueEnsemble.States.Map2((fun x y -> MathUtils.BoundUnitSF32( x + y * step + randIter())), updated)
            Connections = cliqueEnsemble.Connections
        }




module CesrDtoBuilder =

    let InitCesrDto (prams:IDictionary<string, Param>) =
        Rop.RopResult.Success((prams, {startSeed=0; ensembleCount=0; nodeCount=0; stepSize=ZeroF32; noise=ZeroF32}), [])
    
    let AddStartSeed (tuple: (IDictionary<string, Param> * CesrDto) ) =
            match (Parameters.GetIntParam (fst tuple) "StartSeed") with
            | Failure f -> Rop.fail f
            | Success (v,m) -> Rop.RopResult.Success(( (fst tuple) , {(snd tuple) with startSeed=v} ), [])

    let AddEnsembleCount (tuple: (IDictionary<string, Param> * CesrDto) ) =
            match (Parameters.GetIntParam (fst tuple) "EnsembleCount") with
            | Failure f -> Rop.fail f
            | Success (v,m) -> Rop.RopResult.Success(( (fst tuple) , {(snd tuple) with ensembleCount=v} ), [])

    let AddNodeCount(tuple: (IDictionary<string, Param> * CesrDto) ) =
            match (Parameters.GetIntParam (fst tuple) "NodeCount") with
            | Failure f -> Rop.fail f
            | Success (v,m) -> Rop.RopResult.Success(( (fst tuple) , {(snd tuple) with nodeCount=v} ), [])

    let AddStepSize (tuple: (IDictionary<string, Param> * CesrDto) ) =
            match (Parameters.GetFloat32Param (fst tuple) "StepSize") with
            | Failure f -> Rop.fail f
            | Success (v,m) -> Rop.RopResult.Success(( (fst tuple) , {(snd tuple) with stepSize=v} ), [])

    let AddNoise (tuple: (IDictionary<string, Param> * CesrDto) ) =
            match (Parameters.GetFloat32Param (fst tuple) "Noise") with
            | Failure f -> Rop.fail f
            | Success (v,m) -> Rop.RopResult.Success(( (fst tuple) , {(snd tuple) with noise=v} ), [])

    let MakeDto (prams:IDictionary<string,Param>) =
        AddNoise >>= 
            (AddStepSize >>= 
                (AddNodeCount >>= 
                    (AddEnsembleCount >>= (AddStartSeed >>= (InitCesrDto prams)))))

    let MakeSimpleNetwork (prams:IDictionary<string,Param>) =
        let dtoBuilder dto = 
            let sn = SimpleNetwork.CreateRandCesr 
                            dto.startSeed 
                            dto.ensembleCount, 
                            dto.nodeCount
            sn

        match (MakeDto prams) with
            | Failure f -> Rop.fail f
            | Success ((p,dto),m) -> Rop.succeed ( dtoBuilder dto)
