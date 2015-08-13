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
            learnRate:float32,
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
    member this.LearnRate = learnRate


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
                rM:Matrix<float32>,
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
                dSdP:Matrix<float32>
            ) =

    member this.Athena = new Athena(iteration, aM, bM, sM)
    member this.mR = rM
    member this.mV = DenseMatrix.init 1 sM.ColumnCount  
                        (fun x y -> (
                                      aM.[0,y] * (1.0f + sM.[0,y])
                                      + 
                                      bM.[0,y] * (1.0f - sM.[0,y])
                                     ) / 2.0f
                        )
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

type AthenaStageRes(
                    athenaTr:AthenaTr,
                    memIndex:int, 
                    pNoiseL:float32, 
                    sNoiseL:float32, 
                    seed:int, 
                    cPp:float32, 
                    cSs:float32, 
                    cRp:float32, 
                    cPs:float32, 
                    message:string 
                ) =

    member this.AthenaTr = athenaTr
    member this.MemIndex = memIndex
    member this.pNoiseL = pNoiseL
    member this.sNoiseL = sNoiseL
    member this.Seed = seed
    member this.Cpp = cPp
    member this.Css = cSs
    member this.Crp = cRp
    member this.Cps = cPs
    member this.Message = message

    
    type ZeusSnap = { Id:Guid; ParentId:Option<Guid>; 
                      AthenaStageRes:AthenaStageRes;}

    
    type ZeusSnaps() =
        let _snapList = ([]: ZeusSnap list)
                
        member this.AddAthenaStageRes(athenaStageRes:AthenaStageRes) (parentId:Guid) =
            _snapList = { ZeusSnap.Id=Guid.NewGuid(); ParentId=(Some parentId); 
                            AthenaStageRes=athenaStageRes;}::_snapList

        member this.AddOrpanAthenaStageRes(athenaStageRes:AthenaStageRes) =
            _snapList = { ZeusSnap.Id=Guid.NewGuid(); ParentId=None; 
                            AthenaStageRes=athenaStageRes;}::_snapList

        member this.Snaps() = 
            _snapList |> List.toSeq

        member this.Clear() = 
            _snapList = ([]: ZeusSnap list)


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

    let RdotV (athenaTr:AthenaTr) =
        athenaTr.mR.Row(0).DotProduct(athenaTr.mV.Row(0))

    let AdotB (athenaTr:AthenaTr) =
        athenaTr.Athena.mA.Row(0).DotProduct(athenaTr.Athena.mB.Row(0))

    let SCorr (athenaTr:AthenaTr) =
      athenaTr.Athena.mS.Row(0).DotProduct(
            athenaTr.Athena.mS.Row(0) |> MathNetUtils.VectorShift )


