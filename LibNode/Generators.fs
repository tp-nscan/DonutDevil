namespace LibNode
module Generators =
    open MathNet.Numerics
    open MathNet.Numerics.Distributions
    open MathNet.Numerics.LinearAlgebra
    open MathNet.Numerics.LinearAlgebra.Matrix
    open MathNet.Numerics.Random

    
    let rng = Random.mersenneTwisterShared

    let RandomUnSignedBitsFloat32  (count:int) =
        let two = System.Convert.ToSingle(2)
        seq { for i in 1 .. count -> System.Convert.ToSingle(rng.Next(2)) }

    let RandomSignedBitsFloat32 (count:int) =
        let two = System.Convert.ToSingle(2)
        seq { for i in 1 .. count -> System.Convert.ToSingle(rng.Next(2)) }
        
    let RandomUnsignedIntervalFloat32 (max:float32) (count:int) =
        let two = System.Convert.ToSingle(2)
        seq { for i in 1 .. count -> System.Convert.ToSingle(rng.NextDouble()) * two * max - max }

    let RandomSignedIntervalFloat32 (max:float32) (count:int) =
        let two = System.Convert.ToSingle(2)
        seq { for i in 1 .. count -> System.Convert.ToSingle(rng.NextDouble() * 2.0) * max - max }

    let RandomIntervalFloat32 (unsigned:bool) (max:float32) (count:int) =
        if unsigned then
            RandomUnsignedIntervalFloat32 max count
        else
            RandomSignedIntervalFloat32 max count

    let RandomBitsFloat32 (unsigned:bool) (count:int) =
        if unsigned then
            RandomUnSignedBitsFloat32 count
        else
            RandomSignedBitsFloat32 count

    let RandomFloat32 (justBits:bool) (unsigned:bool) (max:float32) (count:int) =
        if justBits then
            RandomIntervalFloat32 unsigned max count
        else
            RandomBitsFloat32 unsigned count