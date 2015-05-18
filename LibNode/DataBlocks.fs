namespace LibNode
open System
open MathUtils

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
        | Complex of float32  // max vector length
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
        | Dense of MatrixFormat * GroupShape * GroupShape * float32[,]
        | DenseSymmetric of SymmetricFormat * GroupShape * float32[][]
        | Sparse of GroupShape * GroupShape * CellF32[]

    type ConnectionSets = Named<ConnectionSet>[]

    // for a named list of memories with consistent structure
    type NodeSets4 =
        | Binary of GroupShape * Named<float32[]>[]
        | Colors of GroupShape * Named<int[]>[]


    type NodeSets =
        | Ng1D of Vs1D * GroupShape *  Named<float32[]>[]
        | Ng2D of Vs2D * GroupShape * Named<PointF32[]>[]
        | Ng3D of Vs3D * GroupShape * Named<TripleF32[]>[]

    type NodeSet =
        | Ng1D of Vs1D * GroupShape * float32[]
        | Ng2D of Vs2D * GroupShape * PointF32[]
        | Ng3D of Vs3D * GroupShape * TripleF32[]

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
            let nodes = (RandomFloat32 justBits unsigned  max  ) |> Seq.take nodeCount
            Ng1D(vs1D, groupShape, nodes|> Seq.toArray)


        match vs1D with        
        | UnsignedBit -> myNg1D true true unitFloat
        | SignedBit -> myNg1D true false unitFloat
        | UnitUnsigned -> myNg1D false true unitFloat
        | UnitSigned -> myNg1D false false unitFloat
        | UnSigned max -> myNg1D false true max
        | Signed max -> myNg1D false false max
        | Ring -> myNg1D false true unitFloat