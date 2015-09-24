namespace LibNode
open System

type D1Val<'a> = {Index:int; Val:'a}

type D2Val<'a> = {X:int; Y:int; Val:'a}


//%s for strings
//%b for bools
//%i for ints
//%f for floats
//%A for pretty-printing tuples, records and union types

// see also 
// http://stackoverflow.com/questions/4949941/convert-string-to-system-datetime-in-f
module TryParser =
    // convenient, functional TryParse wrappers returning option<'a>
    let tryParseWith tryParseFunc = tryParseFunc >> function
        | true, v    -> Some v
        | false, _   -> None

    let parseDate   = tryParseWith System.DateTime.TryParse
    let parseInt    = tryParseWith System.Int32.TryParse
    let parseSingle = tryParseWith System.Single.TryParse
    let parseDouble = tryParseWith System.Double.TryParse
    // etc.

    // active patterns for try-parsing strings
    let (|Date|_|)   = parseDate
    let (|Int|_|)    = parseInt
    let (|Single|_|) = parseSingle
    let (|Double|_|) = parseDouble

// tests
    let parseMe = function
        | Date d   -> printfn "DateTime %A" d
        | Int 42   -> printfn "Bingo!"
        | Int i    -> printfn "Int32 %i" i
        | Single f -> printfn "Single %g" f
        | Double d -> printfn "Double %g" d // never hit, always parsed as float32
        | s        -> printfn "Don't know how to parse %A" s

//    parseMe "213"
//    parseMe "2010-02-22"
//    parseMe "3,0"
//    parseMe "jslkdfjlkj"
//    parseMe "42"

module Dict =
  open System.Collections.Generic
  let toSeq d = d |> Seq.map (fun (KeyValue(k,v)) -> (k,v))
  let toSeqValues d = d |> Seq.map (fun (KeyValue(k,v)) -> v)
  let toArray (d:IDictionary<_,_>) = d |> toSeq |> Seq.toArray
  let toListT (d:IDictionary<_,_>) = d |> toSeq |> Seq.toList
  let toList d = d |> Seq.map (fun (KeyValue(k,v)) -> v) |> Seq.toList
  let ofMap (m:Map<'k,'v>) = new Dictionary<'k,'v>(m) :> IDictionary<'k,'v>
  let ofList (l:('k * 'v) list) = new Dictionary<'k,'v>(l |> Map.ofList) :> IDictionary<'k,'v>
  let ofSeq (s:('k * 'v) seq) = new Dictionary<'k,'v>(s |> Map.ofSeq) :> IDictionary<'k,'v>
  let ofArray (a:('k * 'v) []) = new Dictionary<'k,'v>(a |> Map.ofArray) :> IDictionary<'k,'v>

module MathUtils =
    type IntPair = {x:int; y:int}
    type PointF32 = {x:float32; y:float32 }
    type TripleF32 = {x:float32; y:float32; z:float32}
    type CellF32 = {x:int; y:int; value:float32}
    
    let AbsF32 (v:float32) =
        if v < 0.0f then -v
        else v

    let inline AddInRange min max a b =
        let res = a + b
        if res < min then min
        else if res > max then max
        else res

    let inline FlipWhen a b flipProb draw current =
        match (flipProb < draw) with
        | true -> if a=current then b else a
        | false -> current

    let inline ZipMap f a b = Seq.zip a b |> Seq.map (fun (x,y) -> f x y)

    let SeqAddInRange min max offsets baseSeq =
        Seq.map2 (AddInRange min max) baseSeq offsets
    
    let MesForItem initElement count mutator =
       Array.init count (fun index ->
         match index with
         | 0 -> initElement
         | _ -> mutator initElement)
                                        
    let MesForArray (initArray:'a[][]) copies mutator =
       let initLen = initArray.GetLength(0)
       Array.init (copies*initLen) (fun index ->
         match (index % copies) with
         | 0 -> initArray.[index/copies]
         | _ -> mutator initArray.[index/copies])

    let GetRowsForArray2D (array:'a[,]) =
      let rowC = array.GetLength(0)
      let colC = array.GetLength(1)
      Array.init rowC (fun i ->
        Array.init colC (fun j -> array.[i,j]))

    let MesForArray2D (array2d:'a[,]) copies mutator =
       let nstArray = GetRowsForArray2D array2d
       let initLen = nstArray.GetLength(0)
       Array.init (copies*initLen) (fun index ->
         match (index % copies) with
         | 0 -> nstArray.[index/copies]
         | _ -> mutator nstArray.[index/copies])

    let F32ToUF32 value =
        if value < 0.0f then 0.0f
        else if value > 1.0f then 1.0f
        else value

    let F32ToSF32 value =
        if value < -1.0f then -1.0f
        else if value > 1.0f then 1.0f
        else value

    let FloatToUF32 value =
        if value < 0.0 then 0.0f
        else if value > 1.0 then 1.0f
        else System.Convert.ToSingle value

    let FloatToSF32 value =
        if value < -1.0 then -1.0f
        else if value > 1.0 then 1.0f
        else System.Convert.ToSingle value

    let inline AorB a b thresh value =
        if value < thresh then a
        else b

    let ScaleToSF32 (scale:float32) (a:float32[]) =
        a |> Array.map(fun x-> (x/scale)|>F32ToSF32)

    let ClipFractionSF32 (b:float32) (values:float32[]) =
        let bubble = Convert.ToInt32(Convert.ToSingle(values.Length - 1) * F32ToUF32(b))
        let sa = values |> Array.sortBy(fun x-> Math.Abs(x))
        values |> ScaleToSF32  (AbsF32(sa.[bubble]))

    type GroupShape =
        | Bag of int
        | Linear of int
        | Ring of int
        | Rectangle of IntPair
        | Torus of IntPair

    type SymmetricFormat =
        | UT
        | LT
        | Full

    type CompFormat =
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
       try
        if values.Length <> rowCount*colCount then 
            "Wrong array length in Array2DFromColumnMajor"  |> Rop.fail
        else (Array2D.init rowCount colCount 
                (fun x y -> values.[y+x*colCount])) |> Rop.succeed
       with
        | ex -> (sprintf "Array2DFromRowMajor message: %s"
                    ex.Message) |> Rop.fail

    let Array2DFromColumnMajor (rowCount:int) (colCount:int) (values:float32[]) =
       try
        if values.Length <> rowCount*colCount then 
            "Wrong array length in Array2DFromColumnMajor"  |> Rop.fail
        else (Array2D.init rowCount colCount 
                (fun x y -> values.[x+y*rowCount])) |> Rop.succeed
       with
        | ex -> (sprintf "Array2DFromColumnMajor message: %s"
                    ex.Message) |> Rop.fail


    let flattenRowMajor (A:'a[,]) = A |> Seq.cast<'a>

    let flattenColumnMajor (A:'a[,]) = A |> TransposeArray2D |> Seq.cast<'a>

    let getColumn c (A:_[,]) = A.[*,c] |> Seq.toArray

    let getRow r (A:_[,]) = A.[r,*] |> Seq.toArray

    let ZeroTheDiagonalF32 (mtx:float32[,]) = 
        Array2D.init (mtx.GetLength 0) 
                     (mtx.GetLength 1) 
                     (fun x y -> if(x=y) then 0.0f  else mtx.[x,y])


    let rec Skip (lst:List<'a>) =
        match lst with
        | [] -> []
        | a::b::c -> b::(Skip c)
        | a::b -> a::b
