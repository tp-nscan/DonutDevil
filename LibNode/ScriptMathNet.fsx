#r @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\packages\MathNet.Numerics.3.7.0\lib\net40\MathNet.Numerics.dll"
#r @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\packages\MathNet.Numerics.FSharp.3.7.0\lib\net40\MathNet.Numerics.FSharp.dll"
#r @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\LibNode\bin\Debug\LibNode.dll"

open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open LibNode.MathUtils
open LibNode.MathNetUtils
open LibNode.Glauber


let rng = Random.MersenneTwister(123)

let mS = RandNormalSqSymDenseSF32 5 rng 0.8

let mrS = mS.SubMatrix(2,0,0,5)

let vs = GlauberRadius5

let m = GlauberDenseMatrix 15 GlauberRadius5

let ms = GlauberSparseMatrix 15 GlauberRadius5

let msm = ms.Multiply m

let mms = m.Multiply ms


let mz = m |> MatrixF32ZeroD

let chkGlauberBalance r f d =
    let gvs = (GlauberVals r f d) 
    (gvs , (gvs |> Array.sum))

let tm = LibNode.Generators.NormalSF32 rng 0.28
         |> Seq.take 100 |> Seq.toArray

// dense 3x4 matrix created from a sequence of sequence-columns
let x = Seq.init 4 (fun c -> Seq.init 3 (fun r -> float (100*r + c)))
let m5 = DenseMatrix.ofColumnSeq x

let m7a = DenseMatrix.random<float32> 3 4 (ContinuousUniform(-2.0, 4.0))
let ww = m7a.ToArray()
let wnz = ww |> ZeroTheDiagonalF32

let F32ToSF32 value =
    if value < -1.0f then -1.0f
    else if value > 1.0f then 1.0f
    else value

let F32ToUF32 value =
    if value < 0.0f then 0.0f
    else if value > 1.0f then 1.0f
    else value

let away = [|1.0f;-22.2f;5.2f;-4.3f;1.1f;22.2f;-55.2f;14.3f|]


