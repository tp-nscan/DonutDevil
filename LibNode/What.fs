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
        | si, sj when (si>=0.0f) && (sj< 0.0f) -> si*sj
        | si, sj when (si>=0.0f) && (sj>=0.0f) -> 0.0f
        | _, _     -> failwith  "cant happen"

    let BAadj si sj =
        match si, sj with
        | si, sj when (si< 0.0f) && (sj< 0.0f) -> 0.0f
        | si, sj when (si< 0.0f) && (sj>=0.0f) -> si*sj
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

    member this.Update() =
        let dPdR = this.mR.Map2((fun x y -> x * y * this.cRp), this.mS)
        let dAdA = this.mA.Multiply(this.mAa)
        let dAdB = this.mB.Multiply(this.mBa)
        let dBdA = this.mA.Multiply(this.mAb)
        let dBdB = this.mB.Multiply(this.mBb)
        let dSdS = this.mS.Multiply(this.mSs)
        let sNoise = this.sNoise |> Seq.take this.mS.ColumnCount |> Seq.toArray
        let aNoise = this.pNoise |> Seq.take this.mS.ColumnCount |> Seq.toArray
        let bNoise = this.pNoise |> Seq.take this.mS.ColumnCount |> Seq.toArray
        
        let newA = aNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + this.mA.[0,i] + this.cPp * (dAdA.[0,i] + dAdB.[0,i]) 
                      + dPdR.[0,i] ))

        let newB = bNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + this.mB.[0,i] + this.cPp * (dBdB.[0,i] + dBdA.[0,i]) 
                      - dPdR.[0,i] ))

        let newS = sNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + this.mS.[0,i] + this.cSs * dSdS.[0,i] + 
                      this.cPs * this.mR.[0,i]  * (this.mA .[0,i] 
                      - this.mB.[0,i]) ))

        new Wng(
            iteration = this.Iteration,
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


    member this.Update2() =
        let dPdR = this.mR.Map2((fun x y -> x * y * this.cRp), this.mS)
        let dAdA = this.mA.Multiply(this.mAa)
        let dAdB = this.mB.Multiply(this.mBa)
        let dBdA = this.mA.Multiply(this.mAb)
        let dBdB = this.mB.Multiply(this.mBb)
        let dSdS = this.mS.Multiply(this.mSs)
        let sNoise = this.sNoise |> Seq.take this.mS.ColumnCount |> Seq.toArray
        let aNoise = this.pNoise |> Seq.take this.mS.ColumnCount |> Seq.toArray
        let bNoise = this.pNoise |> Seq.take this.mS.ColumnCount |> Seq.toArray
        
        let newA = aNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + this.mA.[0,i] + this.cPp * (dAdA.[0,i] + dAdB.[0,i]) 
                      + dPdR.[0,i] ))

        let newB = bNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + this.mB.[0,i] + this.cPp * (dBdB.[0,i] + dBdA.[0,i]) 
                      - dPdR.[0,i] ))

        let newS = sNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + this.mS.[0,i] + this.cSs * dSdS.[0,i] + 
                      this.cPs * this.mR.[0,i]  * (this.mA .[0,i] 
                      - this.mB.[0,i]) ))

        new Wng(
            iteration = this.Iteration,
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
            rM = this.mR,
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
                            (FpFrTpTr this.mAa.[0,i] this.mR.[0,i] this.mAa.[0,j] this.mR.[0,j])
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
                            (FpFrTpTr this.mAa.[0,i] this.mR.[0,i] this.mAa.[0,j] this.mR.[0,j])
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

    member this.Learn2(learnRate:float32) =
        
        let aamNew = 
            DenseMatrix.init 
                this.mAa.RowCount this.mAa.ColumnCount
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            aaM.[i,j] +          
                            (AAadj this.mS.[0,i] this.mS.[0,j]) *
                            learnRate *
                            (FpFrTpTr this.mAa.[0,i] this.mR.[0,i] this.mAa.[0,j] this.mR.[0,j])
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