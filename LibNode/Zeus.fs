﻿namespace LibNode
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
            ssM:Matrix<float32>,
            reM:Matrix<float32>
        ) =
    member this.GroupCount = reM.ColumnCount
    member this.EnsembleCount = reM.RowCount
    member this.mAa = aaM
    member this.mAb = abM
    member this.mBa = baM
    member this.mBb = bbM
    member this.mSs = ssM
    member this.meR = reM

type Scorr =
    | AA of float32
    | AB of float32
    | BA of float32
    | BB of float32


 type ZeusTr(
            aaM:Matrix<float32>,
            abM:Matrix<float32>,
            baM:Matrix<float32>,
            bbM:Matrix<float32>,
            ssM:Matrix<float32>,
            reM:Matrix<float32>,
            scM:Scorr[,]
        ) =

    member this.Zeus 
        = new Zeus(            
                    aaM = aaM,
                    abM = abM,
                    baM = baM,
                    bbM = bbM,
                    ssM = ssM,
                    reM = reM
                  )
    member this.scM = scM


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
                dA:Matrix<float32>,
                dB:Matrix<float32>,
                dS:Matrix<float32>,
                dAdR:Matrix<float32>,
                dBdR:Matrix<float32>,
                dAdA:Matrix<float32>,
                dAdB:Matrix<float32>,
                dBdA:Matrix<float32>,
                dBdB:Matrix<float32>,
                dSdS:Matrix<float32>,
                dSdP:Matrix<float32>,
                message:string
            ) =

    member this.Athena = new Athena(iteration, aM, bM, sM)
    member this.dA = dA
    member this.dB = dB
    member this.dS = dS
    member this.dAdR = dAdR
    member this.dBdR = dBdR
    member this.dAdA = dAdA
    member this.dAdB = dAdB
    member this.dBdA = dBdA
    member this.dBdB = dBdB
    member this.dSdS = dSdS
    member this.dSdP = dSdP
    member this.Message = message

module ZeusUtils =

    let MakeScorr si sj =
        match si, sj with
        | si, sj when (si< 0.0f) && (sj< 0.0f) -> Scorr.AA (si*sj)
        | si, sj when (si< 0.0f) && (sj>=0.0f) -> Scorr.AB (- si*sj)
        | si, sj when (si>=0.0f) && (sj< 0.0f) -> Scorr.BA (- si*sj)
        | si, sj when (si>=0.0f) && (sj>=0.0f) -> Scorr.BB (si*sj)
        | _, _     -> failwith  "cant happen"

    let FpFrTpTr (fp:float32) fr tp tr = 
        if (fp*fr < 0.0f) then 0.0f else
        (fr*tr - fp*tp)

