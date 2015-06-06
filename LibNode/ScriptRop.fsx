#r @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\LibNode\bin\Debug\LibNode.dll"
open System
open Rop


module String10 =
    type T = String10 of string

    let create (s:string) =
        match s with
        | null -> fail "StringError.Missing"
        | _ when s.Length > 10 -> fail "(MustNotBeLongerThan 10)"
        | _ -> succeed (String10 s)

    let apply f (String10 s) =
        f s

type PersonalName = {
    FirstName: String10.T 
    LastName: String10.T 
}

module wank =
    let createPersonalName firstName lastName = 
        {FirstName = firstName; LastName = lastName}

    let nameOrError fn ln = 
        createPersonalName
        <!> String10.create fn
        <*> String10.create ln

    type IntPair = {x:int; y:int}

    type GroupShape =
        | Bag of int
        | Linear of int
        | Ring of int
        | Rectangle of IntPair
        | Torus of IntPair

    type ConnectionSet =
        | Dense of GroupShape * GroupShape * int[]
        | Sparse of GroupShape * GroupShape *  float32[][]

    let flub = GroupShape.Torus {x=1; y=2}

    type Wank = | Bong of int*int
    let pin = Wank.Bong (1,2)


    let wub = ConnectionSet.Dense ( flub, flub, [|1; 2|])