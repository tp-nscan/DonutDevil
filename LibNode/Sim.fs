namespace LibNode

open System
open MathUtils
open LibNode.Generators
open System.Collections.Generic
open Rop

type ISym =
    abstract member Update: unit -> RopResult<ISym,string>
    abstract member Params: IDictionary<string,Param>
    abstract member GetDataBlocks: unit -> RopResult<IDictionary<string,DataBlock>, string>
    abstract member Generation:int
    abstract member Id:Guid
    abstract member Messages:string list
    abstract member ParentId:Option<Guid>

type Sym (prams:IDictionary<string, Param>,
          id:Guid,
          parentId: Option<Guid>,
          symState: ISymState,
          messages: string list,
          generation:int) =

    let _dParams = prams
    let _symState = symState
    let _generation = generation
    let _messages = messages
    let _id = id
    let _parentId = parentId

    interface ISym with
        member this.Update() =
            match _symState.Update() with
            | Success (x,msgs) -> RopResult.Success(new Sym( _dParams, _id, _parentId, x, msgs, _generation + 1) :> ISym ,msgs)
            | Failure errors -> Failure errors
        member this.Params =
            _dParams
        member this.GetDataBlocks() = 
            RopResult.Failure []
        member this.Generation = 
            _generation
        member this.Id = 
            _id
        member this.Messages = 
            _messages
        member this.ParentId = 
            _parentId


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