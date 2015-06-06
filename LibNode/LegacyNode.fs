namespace LibNode
open System
open MathUtils
open LibNode.Generators

    type NodeGroupIndex = int

    type NodeValue = float32

    type Node = { Value: NodeValue; GroupIndex: NodeGroupIndex }

    type NodeGroup(arrayLength:int) =
        let float32Zero = System.Convert.ToSingle(0)
        let _array = Array.create (arrayLength) float32Zero

        member this.ArrayLength
            with get() = _array.Length

        member this.Values
            with get() = _array
                
        member this.AddNodes(nodes: seq<Node>) =
            Seq.iter (fun node -> _array.[node.GroupIndex]<-node.Value) nodes