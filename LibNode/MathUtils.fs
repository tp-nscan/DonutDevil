namespace LibNode

module Dict =
  open System.Collections.Generic
  let toSeq d = d |> Seq.map (fun (KeyValue(k,v)) -> (k,v))
  let toArray (d:IDictionary<_,_>) = d |> toSeq |> Seq.toArray
  let toList (d:IDictionary<_,_>) = d |> toSeq |> Seq.toList
  let ofMap (m:Map<'k,'v>) = new Dictionary<'k,'v>(m) :> IDictionary<'k,'v>
  let ofList (l:('k * 'v) list) = new Dictionary<'k,'v>(l |> Map.ofList) :> IDictionary<'k,'v>
  let ofSeq (s:('k * 'v) seq) = new Dictionary<'k,'v>(s |> Map.ofSeq) :> IDictionary<'k,'v>
  let ofArray (a:('k * 'v) []) = new Dictionary<'k,'v>(a |> Map.ofArray) :> IDictionary<'k,'v>

module MathUtils =
    open System
    open MathNet.Numerics
    open MathNet.Numerics.Distributions
    open MathNet.Numerics.LinearAlgebra
    open MathNet.Numerics.LinearAlgebra.Matrix
    open MathNet.Numerics.Random


    let ZeroF32 = Convert.ToSingle 0.0
    let OneF32 = Convert.ToSingle 1.0
    let NOneF32 = Convert.ToSingle -1.0
    let TwoF32 = Convert.ToSingle 2.0
    
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


    let Hamming s1 s2 = Array.map2((=)) s1 s2 |> Seq.sumBy(fun b -> if b then 0 else 1)


    let CompareArrays<'a> comp (seqA:'a[]) (seqB:'a[]) =
        Seq.fold (&&) true (Seq.zip seqA seqB |> Seq.map (fun (aa,bb) -> comp aa bb))


    let CompareFloat32Arrays (seqA:float32[]) (seqB: float32[]) =
        Seq.fold (&&) true (Seq.zip seqA seqB |> Seq.map (fun (aa,bb) -> aa = bb))


    let TransposeArray2D (mtx : _ [,]) = 
        Array2D.init (mtx.GetLength 1) (mtx.GetLength 0) (fun x y -> mtx.[y,x])


    let Array2DFromRowMajor (rowCount:int) (colCount:int) (values:float32[]) =
        if values.Length <> rowCount*colCount then
            None
        else
            Some (Array2D.init rowCount colCount (fun x y -> values.[y+x*colCount]))

 
    let Array2DFromColumnMajor (rowCount:int) (colCount:int) (values:float32[]) =
        if values.Length <> rowCount*colCount then
            None
        else
            Some (Array2D.init rowCount colCount (fun x y -> values.[x+y*rowCount]))


    let flattenRowMajor (A:'a[,]) = A |> Seq.cast<'a>

    let flattenColumnMajor (A:'a[,]) = A |> TransposeArray2D |> Seq.cast<'a>

    let getColumn c (A:_[,]) = A.[*,c] |> Seq.toArray

    let getRow r (A:_[,]) = A.[r,*] |> Seq.toArray
