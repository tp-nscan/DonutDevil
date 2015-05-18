#r @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\packages\MathNet.Numerics.3.7.0\lib\net40\MathNet.Numerics.dll"
#r @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\packages\MathNet.Numerics.FSharp.3.7.0\lib\net40\MathNet.Numerics.FSharp.dll"

open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random

fsi.AddPrinter(fun (matrix:Matrix<float>) -> matrix.ToString())
fsi.AddPrinter(fun (matrix:Matrix<float32>) -> matrix.ToString())
fsi.AddPrinter(fun (matrix:Matrix<complex>) -> matrix.ToString())
fsi.AddPrinter(fun (matrix:Matrix<complex32>) -> matrix.ToString())
fsi.AddPrinter(fun (vector:Vector<float>) -> vector.ToString())
fsi.AddPrinter(fun (vector:Vector<float32>) -> vector.ToString())
fsi.AddPrinter(fun (vector:Vector<complex>) -> vector.ToString())
fsi.AddPrinter(fun (vector:Vector<complex32>) -> vector.ToString())


#load "Generators.fs"
#load "DataBlocks.fs"


open LibNode
//let samples = Random.doubles 1000
//
//// overwrite the whole array with new random values
//Random.doubleFill samples
//
//// create an infinite sequence:
//let sampleSeq = Random.doubleSeq ()
//
//// take a single random value
//let rng = Random.mersenneTwisterShared
//let sample = rng.NextDouble()
//let sampled = rng.NextDecimal()

let ralph = LibNode.Generators.RandomUnsignedIntervalF32 (System.Convert.ToSingle(0.5)) 20 |> Seq.toArray

let linus = Ng1D ((Vs1D.UnSigned (System.Convert.ToSingle(0.5))), (GroupShape.Linear 20), ralph)


let linus2 = Ng1D (Vs1D.Ring, (GroupShape.Linear 20), ralph)
