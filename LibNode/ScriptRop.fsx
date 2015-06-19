#r @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\LibNode\bin\Debug\LibNode.dll"
open System
open Rop

module FoldListo = 
    let le = [1;2;3;4;5] |> List.fold (fun acc x -> x :: acc) []

    let checkly ofWhat =
        match ofWhat with
        |1 |2 |3 |4 |5 |6 -> "good" |> Rop.succeedWithMsg ofWhat
        | _ -> "no good" |> Rop.fail

    let rec slap<'a,'b> (b: 'a -> RopResult<'b, string>) (ar: 'a list) (res:RopResult<'b, string> list) =
        match ar with
        | [] -> res
        | head :: tail -> slap b tail ((b head)::res)

    let shmak = slap checkly le []

    let pink = MergeResultList shmak