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

//    let womp = FilterFails shmak []
//    let bomp = FilterSuccess shmak []
//
//    let flimp = bomp |> List.collect(fun x->snd x)
//    let climp = bomp |> List.map(fun x->fst x)

//
//module String10 =
//    type T = String10 of string
//
//    let create (s:string) =
//        match s with
//        | null -> fail "StringError.Missing"
//        | _ when s.Length > 10 -> fail "(MustNotBeLongerThan 10)"
//        | _ -> succeed (String10 s)
//
//    let apply f (String10 s) =
//        f s
//
//type PersonalName = {
//    FirstName: String10.T 
//    LastName: String10.T 
//}
//
//module wank =
//    let createPersonalName firstName lastName = 
//        {FirstName = firstName; LastName = lastName}
//
//    let nameOrError fn ln = 
//        createPersonalName
//        <!> String10.create fn
//        <*> String10.create ln
//
//    type IntPair = {x:int; y:int}
//
//    type GroupShape =
//        | Bag of int
//        | Linear of int
//        | Ring of int
//        | Rectangle of IntPair
//        | Torus of IntPair
//
//    type ConnectionSet =
//        | Dense of GroupShape * GroupShape * int[]
//        | Sparse of GroupShape * GroupShape *  float32[][]
//
//    let flub = GroupShape.Torus {x=1; y=2}
//
//    type Wank = | Bong of int*int
//    let pin = Wank.Bong (1,2)
//
//
//    let wub = ConnectionSet.Dense ( flub, flub, [|1; 2|])