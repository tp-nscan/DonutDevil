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

// dense 3x4 matrix created from a sequence of sequence-columns
let x = Seq.init 4 (fun c -> Seq.init 3 (fun r -> float (100*r + c)))
let m5 = DenseMatrix.ofColumnSeq x

let m7a = DenseMatrix.random<float32> 3 4 (ContinuousUniform(-2.0, 4.0))
let ww = m7a.ToArray()
let wnz = ww |> ZeroTheDiagonalF32

open System
open LibNode.MathUtils

let BoundUnitSF32 value =
    if value < -1.0f then -1.0f
    else if value > 1.0f then 1.0f
    else value

let BoundUnitUF32 value =
    if value < 0.0f then 0.0f
    else if value > 1.0f then 1.0f
    else value

let away = [|1.0f;-22.2f;5.2f;-4.3f;1.1f;22.2f;-55.2f;14.3f|]

let TrimToScale (a:float32[]) (b:float32) =
    let sa = a |> Array.sortBy(fun x-> Math.Abs(x))
    a |> Array.map(fun x-> (x/b)|>BoundUnitSF32)

let TrimByFraction (a:float32[]) (b:float32) =
    let bub = Convert.ToInt32(Convert.ToSingle(a.Length - 1) * BoundUnitUF32(b))
    let sa = a |> Array.sortBy(fun x-> Math.Abs(x))
    if bub = a.Length then a
    else TrimToScale a (AbsF32(sa.[bub]))

