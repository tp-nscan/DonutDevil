namespace LibNode

open System
open MathUtils
open LibNode.Generators
open System.Collections.Generic
open Rop

type ISym =
    abstract member Update: IDictionary<string,Param> -> RopResult<ISym,string>
    abstract member GetDataBlocks: unit -> RopResult<IDictionary<string,DataBlock>, string>
    abstract member Generation:int

type HopAlong (prams:IDictionary<string, Param>, 
               ensemble: float32[,], 
               generation:int) =

    let _dParams = prams
    let _ensemble = ensemble
    let _generation = generation

    interface ISym with
        member this.Update prams =
            let ha = new HopAlong(_dParams,  _ensemble, 0)
            Rop.succeed (ha :> ISym)
        member this.GetDataBlocks() = 
            this.DataBlocks
        member this.Generation = 
            _generation

    member this.Params
            with get() = _dParams

    member this.DataBlocks  
            with get() = Rop.fail "Not implemented"


module SymGen =
    let MakeHopAlong (prams:IDictionary<string, Param>) =
        //(RandomNg1D (Vs1D.UnSigned OneF32))  >>= (GetIntParam prams)
        None


        //    let MakeRandomLinearBinaryMemories(memLength:int) (memCount:int) (name:string) =
//        MemoryBuilders.MakeRandomBinaryDataBlock (GroupShape.Linear memLength) memCount name
//     
//    let HopAlongDict(memLength:int) (memCount:int) (ensembleSize:int) =
//       let mems = MakeRandomLinearBinaryMemories memLength memCount "Memories"
//
//       [| mems |] 
//            |> Array.map(fun p -> (DataBlockUtils.Name p), p)
//            |> Dict.ofArray