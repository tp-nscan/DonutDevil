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

    type ICliqueEnsemble =
        abstract member States: unit -> float32[,]
        abstract member StepSize: float32
        abstract member Generation: int
        abstract member Update: unit -> ICliqueEnsemble

    type CliqueEnsemble(states:Matrix<float32>, connections:Matrix<float32>, stepSize:float32, generation:int, rng:Random) =
        let _rng = rng
        let _states = states
        let _connections = connections
        let _stepSize = stepSize
        let _generation = generation
 
//        new(states:float32[,], connections:float32[,], stepSize:float32, seed:int) =
//            let rng = Random.MersenneTwister(seed)
//            let gen = 0
//            let
//            CliqueEnsemble(DenseMatrix<float32>., connections, stepSize, 0, rng)

//    type CliqueEnsemble(states:float32[,], connections:float32[,], stepSize:float32, generation:int) =
//        let _states = states
//        let _connections = connections
//        let _stepSize = stepSize
//        let _generation = generation
//            
//        new(states:float32[,], connections:float32[,], stepSize:float32) =
//            CliqueEnsemble(states, connections, stepSize, 0)




    type Ca = { States:Matrix<float32>; Connections:Matrix<float32> }

    type RandomCesrDto = {id:Guid; startSeed:int; ensembleCount:int; nodeCount:int; 
                          stepSize:float32; noise:float32}
    type CesrDto = {id:Guid; parentId:Option<Guid>; startSeed:Option<int>; States:Matrix<float32>; 
                    Connections:Matrix<float32>; stepSize:float32; noise:float32}

module SimpleNetwork =

    let CreateRandCesr (seed:int) (stateCount:int) (nodeCount:int) =
        let randIter = Generators.SeqPopper (Generators.RandF32s seed OneF32)
        { 
          Ca.States = DenseMatrix.init stateCount nodeCount (fun i j -> randIter()); 
          Connections = DenseMatrix.init nodeCount nodeCount (fun i j -> randIter()) 
        }

    let Step (ca:Ca) (step:float32) =
        let updated = ca.States.Multiply ca.Connections
        ca.States.Map2((fun x y -> BoundUnitSF32(x + y*step)), updated)

    let StepWithNoise (seed:int) (noise:float32) (ca:Ca) (step:float32) =
        let randIter = Generators.SeqPopper (Generators.RandF32s seed noise)

        let updated = ca.States.Multiply ca.Connections
        { 
            Ca.States = ca.States.Map2((fun x y -> MathUtils.BoundUnitSF32( x + y * step + randIter())), updated)
            Connections = ca.Connections
        }


type FullClique (states:Matrix<float32>, connections: Matrix<float32>, generation:int) =
    let _states = states
    let _connections = connections
    let _generation = generation





module CesrDtoBuilder =

    let CreateRandomDto (id:Guid) (startSeed:int) (ensembleCount:int) (nodeCount:int) (stepSize:float32) (noise:float32) =
                  {id=id; startSeed=startSeed; ensembleCount=ensembleCount; nodeCount=nodeCount; stepSize=stepSize; noise=noise}


    let CreateDtoFromPrarams (prams:IDictionary<string, Param>) =

        let ConvertDto (dto:RandomCesrDto) = 
            let randIter = Generators.SeqPopper (Generators.RandF32s dto.startSeed OneF32)
            {   
                CesrDto.id = dto.id; 
                parentId = None; 
                startSeed = Some dto.startSeed;
                States = DenseMatrix.init dto.ensembleCount dto.nodeCount (fun i j -> randIter()); 
                Connections = DenseMatrix.init dto.nodeCount dto.nodeCount (fun i j -> randIter()) ; 
                stepSize = dto.stepSize; noise=dto.noise
             }

        let dtoResult = CreateRandomDto   
                            <!> (Parameters.GetGuidParam prams "Id")
                            <*> (Parameters.GetIntParam prams "StartSeed") 
                            <*> (Parameters.GetIntParam prams "EnsembleCount")
                            <*> (Parameters.GetIntParam prams "NodeCount")
                            <*> (Parameters.GetFloat32Param prams "StepSize")
                            <*> (Parameters.GetFloat32Param prams "Noise")

        match dtoResult with
        | Success (x, msgs) -> RopResult.Success ((ConvertDto x), msgs)
        | Failure errors -> Failure errors


    let MakeSimpleNetwork (prams:IDictionary<string,Param>) =
        None
