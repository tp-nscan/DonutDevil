namespace LibNode
open System
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open MathUtils

open Rop

type StateCount = StateCount of int
type EnsembleCount = EnsembleCount of int

type SymReportShape =
    | CliqueSet of EnsembleCount * StateCount
    | DenseConnections of StateCount

type ISymState =
    abstract member Update: unit -> RopResult<ISymState,string>
    abstract member FloatArrays: INamed<float32[]>[]

type Ca = { States:Matrix<float32>; Connections:Matrix<float32> }

type FullCliqueDto = {  id:Guid; parentId:Option<Guid>; States:Matrix<float32>; 
                        Connections:Matrix<float32>; stepSize:float32; noiseSeed:int; 
                        noiseLevel:float32 }


type FullClique private (states: Matrix<float32>, connections: Matrix<float32>, 
                         stepsize:float32, seqNoise:seq<float32>) =
    let _states = states
    let _connections = connections
    let _stepsize = stepsize
    let _seqNoise = seqNoise

    new(dto:FullCliqueDto) = 
        let seqNoise = (Generators.RandF32s dto.noiseSeed dto.noiseLevel)
        FullClique(dto.States, dto.Connections, dto.stepSize, seqNoise)

    interface ISymState with
        member this.Update() =
            let randIter = Generators.SeqIter _seqNoise
            let updated = _states.Multiply _connections
            let newStates = _states.Map2((fun x y -> MathUtils.BoundUnitSF32( x + y * stepsize + randIter())), updated)
            RopResult.Success(new FullClique( newStates, _connections, _stepsize, _seqNoise) :> ISymState , [])

        member this.FloatArrays = [| 
                new NamedDataPtr<float32[]>("Connections", fun () -> (_connections.ToArray() |> FlattenColumnMajor) );
                new NamedDataPtr<float32[]>("States", fun () -> (_states.ToArray() |> FlattenColumnMajor)) 
            |]


module CesrBuilder =

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

    let CreateDtoFromRandomParams (prams:Param list) =

        let ConvertDto (dto:RandomCesrDto) = 
            let randIter = Generators.SeqIter (Generators.RandF32s dto.cnxnSeed 1.0f)
            { 
              FullCliqueDto.id = dto.id; 
              parentId = None;
              States = DenseMatrix.init dto.ensembleCount dto.nodeCount (fun i j -> randIter()); 
              Connections = DenseMatrix.init dto.nodeCount dto.nodeCount (fun i j -> randIter()) ; 
              stepSize = dto.stepSize;
              noiseSeed = dto.noiseSeed;
              noiseLevel = dto.noiseLevel
            }

        let dtoResult = CreateRandomDto   
                            <!> (Parameters.GetGuidParam prams "Id")
                            <*> (Parameters.GetIntParam prams "NodeCount")
                            <*> (Parameters.GetIntParam prams "EnsembleCount")
                            <*> (Parameters.GetFloat32Param prams "StepSize")
                            <*> (Parameters.GetIntParam prams "NoiseSeed")
                            <*> (Parameters.GetFloat32Param prams "NoiseLevel")
                            <*> (Parameters.GetIntParam prams "CnxnSeed")

        match dtoResult with
              | Success (x, msgs) -> RopResult.Success ((ConvertDto x), msgs)
              | Failure errors -> Failure errors

    let MakeSimpleNetworkFromRandomParams (prams:Param list) =
        match (CreateDtoFromRandomParams prams) with
              | Success (x, msgs) -> RopResult.Success (FullClique(x), msgs)
              | Failure errors -> Failure errors


