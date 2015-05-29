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

    // a list of memories with uniform structure
    type Memories =
        | Binary of GroupShape * INamed<bool[]>[]
        | Colors of GroupShape * INamed<int[]>[]

    type Memory =
        | Binary of GroupShape * bool[]
        | Colors of GroupShape * int[]

    type ConnectionSet =
        | Dense of GroupShape * GroupShape * float32[,]
        | DenseSymmetric of SymmetricFormat * GroupShape * float32[][]
        | Sparse of GroupShape * GroupShape * CellF32[]

    type ConnectionSets = INamed<ConnectionSet>[]

   type NodeSets =
        | Ns1D of Vs1D * GroupShape * INamed<float32[]>[]
        | Ns2D of Vs2D * GroupShape * INamed<PointF32[]>[]
        | Ns3D of Vs3D * GroupShape * INamed<TripleF32[]>[]

    type NodeSet =
        | Ng1D of Vs1D * GroupShape * float32[]
        | Ng2D of Vs2D * GroupShape * PointF32[]
        | Ng3D of Vs3D * GroupShape * TripleF32[]

    // for a named list of memories with uniform structure
    type KvpList =
        | NodeSets of INamed<NodeSets>
        | ConnectionSets of INamed<ConnectionSets>
        | Memories of INamed<Memories>

    type DataBlock =
        | NodeSet of INamed<NodeSet>
        | ConnectionSet of INamed<ConnectionSet>
        | Memory of INamed<Memory>
        | KvpList of KvpList

module DataBlockUtils =
    let Name (dataBlock:DataBlock) =
        match dataBlock with
        | NodeSet ns -> ns.Name
        | ConnectionSet cs -> cs.Name
        | Memory mem -> mem.Name
        | KvpList kvp -> match kvp with
                         | NodeSets ns -> ns.Name
                         | ConnectionSets cs -> cs.Name
                         | Memories            mems -> mems.Name



module NodeGroupBuilders =

    let PerturbNg1D (seed:int) (vs1D:Vs1D) (devMag:float32) (values:float32[]) =
        match vs1D with 
        | UnsignedBit ->  (FlipUF32A seed devMag values)
        | SignedBit ->  (FlipF32A seed devMag values)
        | UnitUnsigned ->  (PerturbInRangeF32A seed ZeroF32 OneF32 devMag values)
        | UnitSigned ->  (PerturbInRangeF32A seed NOneF32 OneF32 devMag values)
        | UnSigned max ->  (PerturbInRangeF32A seed ZeroF32 max devMag values)
        | Signed absMax ->  (PerturbInRangeF32A seed -absMax absMax devMag values)
        | Ring ->  (PerturbInRangeF32A seed ZeroF32 OneF32 devMag values)


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


    let RandomNg1D (seed:int) (vs1D:Vs1D) (groupShape:GroupShape) =
        let nodeCount = NodeCount groupShape
        let myNg1D (justBits:bool) (unsigned:bool) (max:float32) = 
            let nodes = (RandomFloat32 seed justBits unsigned max) |> Seq.take nodeCount
            Ng1D(vs1D, groupShape, nodes|> Seq.toArray)

        match vs1D with        
        | UnsignedBit -> myNg1D true true OneF32
        | SignedBit -> myNg1D true false OneF32
        | UnitUnsigned -> myNg1D false true OneF32
        | UnitSigned -> myNg1D false false OneF32
        | UnSigned max -> myNg1D false true max
        | Signed max -> myNg1D false false max
        | Ring -> myNg1D false true OneF32


    let RandomNg2D (seed:int) (vs2D:Vs2D) (groupShape:GroupShape) =
        let nodeCount = NodeCount groupShape
        let myNodes = 
            match vs2D with
            | UnitTorus -> (RandomEuclideanPointsF32 seed false) |> Seq.take nodeCount |> Seq.toArray
            | Complex max  -> RandDiscPointsF32 seed OneF32 |> Seq.take nodeCount |> Seq.toArray
            | ComplexNormal -> RandRingPointsF32 seed |> Seq.take nodeCount |> Seq.toArray

        match vs2D with
        | UnitTorus -> Ng2D(vs2D, groupShape, myNodes)
        | Complex max  -> Ng2D(vs2D, groupShape, myNodes)
        | ComplexNormal -> Ng2D(vs2D, groupShape, myNodes)

    
module ConnectionSetBuilders =
    open NodeGroupBuilders

    let MakeRandomDense (seed:int) (gsFrom:GroupShape) (gsTo:GroupShape) (absMax:float32) =
        ConnectionSet.Dense (gsFrom, gsTo, (RandArray2DUF32 seed (NodeCount gsFrom) (NodeCount gsTo) absMax))


module MemoryBuilders =
    open NodeGroupBuilders

    let MakeRandomBinary (seed:int) (groupShape:GroupShape) =
        Memory.Binary (groupShape, (RandBools seed|> Seq.take (NodeCount groupShape) |> Seq.toArray))

    let MakeRandomNamedMemories (seed:int) (groupShape:GroupShape) (count:int) (name:string) =
        new NamedData<Memories>(
            name,
            Memories.Binary(groupShape,
                [|for i in 0..count-1 -> 
                    new NamedData<bool[]>(sprintf "%s_%i" name i, (RandBools seed|> Seq.take (NodeCount groupShape) |> Seq.toArray))
                    :> INamed<bool[]>
                |]
            )
        )

    let MakeRandomBinaryDataBlock (seed:int) (groupShape:GroupShape) (count:int) (name:string) =
        DataBlock.KvpList(
            KvpList.Memories(MakeRandomNamedMemories seed groupShape count name))