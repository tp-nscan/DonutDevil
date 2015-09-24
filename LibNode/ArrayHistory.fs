namespace LibNode
open MathUtils
open Rop

    type IndexedF32A = { Index:int; Array:float32[]}

    type ArrayHist = { Name:string; ArrayLength:int; 
                        IndexedArrays:List<IndexedF32A>;}

    type D1Val<'a> = {Index:int; Val:'a}
    type D2Val<'a> = {X:int; Y:int; Val:'a}

module ArrayHistory =
    
    let Init name arrayLength = 
        { 
            Name = name; 
            ArrayLength = arrayLength; 
            IndexedArrays = [];
        }

    let rec Add arrayHist newHist newIndex targetLength =
        match (arrayHist.IndexedArrays.Length = targetLength) with
        | true -> 
            let trimmed = { 
                            Name=arrayHist.Name
                            ArrayLength=arrayHist.ArrayLength
                            IndexedArrays = (Skip arrayHist.IndexedArrays)
                          }
            (Add trimmed newHist newIndex targetLength)
        | false ->
                { 
                    Name=arrayHist.Name
                    ArrayLength=arrayHist.ArrayLength
                    IndexedArrays = { 
                                        Index = newIndex;
                                        Array = newHist |> Seq.take arrayHist.ArrayLength
                                                        |> Seq.toArray
                                    }
                                    :: arrayHist.IndexedArrays
                }

    let GetD2Vals (arrayHist:ArrayHist) = 
        let rec d2vs (ahl:List<IndexedF32A>) yCoord =
            seq {
                match ahl with
                    | [] -> [] |> ignore
                    | head::[] -> 
                        yield! head.Array |> Seq.mapi(fun i ia -> { X=i; Y=yCoord; Val=ia} )
                    | head::tail -> 
                        yield! d2vs tail (yCoord - 1)
                        yield! head.Array |> Seq.mapi(fun i ia -> { X=i; Y=yCoord; Val=ia} )
                        
            }
        d2vs arrayHist.IndexedArrays (arrayHist.IndexedArrays.Length- 1)

    let GetIndxes (arrayHist:ArrayHist) = 
        arrayHist.IndexedArrays |> List.toSeq |> Seq.map(fun la -> la.Index)
