namespace LibNode
open System
open MathUtils
open LibNode.Generators

    type Named<'a> = {name:string; value:'a}

    type Namoed<'a> = {name:string; value:unit->'a}

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
        | Dense of GroupShape * GroupShape * float32[,]
        | DenseSymmetric of SymmetricFormat * GroupShape * float32[][]
        | Sparse of GroupShape * GroupShape * CellF32[]

    type ConnectionSets = Named<ConnectionSet>[]

    type NodeSets =
        | Ns1D of Vs1D * GroupShape * Named<float32[]>[]
        | Ns2D of Vs2D * GroupShape * Named<PointF32[]>[]
        | Ns3D of Vs3D * GroupShape * Named<TripleF32[]>[]

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

    let PerturbNg1D (vs1D:Vs1D) (devMag:float32) (values:float32[]) =
        match vs1D with 
        | UnsignedBit ->  (FlipUF32A devMag values)
        | SignedBit ->  (FlipF32A devMag values)
        | UnitUnsigned ->  (PerturbInRangeF32A ZeroF32 OneF32 devMag values)
        | UnitSigned ->  (PerturbInRangeF32A NOneF32 OneF32 devMag values)
        | UnSigned max ->  (PerturbInRangeF32A ZeroF32 max devMag values)
        | Signed absMax ->  (PerturbInRangeF32A -absMax absMax devMag values)
        | Ring ->  (PerturbInRangeF32A ZeroF32 OneF32 devMag values)


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
        let nodeCount = NodeCount groupShape
        let myNg1D (justBits:bool) (unsigned:bool) (max:float32) = 
            let nodes = (RandomFloat32 justBits unsigned max) |> Seq.take nodeCount
            Ng1D(vs1D, groupShape, nodes|> Seq.toArray)

        match vs1D with        
        | UnsignedBit -> myNg1D true true OneF32
        | SignedBit -> myNg1D true false OneF32
        | UnitUnsigned -> myNg1D false true OneF32
        | UnitSigned -> myNg1D false false OneF32
        | UnSigned max -> myNg1D false true max
        | Signed max -> myNg1D false false max
        | Ring -> myNg1D false true OneF32



    let RandomNg2D (vs2D:Vs2D) (groupShape:GroupShape) =
        let nodeCount = NodeCount groupShape
        let myNodes = 
            match vs2D with
            | UnitTorus -> (RandomEuclideanPointsF32 false) |> Seq.take nodeCount |> Seq.toArray
            | Complex max  -> RandDiscPointsF32 OneF32 |> Seq.take nodeCount |> Seq.toArray
            | ComplexNormal -> RandRingPointsF32 |> Seq.take nodeCount |> Seq.toArray

        match vs2D with
        | UnitTorus -> Ng2D(vs2D, groupShape, myNodes)
        | Complex max  -> Ng2D(vs2D, groupShape, myNodes)
        | ComplexNormal -> Ng2D(vs2D, groupShape, myNodes)

    
module ConnectionSetBuilders =
    open NodeGroupBuilders

    let MakeRandomDense (gsFrom:GroupShape) (gsTo:GroupShape) (absMax:float32) =
        ConnectionSet.Dense (gsFrom, gsTo, (RandArray2DUF32 (NodeCount gsFrom) (NodeCount gsTo) absMax))


module MemoryBuilders =
    open NodeGroupBuilders

    let MakeRandomBinary (groupShape:GroupShape) =
        Memory.Binary (groupShape, (RandBools |> Seq.take (NodeCount groupShape) |> Seq.toArray))

    let MakeRandomBinaryDataBlock (groupShape:GroupShape) (count:int) (name:string) =
        let namedBoolArrays = [|for i in 0..count-1 -> 
                                {
                                    Named.name=sprintf "%s_%i" name i; 
                                    value=(RandBools |> Seq.take (NodeCount groupShape) |> Seq.toArray)
                                }
                              |]
        {Named.name=name; value=KvpList.Memories(Memories.Binary(groupShape, namedBoolArrays))}
       // DataBlock.KvpList({name=name; value=KvpList.Memories(Memories.Binary(groupShape, namedBoolArrays))})