namespace LibNode
open MathUtils
open Rop

    type IndexedF32A = { Index:int; Array:float32[]}
    type ArrayHistories = { Name:string; ArrayLength:int; 
                            IndexedArrays:List<IndexedF32A>; 
                            TargetLength:int; TrimStep:int}
    type D2Val<'a> = {X:int; Y:int; Val:'a}

module ArrayHistory =

    let Init name newHist targetLength trimStep = 
        let firstArray = {
                            Index = 0;
                            Array = newHist |> Seq.toArray
                         }
        { 
            Name=name; 
            ArrayLength= firstArray.Array.GetLength(0); 
            IndexedArrays = [firstArray];
            TargetLength = targetLength;
            TrimStep = trimStep;
        }

    let rec Add arrayHistories newHist newIndex =
        match (arrayHistories.IndexedArrays.Length = arrayHistories.TargetLength) with
        | true -> 
            let trimmed = { 
                            Name=arrayHistories.Name
                            ArrayLength=arrayHistories.ArrayLength
                            IndexedArrays = { 
                                                Index = newIndex;
                                                Array = newHist |> Seq.take arrayHistories.ArrayLength
                                                                |> Seq.toArray
                                            }
                                            :: (Tithe arrayHistories.TrimStep arrayHistories.IndexedArrays)
                            TargetLength = arrayHistories.TargetLength
                            TrimStep = arrayHistories.TrimStep
                        }
            (Add trimmed newHist newIndex)
        | false ->
                { 
                    Name=arrayHistories.Name
                    ArrayLength=arrayHistories.ArrayLength
                    IndexedArrays = { 
                                        Index = newIndex;
                                        Array = newHist |> Seq.take arrayHistories.ArrayLength
                                                        |> Seq.toArray
                                    }
                                    :: arrayHistories.IndexedArrays
                    TargetLength = arrayHistories.TargetLength
                    TrimStep = arrayHistories.TrimStep
                }

    let GetD2Vals (arrayHistories:ArrayHistories) = 
        let rec d2vs (ahl:List<IndexedF32A>) yCoord =
            seq {
                match ahl with
                    | [] -> failwith "should never get here"
                    | head::[] -> 
                        yield! head.Array |> Seq.mapi(fun i ia -> { X=i; Y=yCoord; Val=ia} )
                    | head::tail -> 
                        yield! d2vs tail (yCoord - 1)
                        yield! head.Array |> Seq.mapi(fun i ia -> { X=i; Y=yCoord; Val=ia} )
                        
            }
        d2vs arrayHistories.IndexedArrays (arrayHistories.IndexedArrays.Length- 1)

    let GetIndxes (arrayHistories:ArrayHistories) = 
        arrayHistories.IndexedArrays |> List.toSeq |> Seq.map(fun la -> la.Index)
