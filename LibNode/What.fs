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

type WhatNodesGpu(cp:Matrix<float32>,
                  cs:Matrix<float32>,
                  a:Vector<float32>,
                  b:Vector<float32>,
                  s:Vector<float32>,
                  r:Vector<float32>,                          
                  iteration:int,
                  stepC:float32,
                  stepS:float32,
                  stepR:float32,
                  pOs:float32,
                  seqNoise:seq<float32>) =

    let _cp = cp
    let _cs = cs
    let _a = a
    let _b = b
    let _s = s
    let _r = r
    let _pOs = pOs
    let _iteration = iteration
    let _stepC = stepC
    let _stepS = stepS
    let _stepR = stepR
    let _seqNoise = seqNoise

    member this.Update() =
        new WhatNodesGpu(
            cp = _cp,
            cs = _cs,
            a = _a,
            b = _b,
            s = _s,
            r = _r,                          
            iteration = _iteration + 1,
            stepC = _stepC,
            stepS = _stepS,
            stepR = _stepR,
            pOs = _pOs,
            seqNoise = _seqNoise
        )

    member this.Learn(learnRate:float32) =
        new WhatNodesGpu(
            cp = _cp,
            cs = _cs,
            a = _a,
            b = _b,
            s = _s,
            r = _r,                          
            iteration = _iteration + 1,
            stepC = _stepC,
            stepS = _stepS,
            stepR = _stepR,
            pOs = _pOs,
            seqNoise = _seqNoise
        )


 module WhatNodeBuilder =

    let CreateRandom((seed:int), ngSize, cSig, pSig, sSig, 
                      noiseLevel, stepC, stepS, stepR, pOs,
                      glauberRadius) =

        let rng = Random.MersenneTwister(seed)

        let cpvs = Generators.NormalSF32 rng cSig
                    |> Seq.take(ngSize*ngSize) |> Seq.toArray
        let cpm = (DenseMatrix.init ngSize ngSize 
                    (fun x y -> if (x=y) then 0.0f 
                                         else cpvs.[x + y*ngSize]))
        
        let aVec = MathNet.Numerics.LinearAlgebra.Vector.Build.DenseOfEnumerable
                    (Generators.NormalSF32 rng pSig
                      |> Seq.take(ngSize) )

        let bVec = MathNet.Numerics.LinearAlgebra.Vector.Build.DenseOfEnumerable
                    (Generators.NormalSF32 rng pSig
                      |> Seq.take(ngSize) )

        let sVec = MathNet.Numerics.LinearAlgebra.Vector.Build.DenseOfEnumerable
                    (Generators.NormalSF32 rng sSig
                      |> Seq.take(ngSize) )

        let rVec = MathNet.Numerics.LinearAlgebra.Vector.Build.DenseOfEnumerable
                    (Generators.SeqOfRandSF32Bits 0.5 rng
                      |> Seq.take(ngSize) )

        let seqNoise = Generators.SeqOfRandSF32 noiseLevel rng

        match GlauberNeutralDense ngSize glauberRadius with
        | Some csMatrix -> Some (new WhatNodesGpu(
                                  cp=cpm,
                                  cs=csMatrix,
                                  a=aVec,
                                  b=bVec,
                                  s=sVec,
                                  r=rVec,                          
                                  iteration=0,
                                  stepC=stepC,
                                  stepS=stepS,
                                  stepR=stepR,
                                  pOs=pOs,
                                  seqNoise=seqNoise
                            ))
        | None -> None