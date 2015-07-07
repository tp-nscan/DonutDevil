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
open MathNetUtils
open Glauber

type Wng(
            iteration:int,
            aaM:Matrix<float32>,
            abM:Matrix<float32>,
            baM:Matrix<float32>,
            bbM:Matrix<float32>,
            aM:Matrix<float32>,
            bM:Matrix<float32>,
            sM:Matrix<float32>,
            rM:Matrix<float32>,
            ssM:Matrix<float32>,
            cPp:float32,
            cSs:float32,
            cRp:float32,
            cPs:float32,
            nP:seq<float32>,
            nS:seq<float32>
        ) =

    let _iteration = iteration
    let _aaM = aaM
    let _abM = abM
    let _baM = baM
    let _bbM = bbM
    let _aM = aM
    let _bM = bM
    let _sM = sM
    let _rM = rM
    let _ssM = ssM
    let _cPp = cPp
    let _cSs = cSs
    let _cRp = cRp
    let _cPs = cPs
    let _nP = nP
    let _nS = nS

    member this.Update() =

        let dPdR = _rM.Map2((fun x y -> x * y * _cRp), _sM)
        let dAdA = _aM.Multiply(_aaM)
        let dAdB = _bM.Multiply(_baM)
        let dBdA = _aM.Multiply(_abM)
        let dBdB = _bM.Multiply(_bbM)
        let dSdS = _sM.Multiply(_ssM)
        let sNoise = _nS |> Seq.take _sM.ColumnCount |> Seq.toArray
        let aNoise = _nS |> Seq.take _sM.ColumnCount |> Seq.toArray
        let bNoise = _nS |> Seq.take _sM.ColumnCount |> Seq.toArray
        
        let newA = aNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + _aM.[0,i] + _cPp*(dAdA.[0,i] + dAdB.[0,i]) 
                      + dPdR.[0,i] ))

        let newB = bNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + _bM.[0,i] + _cPp*(dBdB.[0,i] + dBdA.[0,i]) 
                      - dPdR.[0,i] ))

        let newS = sNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + _sM.[0,i] + _cSs*dSdS.[0,i] + 
                      _cPs * _rM.[0,i]  * (_aM.[0,i] - _bM.[0,i]) ))

        new Wng(
            iteration = _iteration,
            aaM = _aaM,
            abM = _abM,
            baM = _baM,
            bbM = _bbM,
            aM = DenseMatrix.init 1 _sM.ColumnCount  
                    (fun x y -> newA.[x *_aM.ColumnCount + y]),
            bM = DenseMatrix.init 1 _sM.ColumnCount  
                    (fun x y -> newB.[x *_bM.ColumnCount + y]),
            sM = DenseMatrix.init 1 _sM.ColumnCount  
                    (fun x y -> newS.[x *_sM.ColumnCount + y]),
            rM = _rM,
            ssM = _ssM,
            cPp = _cPp,
            cSs = _cSs,
            cRp = _cRp,
            cPs = _cPs,
            nP = _nP,
            nS = _nS
        )

        member this.Learn(learnRate:float32) =

            let delAtoA = _aM.Transpose().Multiply(_aM)
            let delBToA = _aM.Transpose().Multiply(_bM)
            let delBToB = _bM.Transpose().Multiply(_bM)

            new Wng(
                iteration = _iteration,
                aaM = _aaM,
                abM = _abM,
                baM = _baM,
                bbM = _bbM,
                aM = _aM,
                bM = _bM,
                sM = _sM,
                rM = _rM,
                ssM = _ssM,
                cPp = _cPp,
                cSs = _cSs,
                cRp = _cRp,
                cPs = _cPs,
                nP = _nP,
                nS = _nS
            )

 module WngBuilder =
    

    let CreateRandom((seed:int), ngSize, ppSig, pSig, sSig, 
                      pNoise, sNoise, cPp, cSs, cRp, cPs,
                      glauberRadius) =

        let rng = Random.MersenneTwister(seed)

        let mAa = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD

        let mAb = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD

        let mBb = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD
        

        let rSqData = Generators.NormalSF32 rng pSig
                       |> Seq.take(ngSize) |> Seq.toArray
        let mA = DenseMatrix.init 1 ngSize 
                    (fun x y -> rSqData.[x*ngSize + y])

        let rSqData = Generators.NormalSF32 rng pSig
                       |> Seq.take(ngSize) |> Seq.toArray
        let mB = DenseMatrix.init 1 ngSize 
                    (fun x y -> rSqData.[x*ngSize + y])

        let rSqData = Generators.NormalSF32 rng sSig
                       |> Seq.take(ngSize) |> Seq.toArray
        let mS = DenseMatrix.init 1 ngSize 
                    (fun x y -> rSqData.[x*ngSize + y])


        let rSqData = Generators.SeqOfRandSF32Bits 0.5 rng
                       |> Seq.take(ngSize) |> Seq.toArray
        let mR = DenseMatrix.init 1 ngSize 
                    (fun x y -> rSqData.[x*ngSize + y])

        let pNoise = Generators.SeqOfRandSF32 pNoise rng
        let sNoise = Generators.SeqOfRandSF32 sNoise rng

        match GlauberNeutralDense ngSize glauberRadius with
        | Some csMatrix -> 
            Some ( 
                new Wng(
                            iteration = 0,
                            aaM = mAa,
                            abM = mAb,
                            baM = mAb.Transpose(),
                            bbM = mBb,
                            aM = mA,
                            bM = mB,
                            sM = mS,
                            rM = mR,
                            ssM = csMatrix,
                            cPp = cPp,
                            cSs = cSs,
                            cRp = cRp,
                            cPs = cPs,
                            nP = pNoise,
                            nS = sNoise
                    ))
        | None -> None

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