module ZeusF =

    let NextAthenaTr (zeus:Zeus) memIndex pNoise 
                     sNoise cPp cSs cRp cPs message 
                     (athena:Athena) =

        let groupCt = athena.GroupCount
        let curMem = zeus.meR.SubMatrix(memIndex, 1, 0, zeus.meR.ColumnCount)

        let sNoise = sNoise |> Seq.take groupCt |> Seq.toArray
        let aNoise = pNoise |> Seq.take groupCt |> Seq.toArray
        let bNoise = pNoise |> Seq.take groupCt |> Seq.toArray

        let dAdR = curMem.Map2((fun x y -> x * (1.0f+y) * (1.0f+y) * cRp), athena.mS)
        let dBdR = curMem.Map2((fun x y -> x * (1.0f-y) * (1.0f-y) * cRp), athena.mS)

        let corrM = DenseMatrix.init 
                        groupCt groupCt  
                        (UpperTriangulateZd 
                        groupCt ( fun x y -> let sqr = (1.0f + athena.mS.[0,x] 
                                                          * athena.mS.[0,y])
                                             sqr*sqr ))

        let acorM = DenseMatrix.init 
                        groupCt groupCt  
                        (UpperTriangulateZd 
                        groupCt ( fun x y -> let sqr = (1.0f - athena.mS.[0,x] * athena.mS.[0,y])
                                             sqr*sqr ))

        let mAas = zeus.mAa.Map2 ((fun a b -> a*b), corrM)
        let mBas = zeus.mBa.Map2 ((fun a b -> a*b), acorM)
        let mAbs = zeus.mAb.Map2 ((fun a b -> a*b), acorM)
        let mBbs = zeus.mBb.Map2 ((fun a b -> a*b), corrM)


        let dAdA = athena.mA.Multiply(mAas)
        let dAdB = athena.mB.Multiply(mBas)
        let dBdA = athena.mA.Multiply(mAbs)
        let dBdB = athena.mB.Multiply(mBbs)

        let dA = DenseMatrix.init 1 groupCt  
                  (fun x y -> aNoise.[y] + 
                              cPp * (dAdA.[0,y] + dAdB.[0,y]) + 
                              dAdR.[0,y])

        let dB = DenseMatrix.init 1 groupCt  
                  (fun x y -> bNoise.[y] +
                              cPp * (dBdB.[0,y] + dBdA.[0,y]) + 
                              dBdR.[0,y])

        let dSdS = athena.mS.Multiply(zeus.mSs)

        let dSdP = DenseMatrix.init 1 groupCt  
                        (fun x y -> curMem.[0,y]  * (athena.mA.[0,y] 
                                    - athena.mB.[0,y]))

        let dS = DenseMatrix.init 1 groupCt  
                  (fun x y -> sNoise.[y] +
                              cSs * dSdS.[0,y] + 
                              cPs * dSdP.[0,y])

        new AthenaTr(                
                iteration=athena.Iteration + 1,
                aM=DenseMatrix.init 1 groupCt  
                        (fun x y -> F32ToSF32(athena.mA.[0,y] + dA.[0,y])),
                bM=DenseMatrix.init 1 groupCt  
                        (fun x y -> F32ToSF32(athena.mB.[0,y] + dB.[0,y])),
                sM=DenseMatrix.init 1 groupCt  
                        (fun x y -> F32ToSF32(athena.mS.[0,y] + dS.[0,y])),
                dA=dA,
                dB=dA,
                dS=dA,
                dAdR=dAdR,
                dBdR=dBdR,
                dAdA=dAdA,
                dAdB=dAdB,
                dBdA=dBdA,
                dBdB=dBdB,
                dSdS=dSdS,
                dSdP=dSdP,
                message = message
            )


    let HaltAthenaTr halter (zeus:Zeus) memIndex  
                    pNoiseLevel sNoiseLevel (seed:int)
                    cPp cSs cRp cPs 
                    (athena:Athena) =

        let rng = Random.MersenneTwister(seed)
        let pNoise = Generators.SeqOfRandSF32 pNoiseLevel rng
        let sNoise = Generators.SeqOfRandSF32 sNoiseLevel rng

        let message  = sprintf "memIndex:%i; pNoiseLevel:%f; sNoiseLevel:%f; 
                                    seed:%i; cPp:%f; cSs:%f; cRp:%f; cPs:%f;"
                                    memIndex pNoiseLevel sNoiseLevel seed cPp cSs cRp cPs 

        let message  = "hi"
        let CurriedNext  = 
                NextAthenaTr zeus memIndex
                     pNoise sNoise cPp cSs cRp cPs message

        let rec Ura (aTr:AthenaTr) halter =
            match halter(aTr) with
            | true -> aTr
            | _ -> Ura (CurriedNext aTr.Athena) halter

        Ura (CurriedNext athena) halter

    let RepAthenaTr (zeus:Zeus) memIndex pNoiseLevel 
                    sNoiseLevel (seed:int)
                    cPp cSs cRp cPs 
                    (athena:Athena) reps =

          HaltAthenaTr (fun (aTr:AthenaTr) -> 
                                aTr.Athena.Iteration = 
                                    reps + athena.Iteration
                       )
                       zeus memIndex pNoiseLevel
                       sNoiseLevel seed
                       cPp cSs cRp cPs 
                       athena
