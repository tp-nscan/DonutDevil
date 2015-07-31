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

type Zeus(
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

type Athena(
            iteration:int,
            aM:Matrix<float32>,
            bM:Matrix<float32>,
            sM:Matrix<float32>
        ) =

    member this.Iteration = iteration
    member this.GroupCount = aM.ColumnCount
    member this.mA = aM
    member this.mB = bM
    member this.mS = sM


type AthenaTr(
                iteration:int,
                aM:Matrix<float32>,
                bM:Matrix<float32>,
                sM:Matrix<float32>,
                dAdR:Matrix<float32>,
                dBdR:Matrix<float32>,
                dAdA:Matrix<float32>,
                dAdB:Matrix<float32>,
                dBdA:Matrix<float32>,
                dBdB:Matrix<float32>,
                dSdS:Matrix<float32>,
                dSdP:Matrix<float32>
            ) =

    member this.Athena = new Athena(iteration, aM, bM, sM)
    member this.dAdR = dAdR
    member this.dBdR = dBdR
    member this.dAdA = dAdA
    member this.dAdB = dAdB
    member this.dBdA = dBdA
    member this.dBdB = dBdB
    member this.dSdS = dSdS
    member this.dSdP = dSdP

module ZeusUtils =

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

    let UpdateTr zeus pNoise (mem:Matrix<float32>) sNoise cPp cSs cRp cPs (athena:Athena) =

        let stride = athena.GroupCount

        let sNoise = sNoise |> Seq.take athena.GroupCount |> Seq.toArray
        let aNoise = pNoise |> Seq.take athena.GroupCount |> Seq.toArray
        let bNoise = pNoise |> Seq.take athena.GroupCount |> Seq.toArray

        let dAdR = mem.Map2((fun x y -> x * (1.0f+y) * (1.0f+y) * cRp), athena.mS)
        let dBdR = mem.Map2((fun x y -> x * (1.0f-y) * (1.0f-y) * cRp), athena.mS)


        let res = new AthenaTr(                
                                iteration=athena.Iteration,
                                aM=athena.mA,
                                bM=athena.mA,
                                sM=athena.mA,
                                dAdR=athena.mA,
                                dBdR=athena.mA,
                                dAdA=athena.mA,
                                dAdB=athena.mA,
                                dBdA=athena.mA,
                                dBdB=athena.mA,
                                dSdS=athena.mA,
                                dSdP=athena.mA)

        None

//
//        let dAdR = this.mR.Map2((fun x y -> x * (1.0f+y) * (1.0f+y) * this.cRp), this.mS)
//        let dBdR = this.mR.Map2((fun x y -> x * (1.0f-y) * (1.0f-y) * this.cRp), this.mS)
//
//        let fcM = DenseMatrix.init 
//                   stride stride  
//                   (UpperTriangulateZd 
//                     stride ( fun x y -> let sqr = (1.0f + this.mS.[0,x] * this.mS.[0,y])
//                                         sqr*sqr ))
//
//        let faM = DenseMatrix.init 
//                   stride stride  
//                   (UpperTriangulateZd 
//                     stride ( fun x y -> let sqr = (1.0f - this.mS.[0,x] * this.mS.[0,y])
//                                         sqr*sqr ))
//
//        let mAas = this.mAa.Map2 ((fun a b -> a*b), fcM)
//
//        let mBas = this.mBa.Map2 ((fun a b -> a*b), faM)
//
//        let mAbs = this.mAb.Map2 ((fun a b -> a*b), faM)
//
//        let mBbs = this.mBb.Map2 ((fun a b -> a*b), fcM)
//
//
//        let dAdA = this.mA.Multiply(mAas)
//        let dAdB = this.mB.Multiply(mBas)
//        let dBdA = this.mA.Multiply(mAbs)
//        let dBdB = this.mB.Multiply(mBbs)
//
//
//        let newA = aNoise |> Array.mapi(fun i n -> 
//            F32ToSF32(n + this.mA.[0,i] + this.cPp * (dAdA.[0,i] + dAdB.[0,i]) 
//                      + dAdR.[0,i] ))
//
//        let newB = aNoise |> Array.mapi(fun i n -> 
//            F32ToSF32(n + this.mB.[0,i] + this.cPp * (dBdB.[0,i] + dBdA.[0,i]) 
//                      + dBdR.[0,i] ))
//
//
//        let dSdS = this.mS.Multiply(this.mSs)
//        let newS = sNoise |> Array.mapi(fun i n -> 
//            F32ToSF32(n + this.mS.[0,i] + this.cSs * dSdS.[0,i] + 
//                      this.cPs * this.mR.[0,i]  * (this.mA .[0,i] 
//                      - this.mB.[0,i]) ))
//
//        new Wng(
//            iteration = this.Iteration + 1,
//            aaM = this.mAa,
//            abM = this.mAb,
//            baM = this.mBa,
//            bbM = this.mBb,
//            aM = DenseMatrix.init 1 this.mS.ColumnCount  
//                    (fun x y -> newA.[x * this.mA.ColumnCount + y]),
//            bM = DenseMatrix.init 1 this.mS.ColumnCount  
//                    (fun x y -> newB.[x * this.mB.ColumnCount + y]),
//            sM = DenseMatrix.init 1 this.mS.ColumnCount  
//                    (fun x y -> newS.[x * this.mS.ColumnCount + y]),
//            rM = this.mR ,
//            ssM = this.mSs,
//            cPp = this.cPp,
//            cSs = this.cSs,
//            cRp = this.cRp,
//            cPs = this.cPs,
//            nP = this.pNoise,
//            nS = this.sNoise
//        )


    let Learn (learnRate:float32) =
        
        None


//        let aamNew = 
//            DenseMatrix.init 
//                this.mAa.RowCount this.mAa.ColumnCount
//                (fun i j 
//                    -> if(i=j) then 0.0f else 
//                        (F32ToSF32
//                            aaM.[i,j] +          
//                            (AAadj this.mS.[0,i] this.mS.[0,j]) *
//                            learnRate *
//                            (FpFrTpTr this.mA.[0,i] this.mR.[0,i] this.mA.[0,j] this.mR.[0,j])
//                        )
//                )
//
//        let abmNew = 
//            DenseMatrix.init 
//                this.mAb.RowCount this.mAb.ColumnCount
//                (fun i j 
//                    -> if(i=j) then 0.0f else 
//                        (F32ToSF32
//                            this.mAb.[i,j] +          
//                            (ABadj this.mS.[0,i] this.mS.[0,j]) *
//                            learnRate *
//                            (FpFrTpTr this.mA.[0,i] this.mR.[0,i] this.mA.[0,j] this.mR.[0,j])
//                        )
//                )
//        
//        let bamNew = 
//            DenseMatrix.init 
//                this.mBa.RowCount this.mBa.ColumnCount
//                (fun i j 
//                    -> if(i=j) then 0.0f else 
//                        (F32ToSF32
//                            this.mBa.[i,j] +          
//                            (BAadj this.mS.[0,i] this.mS.[0,j]) *
//                            learnRate *
//                            (FpFrTpTr this.mA.[0,i] this.mR.[0,i] this.mA.[0,j] this.mR.[0,j])
//                        )
//                )
//
//        let bbmNew = 
//            DenseMatrix.init 
//                this.mBb.RowCount this.mBb.ColumnCount
//                (fun i j 
//                    -> if(i=j) then 0.0f else 
//                        (F32ToSF32
//                            this.mBb.[i,j] +          
//                            (BBadj this.mS.[0,i] this.mS.[0,j]) *
//                            learnRate *
//                            (FpFrTpTr this.mB.[0,i] this.mR.[0,i] this.mB.[0,j] this.mR.[0,j])
//                        )
//                )
//
//        new Wng(
//            iteration = this.Iteration,
//            aaM = aamNew,
//            abM = abmNew,
//            baM = bamNew,
//            bbM = bbmNew,
//            aM = this.mA,
//            bM = this.mB,
//            sM = this.mS,
//            rM = this.mR,
//            ssM = this.mSs,
//            cPp = this.cPp,
//            cSs = this.cSs,
//            cRp = this.cRp,
//            cPs = this.cPs,
//            nP = this.pNoise,
//            nS = this.sNoise
//        )
