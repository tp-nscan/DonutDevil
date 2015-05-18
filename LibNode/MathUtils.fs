namespace LibNode
module MathUtils =
    open System
    open MathNet.Numerics
    open MathNet.Numerics.Distributions
    open MathNet.Numerics.LinearAlgebra
    open MathNet.Numerics.LinearAlgebra.Matrix
    open MathNet.Numerics.Random

    let Hamming s1 s2 = Array.map2((=)) s1 s2 |> Seq.sumBy(fun b -> if b then 0 else 1)

    
    type IntPair = {x:int; y:int}
    type PointF32 = {x:float32; y:float32 }
    type TripleF32 = {x:float32; y:float32; z:float32}
    type CellF32 = {x:int; y:int; value:float32}


    type GroupShape =
        | Linear of int
        | Ring of int
        | Rectangle of IntPair
        | Torus of IntPair

    type SymmetricFormat =
        | UT
        | LT
        | Full

    type MatrixFormat =
        | RowMajor
        | ColumnMajor


    let PointF32Length (point:PointF32) = 
        let vsq = point.x * point.x + point.y * point.y
        Math.Sqrt(vsq |> System.Convert.ToDouble)
                      |> System.Convert.ToSingle

    let PointF32LengthSquared (point:PointF32) = 
        point.x * point.x + point.y * point.y


    let TripleF32Length (point:TripleF32) = 
        let vsq = point.x * point.x + point.y * point.y
        Math.Sqrt(vsq |> System.Convert.ToDouble)
                      |> System.Convert.ToSingle

    let TripleF32LengthSquared (point:TripleF32) = 
        point.x * point.x + point.y * point.y


    let Euclidean32 (s1:float32[]) (s2:float32[]) = 
        Array.map2(fun x y ->  (x - y) * (x - y)) s1 s2 
                            |> Seq.sum
                            |> System.Convert.ToDouble
                            |> Math.Sqrt
                            |> System.Convert.ToSingle
