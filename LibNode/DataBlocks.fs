namespace LibNode
open System
open LibNode.Generators

    exception ErrorStr of string

    type Named<'a> = {name:string; value:'a}

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


    type IntPair = {x:int; y:int}
    type fPoint32 = {x:float32; y:float32 }
    type fTriple32 = {x:float32; y:float32; z:float32}

    // Ms (network geometry)
    type GroupShape =
        | Linear of int
        | Ring of int
        | Rectangle of IntPair
        | Torus of IntPair


    // The value options for when the data type for each node is one float
    type Vs1D =
        | UnsignedBit
        | SignedBit
        | UnitUnsigned
        | UnitSigned
        | UnSigned of float32 // max val
        | Signed of float32   // max abs val
        | Ring

    // The value options for when the data type for each node is two floats
    type Vs2D =
        | UnitTorus
        | Complex of float32  // max arg length
        | ComplexNormal // arg length = 1.0

    // The value options for when the data type for each node is three floats
    type Vs3D =
        | Open of float32 // max vector length
        | Normal

    // for a named list of memories with uniform structure
    type Memories =
        | Binary of GroupShape * Named<bool[]>[]
        | Colors of GroupShape * Named<int[]>[]

    type Memory =
        | Binary of GroupShape * bool[]
        | Colors of GroupShape * int[]

    type ConnectionSet =
        | DenseUnsigned of GroupShape * GroupShape * float32[][]
        | DenseSigned of GroupShape * GroupShape * float32[][]
        | SparseUnsigned of GroupShape * GroupShape * int * int * float32[]
        | SparseSigned of GroupShape * GroupShape * int * int * float32[]

    type ConnectionSets = Named<ConnectionSet>[]

    // for a named list of memories with uniform structure
    type NodeSets =
        | Binary of GroupShape * Named<bool[]>[]
        | Colors of GroupShape * Named<int[]>[]

    type NodeSet =
        | Ng1D of Vs1D * GroupShape * float32[]
        | Ng2D of Vs2D * GroupShape * fPoint32[]
        | Ng3D of Vs3D * GroupShape * fTriple32[]

    // for a named list of memories with uniform structure
    type KvpList =
        | NodeSets of NodeSets
        | ConnectionSet of ConnectionSets
        | Memories of Memories

    type DataBlock =
        | NodeSet of Named<NodeSet>
        | ConnectionSet of Named<ConnectionSet>
        | Memory of Named<Memory>
        | KvpList of Named<KvpList>


module NodeGroupBuilders =

    let ToGroupShape1D wrap count =
        if wrap then GroupShape.Linear count
        else GroupShape.Ring count
    
    let ToGroupShape2D wrap count =
        if wrap then GroupShape.Linear count
        else GroupShape.Ring count


    let NodeCount (groupShape:GroupShape) =
        match groupShape with
        | Linear length -> length
        | GroupShape.Ring length -> length
        | Rectangle ip -> ip.x * ip.y
        | Torus ip -> ip.x * ip.y

    let RandomNg1D (vs1D:Vs1D) (groupShape:GroupShape) =
        let unitFloat = System.Convert.ToSingle 1
        let nodeCount = NodeCount groupShape
        let myNg1D (justBits:bool) (unsigned:bool) (max:float32) = 
            let nodes = (RandomFloat32 justBits unsigned  max  nodeCount) |> Seq.toArray
            Ng1D(vs1D, groupShape, nodes)


        match vs1D with        
        | UnsignedBit -> myNg1D true true unitFloat
        | SignedBit -> myNg1D true true unitFloat
        | UnitUnsigned -> myNg1D false true unitFloat
        | UnitSigned -> myNg1D false true unitFloat
        | UnSigned max -> myNg1D false true unitFloat
        | Signed max -> myNg1D false true unitFloat
        | Ring -> myNg1D false true unitFloat