module ZeusF =

    let NextAthenaTr (zeus:Zeus) memIndex pNoise 
                     sNoise cPp (cSs:float32) cRp cPs 
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
                                             sqr*sqr*cPp ))

        let acorM = DenseMatrix.init 
                        groupCt groupCt  
                        (UpperTriangulateZd 
                        groupCt ( fun x y -> let sqr = (1.0f - athena.mS.[0,x] * athena.mS.[0,y])
                                             sqr*sqr*cPp ))

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
                              dAdA.[0,y] + dAdB.[0,y] + 
                              dAdR.[0,y])

        let dB = DenseMatrix.init 1 groupCt
                  (fun x y -> bNoise.[y] +
                              dBdB.[0,y] + dBdA.[0,y] + 
                              dBdR.[0,y])

        let dSdS = athena.mS.Multiply(cSs).Multiply(zeus.mSs)

        let dSdP = DenseMatrix.init 1 groupCt  
                        (fun x y -> cPs * curMem.[0,y]  * (athena.mA.[0,y] 
                                    - athena.mB.[0,y]))

        let dS = DenseMatrix.init 1 groupCt  
                  (fun x y -> sNoise.[y] + dSdS.[0,y] + dSdP.[0,y])

        new AthenaTr(                
                iteration=athena.Iteration + 1,
                aM=DenseMatrix.init 1 groupCt  
                        (fun x y -> F32ToSF32(athena.mA.[0,y] + dA.[0,y])),
                bM=DenseMatrix.init 1 groupCt  
                        (fun x y -> F32ToSF32(athena.mB.[0,y] + dB.[0,y])),
                sM=DenseMatrix.init 1 groupCt  
                        (fun x y -> F32ToSF32(athena.mS.[0,y] + dS.[0,y])),
                rM=curMem,
                dA=dA,
                dB=dB,
                dS=dS,
                dAdR=dAdR,
                dBdR=dBdR,
                dAdA=dAdA,
                dAdB=dAdB,
                dBdA=dBdA,
                dBdB=dBdB,
                dSdS=dSdS,
                dSdP=dSdP
            )


    let HaltAthenaTr halter (zeus:Zeus) memIndex  
                    (pNoiseL:float32) (sNoiseL:float32) 
                    (seed:int) cPp cSs cRp cPs message
                    (athena:Athena) =

        let rng = Random.MersenneTwister(seed)
        let pNoise = Generators.SeqOfRandSF32 pNoiseL rng
        let sNoise = Generators.SeqOfRandSF32 sNoiseL rng

        let CurriedNext  = 
                NextAthenaTr zeus memIndex
                     pNoise sNoise cPp cSs cRp cPs

        let rec Ura (aTr:AthenaTr) halter =
            match halter(aTr) with
            | true -> new AthenaStageRes(aTr, memIndex, pNoiseL, 
                                         sNoiseL, seed, cPp, 
                                         cSs, cRp, cPs, message)
            | _ -> Ura (CurriedNext aTr.Athena) halter

        Ura (CurriedNext athena) halter

    let RepAthenaTr (zeus:Zeus) memIndex pNoiseL 
                    sNoiseL (seed:int)
                    cPp cSs cRp cPs 
                    (athena:Athena) (reps:int) =

        HaltAthenaTr (fun (aTr:AthenaTr) -> 
                            aTr.Athena.Iteration = 
                                reps + athena.Iteration
                     )
                    zeus memIndex pNoiseL
                    sNoiseL seed
                    cPp cSs cRp cPs 
                    (sprintf "reps: %i" reps)
                    athena


    let NextZeusTr (zeus:Zeus) memIndex 
                learnRate (athena:Athena) =

        let curMem = zeus.meR.Row memIndex
        let grpCt = athena.GroupCount

        let a2Scorr 
          = Array2D.init grpCt grpCt
             (fun i j -> ZeusUtils.MakeScorr athena.mS.[0,i] 
                                             athena.mS.[0,j]) 

        
        let a2Dcoal 
          = Array2D.init grpCt grpCt
             (fun i j -> match a2Scorr.[i,j] with
                         | AA sc -> AA (sc * (ZeusUtils.FpFrTpTr 
                                        athena.mA.[0,i] curMem.[i] 
                                        athena.mA.[0,j] curMem.[j]))
                         | AB sc -> AB (sc * (ZeusUtils.FpFrTpTr 
                                        athena.mA.[0,i] curMem.[i] 
                                        athena.mB.[0,j] curMem.[j]))
                         | BA sc -> BA (sc * (ZeusUtils.FpFrTpTr 
                                        athena.mB.[0,i] curMem.[i] 
                                        athena.mA.[0,j] curMem.[j]))
                         | BB sc -> BB (sc * (ZeusUtils.FpFrTpTr 
                                        athena.mB.[0,i] curMem.[i] 
                                        athena.mB.[0,j] curMem.[j]))


                         | _ -> failwith "cant get here"
             )
        
        let aamD = 
            DenseMatrix.init 
                grpCt grpCt
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        match a2Dcoal.[i,j] with
                        | AA sc ->  sc * learnRate
                        | _ -> 0.0f
                )

        let aamNew = 
            DenseMatrix.init 
                grpCt grpCt
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            zeus.mAa.[i,j] +
                            match a2Dcoal.[i,j] with
                            | AA sc ->  sc * learnRate
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
                            match a2Dcoal.[i,j] with
                            | AB sc ->  sc * learnRate
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
                            match a2Dcoal.[i,j] with
                            | BA sc ->  sc * learnRate
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
                            match a2Dcoal.[i,j] with
                            | BB sc ->  sc * learnRate
                            | _ -> 0.0f
                        )
                )

        new ZeusTr(
                    aaM = aamNew,
                    abM = abmNew,
                    baM = bamNew,
                    bbM = bbmNew,
                    ssM = zeus.mSs,
                    reM = zeus.meR,
                    scM = a2Scorr,
                    learnRate = learnRate
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
                rM=DenseMatrix.init 1 groupCt (fun x y -> 0.0f),
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
                dSdP=DenseMatrix.init 1 groupCt (fun x y -> 0.0f)
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
                                0.1f 0.1f 0.1f rndAthena)
        | None -> None


