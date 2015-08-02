namespace LibNode
open System
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open Rop
open MathUtils
open EntityOps
open MathNetUtils
open Glauber

module WhatUtils = 

    let FpFrTpTr (fp:float32) fr tp tr = 
        if (fp*fr < 0.0f) then 0.0f else
        (fr*tr - fp*tp)

    let AAadj si sj =
        match si, sj with
        | si, sj when (si< 0.0f) && (sj< 0.0f) -> 0.0f
        | si, sj when (si< 0.0f) && (sj>=0.0f) -> 0.0f
        | si, sj when (si>=0.0f) && (sj< 0.0f) -> 0.0f
        | si, sj when (si>=0.0f) && (sj>=0.0f) -> si*sj
        | _, _     -> failwith  "cant happen"

    let ABadj si sj =
        match si, sj with
        | si, sj when (si< 0.0f) && (sj< 0.0f) -> 0.0f
        | si, sj when (si< 0.0f) && (sj>=0.0f) -> 0.0f
        | si, sj when (si>=0.0f) && (sj< 0.0f) -> -si*sj
        | si, sj when (si>=0.0f) && (sj>=0.0f) -> 0.0f
        | _, _     -> failwith  "cant happen"

    let BAadj si sj =
        match si, sj with
        | si, sj when (si< 0.0f) && (sj< 0.0f) -> 0.0f
        | si, sj when (si< 0.0f) && (sj>=0.0f) -> -si*sj
        | si, sj when (si>=0.0f) && (sj< 0.0f) -> 0.0f
        | si, sj when (si>=0.0f) && (sj>=0.0f) -> 0.0f
        | _, _     -> failwith  "cant happen"

    let BBadj si sj =
        match si, sj with
        | si, sj when (si< 0.0f) && (sj< 0.0f) -> si*sj
        | si, sj when (si< 0.0f) && (sj>=0.0f) -> 0.0f
        | si, sj when (si>=0.0f) && (sj< 0.0f) -> 0.0f
        | si, sj when (si>=0.0f) && (sj>=0.0f) -> 0.0f
        | _, _     -> failwith  "cant happen"

