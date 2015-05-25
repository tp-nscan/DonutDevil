namespace LibNode
module SimpleNetwork =
    open System
    open MathNet.Numerics
    open MathNet.Numerics.Distributions
    open MathNet.Numerics.LinearAlgebra
    open MathNet.Numerics.LinearAlgebra.Matrix
    open MathNet.Numerics.Random

    open MathUtils
    open Generators
    open NodeGroupBuilders

    let MakeSnow(count:int) (name:string) =
        MemoryBuilders.MakeRandomBinaryDataBlock (GroupShape.Linear count) count name

