namespace LibNode

open System
open System.Collections.Generic
open Rop

type EntityRepoMem() =

    let _dataDict = new System.Collections.Generic.Dictionary<DataId, DataRecord>()
    let _entityDict = new System.Collections.Generic.Dictionary<EntityId, Entity>()

    let DataKey (dk) =
        match dk with
        | DataId.GuidId g -> g.ToString()

    let EntityKey (dk) =
        match dk with
        | EntityId.GuidId g -> g.ToString()

    interface IEntityRepo with

        member this.GetEntity(entityId:EntityId) =
            try
               // sprintf "fsdf %s" "fdsadf" |> Rop.fail
                _entityDict.[entityId] |> Rop.succeed
            with
            | ex -> (sprintf "GetEntity Id: %s  message: %s" (EntityKey entityId) ex.Message) |> Rop.fail

        member this.GetData(dataId:DataId) =
            try
                _dataDict.Item(dataId) |> Rop.succeed
            with
            | ex -> (sprintf "GetData Id: %s  message: %s" (DataKey dataId) ex.Message) |> Rop.fail
                

        member this.SetData(genResult:GenResult) =
            let dr = EntityOps.ToDataRecord genResult
            try
                _dataDict.[dr.DataId] = dr |> ignore
                dr |> Rop.succeed
            with
            | ex -> (sprintf "SetData Id: %s  message: %s" (DataKey dr.DataId) ex.Message) |> Rop.fail

        member this.SetEntity(entity:Entity) =
            try
                _entityDict.[entity.EntityId] = entity |> ignore
                Rop.succeed "dasd"
            with
            | ex -> (sprintf "SetEntity Id: %s  message: %s" (EntityKey entity.EntityId) ex.Message) |> Rop.fail