open WhatUtils

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

    member this.Iteration = iteration
    member this.GroupCount = rM.ColumnCount
    member this.mAa = aaM
    member this.mAb = abM
    member this.mBa = baM
    member this.mBb = bbM
    member this.mA = aM
    member this.mB = bM
    member this.mS = sM
    member this.mR = rM
    member this.mSs = ssM
    member this.cPp = cPp
    member this.cSs = cSs
    member this.cRp = cRp
    member this.cPs = cPs
    member this.pNoise = nP
    member this.sNoise = nS
    member this.mV = DenseMatrix.init 1 this.mS.ColumnCount  
                        (fun x y -> let sw = this.mS.[0,y]
                                    match sw with
                                    | v when v<0.0f -> -v * this.mB.[0,y]
                                    | v -> v* this.mA.[0,y])

    member this.NewPrams(
                        cPp:float32,
                        cSs:float32,
                        cRp:float32,
                        cPs:float32
                     ) =
        new Wng(
            iteration = this.Iteration,
            aaM = this.mAa,
            abM = this.mAb,
            baM = this.mBa,
            bbM = this.mBb,
            aM = this.mA,
            bM = this.mB,
            sM = this.mS,
            rM = this.mR,
            ssM = this.mSs,
            cPp = cPp,
            cSs = cSs,
            cRp = cRp,
            cPs = cPs,
            nP = this.pNoise,
            nS = this.sNoise
        )

    member this.Update() =
        let stride = this.mS.ColumnCount 
        let sNoise = this.sNoise |> Seq.take this.mS.ColumnCount |> Seq.toArray
        let aNoise = this.pNoise |> Seq.take this.mS.ColumnCount |> Seq.toArray
        let bNoise = this.pNoise |> Seq.take this.mS.ColumnCount |> Seq.toArray

        let dAdR = this.mR.Map2((fun x y -> x * (1.0f+y) * (1.0f+y) * this.cRp), this.mS)
        let dBdR = this.mR.Map2((fun x y -> x * (1.0f-y) * (1.0f-y) * this.cRp), this.mS)

        let fcM = DenseMatrix.init 
                   stride stride  
                   (UpperTriangulateZd 
                     stride ( fun x y -> let sqr = (1.0f + this.mS.[0,x] * this.mS.[0,y])
                                         sqr*sqr ))

        let faM = DenseMatrix.init 
                   stride stride  
                   (UpperTriangulateZd 
                     stride ( fun x y -> let sqr = (1.0f - this.mS.[0,x] * this.mS.[0,y])
                                         sqr*sqr ))

        let mAas = this.mAa.Map2 ((fun a b -> a*b), fcM)

        let mBas = this.mBa.Map2 ((fun a b -> a*b), faM)

        let mAbs = this.mAb.Map2 ((fun a b -> a*b), faM)

        let mBbs = this.mBb.Map2 ((fun a b -> a*b), fcM)


        let dAdA = this.mA.Multiply(mAas)
        let dAdB = this.mB.Multiply(mBas)
        let dBdA = this.mA.Multiply(mAbs)
        let dBdB = this.mB.Multiply(mBbs)


        let newA = aNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + this.mA.[0,i] + this.cPp * (dAdA.[0,i] + dAdB.[0,i]) 
                      + dAdR.[0,i] ))

        let newB = aNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + this.mB.[0,i] + this.cPp * (dBdB.[0,i] + dBdA.[0,i]) 
                      + dBdR.[0,i] ))


        let dSdS = this.mS.Multiply(this.mSs)
        let newS = sNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + this.mS.[0,i] + this.cSs * dSdS.[0,i] + 
                      this.cPs * this.mR.[0,i]  * (this.mA .[0,i] 
                      - this.mB.[0,i]) ))

        new Wng(
            iteration = this.Iteration + 1,
            aaM = this.mAa,
            abM = this.mAb,
            baM = this.mBa,
            bbM = this.mBb,
            aM = DenseMatrix.init 1 this.mS.ColumnCount  
                    (fun x y -> newA.[x * this.mA.ColumnCount + y]),
            bM = DenseMatrix.init 1 this.mS.ColumnCount  
                    (fun x y -> newB.[x * this.mB.ColumnCount + y]),
            sM = DenseMatrix.init 1 this.mS.ColumnCount  
                    (fun x y -> newS.[x * this.mS.ColumnCount + y]),
            rM = this.mR ,
            ssM = this.mSs,
            cPp = this.cPp,
            cSs = this.cSs,
            cRp = this.cRp,
            cPs = this.cPs,
            nP = this.pNoise,
            nS = this.sNoise
        )


    member this.Learn(learnRate:float32) =
        
        let aamNew = 
            DenseMatrix.init 
                this.mAa.RowCount this.mAa.ColumnCount
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            aaM.[i,j] +          
                            (AAadj this.mS.[0,i] this.mS.[0,j]) *
                            learnRate *
                            (FpFrTpTr this.mA.[0,i] this.mR.[0,i] this.mA.[0,j] this.mR.[0,j])
                        )
                )

        let abmNew = 
            DenseMatrix.init 
                this.mAb.RowCount this.mAb.ColumnCount
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            this.mAb.[i,j] +          
                            (ABadj this.mS.[0,i] this.mS.[0,j]) *
                            learnRate *
                            (FpFrTpTr this.mA.[0,i] this.mR.[0,i] this.mB.[0,j] this.mR.[0,j])
                        )
                )
        
        let bamNew = 
            DenseMatrix.init 
                this.mBa.RowCount this.mBa.ColumnCount
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            this.mBa.[i,j] +          
                            (BAadj this.mS.[0,i] this.mS.[0,j]) *
                            learnRate *
                            (FpFrTpTr this.mB.[0,i] this.mR.[0,i] this.mA.[0,j] this.mR.[0,j])
                        )
                )

        let bbmNew = 
            DenseMatrix.init 
                this.mBb.RowCount this.mBb.ColumnCount
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            this.mBb.[i,j] +          
                            (BBadj this.mS.[0,i] this.mS.[0,j]) *
                            learnRate *
                            (FpFrTpTr this.mB.[0,i] this.mR.[0,i] this.mB.[0,j] this.mR.[0,j])
                        )
                )

        new Wng(
            iteration = this.Iteration,
            aaM = aamNew,
            abM = abmNew,
            baM = bamNew,
            bbM = bbmNew,
            aM = this.mA,
            bM = this.mB,
            sM = this.mS,
            rM = this.mR,
            ssM = this.mSs,
            cPp = this.cPp,
            cSs = this.cSs,
            cRp = this.cRp,
            cPs = this.cPs,
            nP = this.pNoise,
            nS = this.sNoise
        )

    member this.LearnSymmetric(learnRate:float32) =
        
        let aamNew = 
            DenseMatrix.init 
                this.mAa.RowCount this.mAa.ColumnCount
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            aaM.[i,j] +          
                            (AAadj this.mS.[0,i] this.mS.[0,j]) *
                            learnRate *
                            this.mAa.[0,i] * this.mAa.[0,j]
                        )
                )

        let abmNew = 
            DenseMatrix.init 
                this.mAb.RowCount this.mAb.ColumnCount
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            this.mAb.[i,j] +          
                            (ABadj this.mS.[0,i] this.mS.[0,j]) *
                            learnRate *
                            this.mAa.[0,i] * this.mB.[0,j]
                        )
                )
        
        let bamNew = 
            DenseMatrix.init 
                this.mBa.RowCount this.mBa.ColumnCount
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            this.mBa.[i,j] +          
                            (BAadj this.mS.[0,i] this.mS.[0,j]) *
                            learnRate *
                            this.mB.[0,i] * this.mAa.[0,j]
                        )
                )

        let bbmNew = 
            DenseMatrix.init 
                this.mBb.RowCount this.mBb.ColumnCount
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            this.mBb.[i,j] +          
                            (BBadj this.mS.[0,i] this.mS.[0,j]) *
                            learnRate *
                            this.mB.[0,i] * this.mB.[0,j]
                        )
                )

        new Wng(
            iteration = this.Iteration,
            aaM = aamNew,
            abM = abmNew,
            baM = bamNew,
            bbM = bbmNew,
            aM = this.mA,
            bM = this.mB,
            sM = this.mS,
            rM = this.mR,
            ssM = this.mSs,
            cPp = this.cPp,
            cSs = this.cSs,
            cRp = this.cRp,
            cPs = this.cPs,
            nP = this.pNoise,
            nS = this.sNoise
        )

