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



    let UpdateTr (zeus:Zeus) (mem:Matrix<float32>) 
                 memIndex pNoise sNoise cPp cSs cRp cPs 
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

        let newA = aNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + athena.mA.[0,i] + cPp * (dAdA.[0,i] + dAdB.[0,i]) 
                        + dAdR.[0,i] ))

        let newB = aNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + athena.mB.[0,i] + cPp * (dBdB.[0,i] + dBdA.[0,i]) 
                        + dBdR.[0,i] ))


        let dSdS = athena.mS.Multiply(zeus.mSs)

        let dSdP = DenseMatrix.init 1 groupCt  
                        (fun x y -> curMem.[0,y]  * (athena.mA .[0,y] 
                                    - athena.mB.[0,y]))

        let newS = sNoise |> Array.mapi(fun i n -> 
            F32ToSF32(n + athena.mS.[0,i] + cSs * dSdS.[0,i] + 
                        cPs * dSdP.[0,i]))

        new AthenaTr(                
            iteration=athena.Iteration + 1,
            aM=DenseMatrix.init 1 groupCt  
                    (fun x y -> newA.[x * groupCt + y]),
            bM=DenseMatrix.init 1 groupCt  
                    (fun x y -> newB.[x * groupCt + y]),
            sM=DenseMatrix.init 1 groupCt  
                    (fun x y -> newS.[x * groupCt + y]),
            dAdR=dAdR,
            dBdR=dBdR,
            dAdA=dAdA,
            dAdB=dAdB,
            dBdA=dBdA,
            dBdB=dBdB,
            dSdS=dSdS,
            dSdP=dSdP
            )


    let f a b = a + b

    let rec g a b i =
        let h b = f a b
        match i with
        | 0 -> h b
        | k -> g a (h b) (i-1)


    let UpdateTrRep (zeus:Zeus) (mems:Matrix<float32>) 
                    memIndex pNoiseLevel sNoiseLevel (seed:int)
                    cPp cSs cRp cPs 
                    (athena:Athena) reps =

        let rng = Random.MersenneTwister(seed)
        let pNoise = Generators.SeqOfRandSF32 pNoiseLevel rng
        let sNoise = Generators.SeqOfRandSF32 sNoiseLevel rng


        let UpdateA (a:Athena) = 
            fun  UpdateTr zeus mems memIndex pNoise 
                               sNoise cPp cSs cRp cPp cPs a


        let rec Ura (a:Athena) i =
            match i with
            | 0 -> UpdateA a
            | k -> Ura ((UpdateA a).Athena) (k-1)

        Ura athena reps

        None
        

    let LearnTr (zeus:Zeus) memIndex (learnRate:float32) (athena:Athena) =

        let curMem = zeus.meR.Row memIndex
        let grpCt = athena.GroupCount

        let mCs = Array2D.init grpCt grpCt
                        (fun i j -> MakeScorr athena.mS.[0,i] athena.mS.[0,j]) 
        
        let aamNew = 
            DenseMatrix.init 
                grpCt grpCt
                (fun i j 
                    -> if(i=j) then 0.0f else 
                        (F32ToSF32
                            zeus.mAa.[i,j] +
                            match mCs.[i,j] with
                            | AA sc ->  sc * learnRate *
                                        (FpFrTpTr athena.mA.[0,i] curMem.[i] athena.mA.[0,j] curMem.[j])
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
                                        (FpFrTpTr athena.mA.[0,i] curMem.[i] athena.mB.[0,j] curMem.[j])
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
                                        (FpFrTpTr athena.mB.[0,i] curMem.[i] athena.mA.[0,j] curMem.[j])
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
                                        (FpFrTpTr athena.mB.[0,i] curMem.[i] athena.mB.[0,j] curMem.[j])
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


 module AthenaBuilder =

    let CreateRandom (seed:int) ngSize pSig sSig =

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


 module ZeusBuilder =

    let CreateRandom (seed:int) ngSize memSize
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


        let rndAthena = AthenaBuilder.CreateRandom
                            seed ngSize 0.5 0.3
        let rndMems = MathNetUtils.RandNormalRectDenseSF32
                        memSize ngSize rnd 0.33

        match CreateRandom seed ngSize memSize ppSig
                            glauberRadius with
        | Some rndZeus-> Some (ZeusUtils.UpdateTr rndZeus rndMems 1 pNoise 
                                sNoise 0.1f
                                0.1f 0.1f 0.1f rndAthena)
        | None -> None

