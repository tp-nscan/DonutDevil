namespace LibNode
open Rop

    type IndexedF32A = { Index:int; Array:float32[]}
    type ArrayHistories = { Name:string; ArrayLength:int; IndexedArrays:List<IndexedF32A>}
    type DTVal<'a> = {X:int; Y:int; Val:'a}

module ArrayHistory =

    let AddHistory arrayHistories newHist newIndex = 
        { 
            Name=arrayHistories.Name; 
            ArrayLength=arrayHistories.ArrayLength; 
            IndexedArrays = { 
                                Index = newIndex;
                                Array = newHist |> Seq.take arrayHistories.ArrayLength
                                                |> Seq.toArray
                            }
                            :: arrayHistories.IndexedArrays
        }

