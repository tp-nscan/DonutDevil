namespace LibNode
open System
open System.Collections.Generic
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open MathUtils
open Generators
open NodeGroupBuilders


    type CliqueEnsemble = { States: Matrix<float32>; Connections: Matrix<float32> }


module SimpleNetwork =

    let CreateRandCesr (seed:int) (stateCount:int) (nodeCount:int) =
        let randIter = (Generators.RandSignedFloatsF32 seed OneF32).GetEnumerator()
        let getAndMove (iter:IEnumerator<float32>) =
            iter.MoveNext() |> ignore
            iter.Current

        { States = DenseMatrix.init nodeCount nodeCount (fun i j -> getAndMove randIter); 
          Connections=DenseMatrix.init stateCount nodeCount (fun i j -> getAndMove randIter) }
        
