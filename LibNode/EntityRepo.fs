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

        member this.GetEntities() =
            try
                _entityDict |> Dict.toSeqValues |> Seq.toList |> Rop.succeed
            with
            | ex -> (sprintf "Error getting entities: %s" ex.Message) |> Rop.fail

        member this.GetEntity(entityId:EntityId) =
            try
                _entityDict.[entityId] |> Rop.succeed
            with
            | ex -> (sprintf "GetEntity Id: %s  message: %s" (EntityKey entityId) ex.Message) |> Rop.fail

        member this.GetData(dataId:DataId) =
            try
                _dataDict.Item(dataId) |> Rop.succeed
            with
            | ex -> (sprintf "GetData Id: %s  message: %s" (DataKey dataId) ex.Message) |> Rop.fail

        member this.SaveData (dataRecord:DataRecord) =
            try
                _dataDict.Add(dataRecord.DataId, dataRecord) |> ignore
                dataRecord |> Rop.succeed
            with
            | ex -> (sprintf "SaveData Id: %s  message: %s" (DataKey dataRecord.DataId) ex.Message) |> Rop.fail

        member this.SaveEntity(entity:Entity) =
            try
                _entityDict.Add(entity.EntityId, entity) |> ignore
                entity |> Rop.succeed
            with
            | ex -> (sprintf "SaveEntity Id: %s  message: %s" (EntityKey entity.EntityId) ex.Message) |> Rop.fail