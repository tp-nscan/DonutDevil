#r @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\packages\MathNet.Numerics.3.7.0\lib\net40\MathNet.Numerics.dll"
#r @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\packages\MathNet.Numerics.FSharp.3.7.0\lib\net40\MathNet.Numerics.FSharp.dll"
#r @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\LibNode\bin\Debug\LibNode.dll"


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

open LibNode

#load "Generators.fs"
#load "DataBlocks.fs"

let kvps = [("one",1); ("two",2)] |> Dict.ofList

let l = kvps |> Dict.toList












type LoggingBuilder() =
    let log p = p * 100

    member this.Bind(x, f) = 
        let x = log x
        f x

    member this.Return(x) = 
        x


let logger = new LoggingBuilder()

let loggedWorkflow = 
    logger
        {
        let! x = 42
        let! y = 43
        let! z = x + y
        return z
        }

let strToInt str =
 match (System.Int32.TryParse str) with
 | (true, res) -> Some res
 | (false, _) -> None
 
type MaybeBuilder() =

    member this.Bind(x, f) = 
        match x with
        | None -> None
        | Some a -> f a

    member this.Return(x) = 
        Some x
   
let maybeu = new MaybeBuilder()


let stringAddWorkflow x y z = 
    maybeu 
        {
        let! a = strToInt x
        let! b = strToInt y
        let! c = strToInt z
        return a + b + c
        }    

let strAdd str i =
    match (strToInt str) with
    | None -> None
    | Some a -> Some (a + i)

let (>>=) m f = 
    match m with
    | None -> None
    | Some x -> f x

let good = strToInt "1" >>= strAdd "2" >>= strAdd "3"

let pD = Parameters.LinearLocalSet


let fa a b =  a b

let fb a = function x -> a x

let fc x = fa(fb x)

let SeqSrc = Seq.initInfinite(fun i-> i).GetEnumerator()

let res1() = 
    SeqSrc.MoveNext() |> ignore
    SeqSrc.Current