//
//        let rng = Random.MersenneTwister(seed)
//        let pNoise = Generators.SeqOfRandSF32 pNoiseLevel rng
//        let sNoise = Generators.SeqOfRandSF32 sNoiseLevel rng
//
//        let CurriedNext  = 
//                NextAthenaTr zeus memIndex
//                     pNoise sNoise cPp cSs cRp cPs
//
//        let rec Ura (a:Athena) i =
//            match i with
//            | 0 -> CurriedNext a
//            | k -> Ura ((CurriedNext a).Athena) (k-1)
//
//        Ura athena reps


    let NextZeusTr (zeus:Zeus) memIndex 
                learnRate (athena:Athena) =

        let curMem = zeus.meR.Row memIndex
        let grpCt = athena.GroupCount

        let mCs = Array2D.init grpCt grpCt
                        (fun i j -> ZeusUtils.MakeScorr athena.mS.[0,i] athena.mS.[0,j]) 
        
        let aamNew = 
            DenseMatrix.init 
                grpCt grpCt
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            zeus.mAa.[i,j] +
                            match mCs.[i,j] with
                            | AA sc ->  sc * learnRate *
                                        (ZeusUtils.FpFrTpTr athena.mA.[0,i] curMem.[i] 
                                                            athena.mA.[0,j] curMem.[j])
                            | _ -> 0.0f
                        )
                )

        let abmNew = 
            DenseMatrix.init 
                grpCt grpCt
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            zeus.mAb.[i,j] +
                            match mCs.[i,j] with
                            | AB sc ->  sc * learnRate *
                                        (ZeusUtils.FpFrTpTr athena.mA.[0,i] curMem.[i] 
                                                            athena.mB.[0,j] curMem.[j])
                            | _ -> 0.0f
                        )
                )

        let bamNew = 
            DenseMatrix.init 
                grpCt grpCt
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            zeus.mBa.[i,j] +
                            match mCs.[i,j] with
                            | BA sc ->  sc * learnRate *
                                        (ZeusUtils.FpFrTpTr athena.mB.[0,i] curMem.[i] 
                                                            athena.mA.[0,j] curMem.[j])
                            | _ -> 0.0f
                        )
                )

        let bbmNew = 
            DenseMatrix.init 
                grpCt grpCt
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            zeus.mBb.[i,j] +
                            match mCs.[i,j] with
                            | BB sc ->  sc * learnRate *
                                        (ZeusUtils.FpFrTpTr athena.mB.[0,i] curMem.[i] 
                                                            athena.mB.[0,j] curMem.[j])
                            | _ -> 0.0f
                        )
                )

        new ZeusTr(
                    aaM = zeus.mAa,
                    abM = zeus.mAb,
                    baM = zeus.mBa,
                    bbM = zeus.mBb,
                    ssM = zeus.mSs,
                    reM = zeus.meR,
                    scM = mCs
                  )


 module ZeusBuilders =

    let CreateRandomAthena (seed:int) ngSize pSig sSig =

        let rng = Random.MersenneTwister(seed)
        
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

        new Athena(
                iteration=0,
                aM=mA,
                bM=mB,
                sM=mS
            )

    let AthenaToTr (athena:Athena)  =

        let groupCt = athena.GroupCount

        new AthenaTr(                
                iteration=athena.Iteration,
                aM=athena.mA,
                bM=athena.mB,
                sM=athena.mS,
                dA=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                dB=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                dS=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                dAdR=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                dBdR=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                dAdA=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                dAdB=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                dBdA=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                dBdB=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                dSdS=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                dSdP=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
                message = ""
            )


    let ResetAthenaP pSig (seed:int) (athena:Athena) =

        let rng = Random.MersenneTwister(seed)
        
        let temp = Generators.NormalSF32 rng pSig
                       |> Seq.take(athena.GroupCount)
                       |> Seq.toArray
        let mA = DenseMatrix.init 1 athena.GroupCount
                    (fun x y -> temp.[y])

        let temp = Generators.NormalSF32 rng pSig
                       |> Seq.take(athena.GroupCount) 
                       |> Seq.toArray
        let mB = DenseMatrix.init 1 athena.GroupCount
                    (fun x y -> temp.[y])

        new Athena(
                iteration=0,
                aM=mA,
                bM=mB,
                sM=athena.mS
            )


    let ResetAthenaS sSig (seed:int) (athena:Athena) =

        let rng = Random.MersenneTwister(seed)
        
        let temp = Generators.NormalSF32 rng sSig
                       |> Seq.take(athena.GroupCount)
                       |> Seq.toArray
        let mS = DenseMatrix.init 1 athena.GroupCount
                    (fun x y -> temp.[y])

        new Athena(
                iteration=0,
                aM=athena.mA,
                bM=athena.mB,
                sM=mS
            )


    let CreateRandomZeus (seed:int) ngSize memSize
                          ppSig glauberRadius =

        let rng = Random.MersenneTwister(seed)

        let mAa = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD

        let mAb = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD

        let mBb = (RandNormalSqSymDenseSF32 ngSize rng ppSig)
                  |> MatrixF32ZeroD

        let mRe = MathNetUtils.RandRectDenseSF32Bits
                    memSize ngSize rng


        match GlauberNeutralDense ngSize glauberRadius with
        | Some csMatrix ->
            Some (
                    new Zeus(            
                                aaM = mAa,
                                abM = mAb,
                                baM = mAb.Transpose(),
                                bbM = mBb,
                                ssM = csMatrix,
                                reM = mRe
                            )
                    )
        | None -> None


    let DesignAthenaTr (seed:int) ngSize memSize
                        ppSig glauberRadius = 

        let rnd = Random.MersenneTwister(seed);
        let sNoise = Generators.NormalSF32 rnd 0.4
        let pNoise = Generators.NormalSF32 rnd 0.4


        let rndAthena = CreateRandomAthena
                            seed ngSize 0.5 0.3

        match CreateRandomZeus seed ngSize memSize ppSig
                                glauberRadius with
        | Some rndZeus-> Some (ZeusF.NextAthenaTr rndZeus 1 pNoise 
                                sNoise 0.1f
                                0.1f 0.1f 0.1f "hi" rndAthena)
        | None -> None

