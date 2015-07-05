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

type Spots1dGpu(cp:Matrix<float32>,
                  cs:Matrix<float32>,
                  a:Matrix<float32>,
                  r:Vector<float32>,                          
                  iteration:int,
                  stepC:float32,
                  stepS:float32,
                  stepR:float32,
                  seqNoise:seq<float32>) =

    let _cp = cp
    let _cs = cs
    let _a = a
    let _r = r
    let _iteration = iteration
    let _stepC = stepC
    let _stepS = stepS
    let _stepR = stepR
    let _seqNoise = seqNoise
    
    let _wm = _cp.Map2((fun x y -> MathUtils.F32ToSF32(x + y * stepC )), _cs)

    member this.Update() =
       try
        let randIter = Generators.SeqIter _seqNoise
        let updated = _a.Multiply cs
        let newStates = 
            _a.Map2(
                (fun x y -> MathUtils.F32ToSF32(x + y * stepC + randIter())), 
                            updated)


        Some (new Spots1dGpu(
                    cp = _cp,
                    cs = _cs,
                    a = _a,
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
            cp = _cp,
            cs = _cs,
            a = _a,
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

        let rVec = Vector.Build.DenseOfEnumerable
                    (Generators.SeqOfRandSF32Bits 0.5 rng
                      |> Seq.take(ngSize) )

        let seqNoise = Generators.SeqOfRandSF32 noiseLevel rng


        match GlauberNeutralDense ngSize glauberRadius with
        | Some csMatrix ->
            let sC = System.Convert.ToSingle(stepC)
            let sS = System.Convert.ToSingle(stepS)
            let sR = System.Convert.ToSingle(stepR)
            let nCount = System.Convert.ToSingle(ngSize)
            let sCr = sC / nCount
            let sSr = sS / nCount

            let compMatrix = 
                cpm.Map2(
                    (fun x y -> MathUtils.F32ToSF32(x * sCr + y * sSr)), 
                                csMatrix)

            Some (new Spots1dGpu(
                            cp=cpm,
                            cs=csMatrix,
                            a=aMatrix,
                            r=rVec,                          
                            iteration=0,
                            stepC=sC,
                            stepS=sS,
                            stepR=sR,
                            seqNoise=seqNoise
                    ))
        | None -> None