type Waffle(
            aaM:Matrix<float32>,
            abM:Matrix<float32>,
            baM:Matrix<float32>,
            bbM:Matrix<float32>,
            reM:Matrix<float32>
        ) =
    member this.GroupCount = reM.ColumnCount
    member this.EnsembleCount = reM.RowCount
    member this.mAa = aaM
    member this.mAb = abM
    member this.mBa = baM
    member this.mBb = bbM
    member this.meR = reM

type WaffleHistories = {aeR:ArrayHist; ahV:ArrayHist; ahA:ArrayHist; ahB:ArrayHist; 
                        ahR:ArrayHist; ahS:ArrayHist;}


 module WngBuilder =
    
    let CreateRandom((seed:int), ngSize, ppSig, pSig, sSig,
                      pNoiseLevel, sNoiseLevel, cPp, cSs, cRp, cPs,
                      glauberRadius) =

        let rng = Random.MersenneTwister(seed)

        let mAa = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD

        let mAb = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD

        let mBb = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD
        
        let temp = Generators.NormalSF32 rng pSig
                       |> Seq.take(ngSize) |> Seq.toArray
        let mA = DenseMatrix.init 1 ngSize
                    (fun x y -> temp.[x*ngSize + y])

        let temp = Generators.NormalSF32 rng pSig
                       |> Seq.take(ngSize) |> Seq.toArray
        let mB = DenseMatrix.init 1 ngSize
                    (fun x y -> temp.[x*ngSize + y])

        let temp = Generators.NormalSF32 rng sSig
                       |> Seq.take(ngSize) |> Seq.toArray
        let mS = DenseMatrix.init 1 ngSize
                    (fun x y -> temp.[x*ngSize + y])

        let rSqData = Generators.SeqOfRandSF32Bits 0.5 rng
                       |> Seq.take(ngSize) |> Seq.toArray
        let mR = DenseMatrix.init 1 ngSize
                    (fun x y -> rSqData.[x*ngSize + y])

        let pNoise = Generators.SeqOfRandSF32 pNoiseLevel rng
        let sNoise = Generators.SeqOfRandSF32 sNoiseLevel rng

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


    let InitHistories arrayLength ensembleSize targetLength =
      {
        WaffleHistories.aeR=ArrayHistory.Init "C" ensembleSize targetLength
        ahV=ArrayHistory.Init "V" arrayLength targetLength 
        ahA=ArrayHistory.Init "A" arrayLength targetLength 
        ahB=ArrayHistory.Init "B" arrayLength targetLength 
        ahR=ArrayHistory.Init "R" arrayLength targetLength 
        ahS=ArrayHistory.Init "S" arrayLength targetLength 
      }

    let UpdateHistories (waffleHist:WaffleHistories) (wng:Wng) (waffle:Waffle) =
      let fgc = System.Convert.ToSingle(waffle.GroupCount)
      {
        WaffleHistories.aeR=ArrayHistory.Add 
            waffleHist.aeR 
            (wng.mA.TransposeAndMultiply(waffle.meR) |> FlattenRm |> Seq.map(fun v->v/fgc)) 
            wng.Iteration
        ahV=ArrayHistory.Add waffleHist.ahV (wng.mV |> FlattenRm) wng.Iteration
        ahA=ArrayHistory.Add waffleHist.ahA (wng.mA |> FlattenRm) wng.Iteration
        ahB=ArrayHistory.Add waffleHist.ahB (wng.mB |> FlattenRm) wng.Iteration
        ahR=ArrayHistory.Add waffleHist.ahR (wng.mR |> FlattenRm) wng.Iteration
        ahS=ArrayHistory.Add waffleHist.ahS (wng.mS |> FlattenRm) wng.Iteration
      }



 module WaffleBuilder =

    let GetArrayHistories (waffleHistories:WaffleHistories) =
        seq { 
               yield waffleHistories.aeR
               yield waffleHistories.ahA
               yield waffleHistories.ahB
               yield waffleHistories.ahR
               yield waffleHistories.ahS
               yield waffleHistories.ahV
            }

    let CreateRandom (seed:int) ngSize geSize ppSig =

        let rng = Random.MersenneTwister(seed)

        let mAa = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD

        let mAb = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD

        let mBb = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD

        let rSqData = Generators.SeqOfRandSF32Bits 0.5 rng
                       |> Seq.take(ngSize * geSize) 
                       |> Seq.toArray
        let mRe = DenseMatrix.init geSize ngSize 
                    (fun x y -> rSqData.[x*ngSize + y])

        new Waffle(
            aaM = mAa,
            abM = mAb,
            baM = mAb.Transpose(),
            bbM = mBb,
            reM = mRe
        )


    let CreateWng glauberRadius pSig sSig cPp
                  pNoiseLevel sNoiseLevel cSs cRp 
                  cPs rIndex (sseed:int) 
                  (pseed:int) (waffle:Waffle) =

        let rngS = Random.MersenneTwister(sseed)
        let rngP = Random.MersenneTwister(pseed)
        
        let ngSize = waffle.GroupCount

        let temp = Generators.NormalSF32 rngP pSig
                        |> Seq.take(ngSize) |> Seq.toArray
        let mA = DenseMatrix.init 1 ngSize 
                    (fun x y -> temp.[x*ngSize + y])

        let temp = Generators.NormalSF32 rngP pSig
                        |> Seq.take(ngSize) |> Seq.toArray
        let mB = DenseMatrix.init 1 ngSize 
                    (fun x y -> temp.[x*ngSize + y])

        let temp = Generators.NormalSF32 rngS sSig
                        |> Seq.take(ngSize) |> Seq.toArray
        let mS = DenseMatrix.init 1 ngSize 
                    (fun x y -> temp.[x*ngSize + y])

        let pNoise = Generators.SeqOfRandSF32 pNoiseLevel rngP
        let sNoise = Generators.SeqOfRandSF32 sNoiseLevel rngS

        match GlauberNeutralDense ngSize glauberRadius with
        | Some csMatrix -> 
            Some ( 
                new Wng(
                        iteration = 0,
                        aaM = waffle.mAa,
                        abM = waffle.mAb,
                        baM = waffle.mBa,
                        bbM = waffle.mBb,
                        aM = mA,
                        bM = mB,
                        sM = mS,
                        rM = waffle.meR.SubMatrix(rIndex, 1, 0, ngSize),
                        ssM = csMatrix,
                        cPp = cPp,
                        cSs = cSs,
                        cRp = cRp,
                        cPs = cPs,
                        nP = pNoise,
                        nS = sNoise
                ))
        | None -> None



    let ResetP pSig (pseed:int) (wng:Wng) =

        let rngP = Random.MersenneTwister(pseed)
        let ngSize = wng.GroupCount

        let temp = Generators.NormalSF32 rngP pSig
                        |> Seq.take(ngSize) |> Seq.toArray
        let mA = DenseMatrix.init 1 ngSize 
                    (fun x y -> temp.[x*ngSize + y])

        let temp = Generators.NormalSF32 rngP pSig
                        |> Seq.take(ngSize) |> Seq.toArray
        let mB = DenseMatrix.init 1 ngSize 
                    (fun x y -> temp.[x*ngSize + y])

        new Wng(
                iteration = 0,
                aaM = wng.mAa,
                abM = wng.mAb,
                baM = wng.mBa,
                bbM = wng.mBb,
                aM = mA,
                bM = mB,
                sM = wng.mS,
                rM = wng.mR,
                ssM = wng.mSs,
                cPp = wng.cPp,
                cSs = wng.cSs,
                cRp = wng.cRp,
                cPs = wng.cPs,
                nP = wng.pNoise,
                nS = wng.sNoise
        )

    let ResetC (waffle:Waffle) (wng:Wng) =

        new Wng(
                iteration = 0,
                aaM = waffle.mAa,
                abM = waffle.mAb,
                baM = waffle.mBa,
                bbM = waffle.mBb,
                aM = wng.mA,
                bM = wng.mB,
                sM = wng.mS,
                rM = wng.mR,
                ssM = wng.mSs,
                cPp = wng.cPp,
                cSs = wng.cSs,
                cRp = wng.cRp,
                cPs = wng.cPs,
                nP = wng.pNoise,
                nS = wng.sNoise
        )

    let ResetR (waffle:Waffle) (memDex:int) (wng:Wng) =
        new Wng(
                iteration = 0,
                aaM = wng.mAa,
                abM = wng.mAb,
                baM = wng.mBa,
                bbM = wng.mBb,
                aM = wng.mA,
                bM = wng.mB,
                sM = wng.mS,
                rM = waffle.meR.SubMatrix(memDex, 1, 0, waffle.GroupCount),
                ssM = wng.mSs,
                cPp = wng.cPp,
                cSs = wng.cSs,
                cRp = wng.cRp,
                cPs = wng.cPs,
                nP = wng.pNoise,
                nS = wng.sNoise
        )

    let ResetS sSig (sseed:int) (wng:Wng) =
    
        let rngS = Random.MersenneTwister(sseed)
        let ngSize = wng.GroupCount

        let temp = Generators.NormalSF32 rngS sSig
                        |> Seq.take(ngSize) |> Seq.toArray
        let mS = DenseMatrix.init 1 ngSize 
                    (fun x y -> temp.[x*ngSize + y])

        new Wng(
                iteration = 0,
                aaM = wng.mAa,
                abM = wng.mAb,
                baM = wng.mBa,
                bbM = wng.mBb,
                aM = wng.mA,
                bM = wng.mB,
                sM = mS,
                rM = wng.mR,
                ssM = wng.mSs,
                cPp = wng.cPp,
                cSs = wng.cSs,
                cRp = wng.cRp,
                cPs = wng.cPs,
                nP = wng.pNoise,
                nS = wng.sNoise
                )


    let UpdateFromWng (waffle:Waffle) (wng:Wng) =
        new Waffle(
            aaM = wng.mAa,
            abM = wng.mAb,
            baM = wng.mBa,
            bbM = wng.mBb,
            reM = waffle.meR
        )