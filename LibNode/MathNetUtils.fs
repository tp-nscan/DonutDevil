namespace LibNode
open System
open MathNet.Numerics
open MathNet.Numerics.Random
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open Rop

module MathNetUtils =

    let DenseMatrixFloat32ZeroDiagional (matrix:Matrix<float32>) =
        DenseMatrix.init
            matrix.RowCount matrix.ColumnCount 
            (fun i j -> if(i=j) then 0.0f else matrix.[i,j])

