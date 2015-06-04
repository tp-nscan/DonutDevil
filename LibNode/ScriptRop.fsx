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