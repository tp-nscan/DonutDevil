namespace LibNode
open MathUtils
open Rop

    type IndexedF32A = { Index:int; Array:float32[]}

    type ArrayHist = { Name:string; ArrayLength:int; 
                            IndexedArrays:List<IndexedF32A>; 
                            TargetLength:int;}

    type D2Val<'a> = {X:int; Y:int; Val:'a}

module ArrayHistory =
    
    let Init name arrayLength targetLength = 
        { 
            Name = name; 
            ArrayLength = arrayLength; 
            IndexedArrays = [];
            TargetLength = targetLength;
        }

//    let Init name newHist targetLength = 
//        let firstArray = {
//                            Index = 0;
//                            Array = newHist |> Seq.toArray
//                         }
//        { 
//            Name = name; 
//            ArrayLength = firstArray.Array.GetLength(0); 
//            IndexedArrays = [firstArray];
//            TargetLength = targetLength;
//        }

    let rec Add arrayHist newHist newIndex =
        match (arrayHist.IndexedArrays.Length = arrayHist.TargetLength) with
        | true -> 
            let trimmed = { 
                            Name=arrayHist.Name
                            ArrayLength=arrayHist.ArrayLength
                            IndexedArrays = (Skip arrayHist.IndexedArrays)
                            TargetLength = arrayHist.TargetLength
                          }
            (Add trimmed newHist newIndex)
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
                    TargetLength = arrayHist.TargetLength
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
