namespace LibNode

open System
open Rop

type CliqueEnsemble(prams:Param list, entityId:EntityId, rowCount:int, 
                    colCount:int, seed:int, iteration:int,
                    float32Type:Float32Type) =
    let _sourceData = []
    let _params = prams
    let _entityId = entityId
    let _seed = seed
    let _iteration = iteration
    let _rowCount = rowCount
    let _colCount = colCount
    let _float32Type = float32Type

    interface IEntityGen with
        member this.EntityId = _entityId
        member this.GeneratorId =
            {Name="RandMatrixGenerator"; Version=1}
        member this.Iteration = 
            _iteration
        member this.SourceData =
            _sourceData
        member this.Params = 
            _params
        member this.GenResultStates = 
            [(IsFresh(true), Epn("Matrix"))]
        member this.GetGenResult(epn:Epn) = 
            match epn with
            | Epn("Matrix") -> 
                {
                    GenResult.EntityId = _entityId;
                    GenResult.Epn=Epn("Matrix"); 
                    GenResult.ArrayData = 
                        Float32Array( _float32Type, 
                                    (NtGens.RandFloat32Seed _seed float32Type) 
                                    |> Seq.take(_rowCount*_colCount) 
                        |> Seq.toArray);
                    } |> Rop.succeed
            
            | Epn(s) -> sprintf "DataChunkName %s not found" s |> Rop.fail


module CliqueEnsembleBuilder =

   let CreateCliqueEnsemble (prams:Param list) = 
        None