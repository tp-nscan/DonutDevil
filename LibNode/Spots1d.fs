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
open Glauber

type Spots1dGpu(  ppM:Matrix<float32>,
                  ssM:Matrix<float32>,
                  a:Matrix<float32>,
                  s:Matrix<float32>,
                  r:Matrix<float32>,                          
                  iteration:int,
                  stepC:float32,
                  stepS:float32,
                  stepR:float32,
                  seqNoise:seq<float32>
                ) =

    let _ppM = ppM
    let _ssM = ssM
    let _a = a
    let _s = s
    let _r = r
    let _iteration = iteration
    let _stepC = stepC
    let _stepS = stepS
    let _stepR = stepR
    let _seqNoise = seqNoise

    member this.Update() =
       try
        let randIter = Generators.SeqIter _seqNoise
        let ssInc = _s.Multiply _ssM
        let sInc =
            _r.Map2((fun x y -> F32ToSF32(x + y * stepS + randIter())), 
                            ssInc)
        let newS = _s.Map2((fun x y -> F32ToSF32(x + y)), sInc)

        Some (new Spots1dGpu(
                    ppM = _ppM,
                    ssM = _ssM,
                    a = _a,
                    s = newS,
                    r = _r,                          
                    iteration = _iteration + 1,
                    stepC = _stepC,
                    stepS = _stepS,
                    stepR = _stepR,
                    seqNoise = _seqNoise
                ))
       with 
        | ex -> None


    member this.Learn(learnRate:float32) =
        new Spots1dGpu(
            ppM = _ppM,
            ssM = _ssM,
            a = _a,
            s = _s,
            r = _r,                          
            iteration = _iteration + 1,
            stepC = _stepC,
            stepS = _stepS,
            stepR = _stepR,
            seqNoise = _seqNoise
        )


 module Spots1dBuilder =

    let CreateRandom( seed:int, ngSize, cSig:float, pSig:float, 
                      noiseLevel:float, stepC:float, stepS:float, 
                      stepR:float, glauberRadius) =

        let rng = Random.MersenneTwister(seed)

        let cpvs = Generators.NormalSF32 rng cSig
                    |> Seq.take(ngSize*ngSize) |> Seq.toArray

        let cpm = DenseMatrix.init ngSize ngSize 
                    (fun x y -> if (x=y) then 0.0f 
                                         else cpvs.[x + y*ngSize])
        
        let avals = Generators.NormalSF32 rng cSig
                    |> Seq.take(ngSize) |> Seq.toArray
        let aMatrix = DenseMatrix.init 1 ngSize (fun x y -> avals.[y])


        let svals = Generators.NormalSF32 rng cSig
                    |> Seq.take(ngSize) |> Seq.toArray
        let sMatrix = DenseMatrix.init 1 ngSize (fun x y -> svals.[y])

        let stepR32 = System.Convert.ToSingle(stepR)
        let rvals = (Generators.SeqOfRandSF32Bits 0.5f rng
                      |> Seq.take(ngSize)) |> Seq.toArray
        let rMatrix = DenseMatrix.init 1 ngSize (fun x y -> rvals.[y] * stepR32)

        let seqNoise = Generators.SeqOfRandSF32 
                            (System.Convert.ToSingle(noiseLevel)) rng


        match GlauberNeutralDense ngSize glauberRadius with
        | Some ssMatrix ->
            let sC = System.Convert.ToSingle(stepC)
            let sS = System.Convert.ToSingle(stepS)
            let sR = System.Convert.ToSingle(stepR)
            let nCount = System.Convert.ToSingle(ngSize)
            let sCr = sC / nCount
            let sSr = sS / nCount

            Some (  new Spots1dGpu(
                            ppM=cpm,
                            ssM=ssMatrix,
                            a=aMatrix,
                            s=sMatrix,
                            r=rMatrix,                          
                            iteration=0,
                            stepC=sC,
                            stepS=sS,
                            stepR=sR,
                            seqNoise=seqNoise
                    ))
        | None -> None

